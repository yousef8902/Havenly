using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Havenly.BLL.ModelVMs.Payment;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;

namespace Havenly.BLL.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IEmailServices _emailServices;

        public PaymentService(
            IPaymentRepository paymentRepo,
            IBookingRepository bookingRepo,
            IEmailServices emailServices)
        {
            _paymentRepo = paymentRepo;
            _bookingRepo = bookingRepo;
            _emailServices = emailServices;
        }

        public async Task<Payment> CreateOrGetPendingPaymentAsync(long bookingId)
        {
            var existing = await _paymentRepo.GetByBookingIdAsync(bookingId);
            if (existing != null)
                return existing;

            var booking = await _bookingRepo.GetById(bookingId);
            if (booking == null)
                throw new InvalidOperationException($"Booking #{bookingId} not found.");

            var payment = new Payment();
            payment.Create(
                paymentId: 0,
                bookingId: bookingId,
                gateway: "Paymob",
                amount: booking.TotalPrice,
                transactionId: $"TXN-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                platformFee: Math.Round(booking.TotalPrice * 0.10m, 2),
                hostPayoutAmount: Math.Round(booking.TotalPrice * 0.90m, 2),
                status: PaymentStatus.Pending
            );

            await _paymentRepo.Add(payment);
            await _paymentRepo.SaveChanges();
            return payment;
        }

        public async Task<bool> CompletePaymentAsync(long paymentId, string transactionId, string gateway = "Paymob")
        {
            var payment = await _paymentRepo.GetById(paymentId);
            if (payment == null) return false;

            payment.MarkCompleted(transactionId, gateway);
            _paymentRepo.Update(payment);
            await _paymentRepo.SaveChanges();

            // Keep Booking status as Pending so Host must review and Approve/Decline
            var booking = await _bookingRepo.GetById(payment.BookingID);
            if (booking != null)
            {
                booking.UpdateStatus(BookingStatus.Pending);
                _bookingRepo.Update(booking);
                await _bookingRepo.SaveChanges();

                // Send email notification to Host about the new booking request
                try
                {
                    var host = booking.Listing?.Property?.Owner;
                    var guest = booking.Guest;
                    var propertyName = booking.Listing?.Property?.PropertyName ?? $"Property #{booking.Listing?.PropertyID}";

                    if (host != null && !string.IsNullOrEmpty(host.Email))
                    {
                        int totalNights = Math.Max(1, (booking.CheckOut.Date - booking.CheckIn.Date).Days);
                        await _emailServices.SendBookingRequestToHostAsync(
                            host.Email,
                            host.Name ?? "Host",
                            guest?.Name ?? "Guest",
                            propertyName,
                            booking.CheckIn,
                            booking.CheckOut,
                            booking.TotalPrice,
                            totalNights,
                            booking.BookingID);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PAYMENT HOST NOTIFICATION ERROR] {ex.Message}");
                }
            }

            return true;
        }

        public async Task<bool> FailPaymentAsync(long paymentId, string? reason = null)
        {
            var payment = await _paymentRepo.GetById(paymentId);
            if (payment == null) return false;

            payment.MarkFailed(reason);
            _paymentRepo.Update(payment);
            await _paymentRepo.SaveChanges();
            return true;
        }

        public async Task<bool> ProcessHostPayoutAsync(long paymentId)
        {
            var payment = await _paymentRepo.GetById(paymentId);
            if (payment == null || payment.Status != PaymentStatus.Completed)
                return false;

            payment.MarkPayoutToHost();
            _paymentRepo.Update(payment);
            await _paymentRepo.SaveChanges();
            return true;
        }

        public async Task<AdminPaymentHistoryVM> GetPaymentHistoryAsync()
        {
            var payments = (await _paymentRepo.GetPaymentsWithDetailsAsync()).ToList();

            var rows = payments.Select(p => new PaymentRowVM
            {
                PaymentId = p.PaymentID,
                BookingId = p.BookingID,
                TransactionId = p.TransactionID,
                GuestName = p.Booking?.Guest?.Name ?? "Guest",
                GuestEmail = p.Booking?.Guest?.Email ?? string.Empty,
                HostName = p.Booking?.Listing?.Property?.Owner?.Name ?? "Host",
                PropertyName = p.Booking?.Listing?.Property?.PropertyName ?? $"Property #{p.Booking?.Listing?.PropertyID}",
                GrossAmount = p.Amount,
                PlatformFee = p.PlatformFee,
                HostPayout = p.HostPayoutAmount,
                IsPaidToHost = p.IsPaidToHost,
                Status = p.Status,
                Gateway = p.Gateway,
                CreatedAt = p.CreatedAt,
                PaidAt = p.PaidAt
            }).ToList();

            var completedPayments = payments.Where(p => p.Status == PaymentStatus.Completed).ToList();

            return new AdminPaymentHistoryVM
            {
                TotalGrossRevenue = completedPayments.Sum(p => p.Amount),
                TotalPlatformCommission = completedPayments.Sum(p => p.PlatformFee),
                PendingHostPayouts = completedPayments.Where(p => !p.IsPaidToHost).Sum(p => p.HostPayoutAmount),
                CompletedHostPayouts = completedPayments.Where(p => p.IsPaidToHost).Sum(p => p.HostPayoutAmount),
                TotalTransactions = payments.Count,
                Payments = rows
            };
        }

        public async Task<HostPayoutsVM> GetHostPayoutsAsync(string hostUserId)
        {
            var payments = (await _paymentRepo.GetPaymentsWithDetailsAsync()).ToList();

            var hostPayments = payments
                .Where(p => p.Booking?.Listing?.Property?.OwnerUserID == hostUserId)
                .ToList();

            var rows = hostPayments.Select(p => new PaymentRowVM
            {
                PaymentId = p.PaymentID,
                BookingId = p.BookingID,
                TransactionId = p.TransactionID,
                GuestName = p.Booking?.Guest?.Name ?? "Guest",
                GuestEmail = p.Booking?.Guest?.Email ?? string.Empty,
                HostName = p.Booking?.Listing?.Property?.Owner?.Name ?? "Host",
                PropertyName = p.Booking?.Listing?.Property?.PropertyName ?? $"Property #{p.Booking?.Listing?.PropertyID}",
                GrossAmount = p.Amount,
                PlatformFee = p.PlatformFee,
                HostPayout = p.HostPayoutAmount,
                IsPaidToHost = p.IsPaidToHost,
                Status = p.Status,
                Gateway = p.Gateway,
                CreatedAt = p.CreatedAt,
                PaidAt = p.PaidAt
            }).OrderByDescending(p => p.CreatedAt).ToList();

            // Only count approved / completed bookings as realized Host Net Earnings
            var approvedPayments = hostPayments
                .Where(p => p.Status == PaymentStatus.Completed &&
                           (p.Booking?.Status == BookingStatus.Approved || p.Booking?.Status == BookingStatus.Completed))
                .ToList();

            // Pending bookings where payment is completed are awaiting host decision
            var pendingPayments = hostPayments
                .Where(p => p.Status == PaymentStatus.Completed && p.Booking?.Status == BookingStatus.Pending)
                .ToList();

            return new HostPayoutsVM
            {
                TotalGrossRevenue = approvedPayments.Sum(p => p.Amount),
                TotalPlatformFee = approvedPayments.Sum(p => p.PlatformFee),
                TotalNetEarnings = approvedPayments.Sum(p => p.HostPayoutAmount),
                PaidOutEarnings = approvedPayments.Where(p => p.IsPaidToHost).Sum(p => p.HostPayoutAmount),
                PendingPayouts = pendingPayments.Sum(p => p.HostPayoutAmount),
                TotalBookingsCount = hostPayments.Count,
                PayoutRows = rows
            };
        }

        public async Task<CheckoutVM?> GetCheckoutDetailsAsync(long bookingId)
        {
            var booking = await _bookingRepo.GetById(bookingId);
            if (booking == null) return null;

            var payment = await CreateOrGetPendingPaymentAsync(bookingId);

            var nights = Math.Max((booking.CheckOut - booking.CheckIn).Days, 1);
            var property = booking.Listing?.Property;

            return new CheckoutVM
            {
                BookingId = booking.BookingID,
                PaymentId = payment.PaymentID,
                PropertyName = property?.PropertyName ?? "Havenly Stay",
                PropertyImage = property?.Images?.FirstOrDefault()?.ImagePath ?? "/images/p1.jpg",
                Location = property?.Address != null ? $"{property.Address.City}, {property.Address.Country}" : "Egypt",
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                Nights = nights,
                PricePerNight = booking.Listing?.Price ?? (booking.TotalPrice / nights),
                TotalAmount = booking.TotalPrice,
                GuestName = booking.Guest?.Name ?? "Guest",
                GuestEmail = booking.Guest?.Email ?? "guest@havenly.com"
            };
        }

        public async Task<PaymentSuccessVM?> GetPaymentReceiptAsync(long paymentId)
        {
            var payment = await _paymentRepo.GetById(paymentId);
            if (payment == null) return null;

            var booking = payment.Booking;
            var nights = booking != null ? Math.Max((booking.CheckOut - booking.CheckIn).Days, 1) : 1;
            var property = booking?.Listing?.Property;

            return new PaymentSuccessVM
            {
                BookingId = payment.BookingID,
                PaymentId = payment.PaymentID,
                TransactionId = payment.TransactionID,
                Amount = payment.Amount,
                Gateway = payment.Gateway,
                PaidAt = payment.PaidAt ?? DateTime.UtcNow,
                PropertyName = property?.PropertyName ?? "Havenly Stay",
                Location = property?.Address != null ? $"{property.Address.City}, {property.Address.Country}" : "Egypt",
                CheckIn = booking?.CheckIn ?? DateTime.Today,
                CheckOut = booking?.CheckOut ?? DateTime.Today.AddDays(1),
                Nights = nights
            };
        }
    }
}
