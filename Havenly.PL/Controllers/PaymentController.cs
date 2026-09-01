using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Havenly.BLL.ModelVMs.Payment;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Havenly.PL.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymobService _paymobService;
        private readonly IBookingRepository _bookingRepository;

        public PaymentController(
            IPaymentService paymentService,
            IPaymobService paymobService,
            IBookingRepository bookingRepository)
        {
            _paymentService = paymentService;
            _paymobService = paymobService;
            _bookingRepository = bookingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout(long bookingId)
        {
            var checkoutDetails = await _paymentService.GetCheckoutDetailsAsync(bookingId);
            if (checkoutDetails == null)
            {
                TempData["ErrorMessage"] = "Booking not found.";
                return RedirectToAction("MyBookings", "Booking");
            }

            var booking = await _bookingRepository.GetById(bookingId);
            if (booking != null)
            {
                // Attempt real Paymob Token if configured
                var token = await _paymobService.CreatePaymentTokenAsync(
                    booking,
                    checkoutDetails.GuestEmail,
                    checkoutDetails.GuestName,
                    "+201000000000");

                if (!string.IsNullOrEmpty(token))
                {
                    checkoutDetails.PaymobIframeUrl = _paymobService.GetIframeUrl(token);
                }
            }

            return View(checkoutDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessSimulatedPayment(
            long paymentId,
            long bookingId,
            string paymentType,
            string? cardNumber,
            string? cardHolder,
            string? expiryDate,
            string? cvv,
            string? walletPhone,
            string? walletProvider)
        {
            if (paymentType == "wallet")
            {
                // Validate Mobile Wallet
                if (string.IsNullOrWhiteSpace(walletPhone))
                {
                    TempData["ErrorMessage"] = "Please enter your mobile wallet phone number.";
                    return RedirectToAction(nameof(Checkout), new { bookingId });
                }

                var cleanPhone = walletPhone.Replace(" ", "").Replace("-", "");
                var phoneRegex = new Regex(@"^(\+20|0)?1[0125][0-9]{8}$");
                if (!phoneRegex.IsMatch(cleanPhone))
                {
                    TempData["ErrorMessage"] = "Invalid wallet number. Please enter a valid mobile number.";
                    return RedirectToAction(nameof(Checkout), new { bookingId });
                }

                var providerName = string.IsNullOrWhiteSpace(walletProvider) ? "Mobile Wallet" : walletProvider;
                var method = $"{providerName} ({cleanPhone})";
                var txnId = $"PM-WAL-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(100000, 999999)}";

                var success = await _paymentService.CompletePaymentAsync(paymentId, txnId, method);
                if (success)
                {
                    return RedirectToAction(nameof(Success), new { paymentId });
                }
            }
            else
            {
                // Validate Card (Visa, Mastercard, Meeza)
                if (string.IsNullOrWhiteSpace(cardNumber))
                {
                    TempData["ErrorMessage"] = "Please enter a card number.";
                    return RedirectToAction(nameof(Checkout), new { bookingId });
                }

                var cleanCard = cardNumber.Replace(" ", "").Replace("-", "");
                if (!IsValidLuhn(cleanCard))
                {
                    TempData["ErrorMessage"] = "Invalid card number.";
                    return RedirectToAction(nameof(Checkout), new { bookingId });
                }

                if (string.IsNullOrWhiteSpace(cvv) || !Regex.IsMatch(cvv.Trim(), @"^[0-9]{3,4}$"))
                {
                    TempData["ErrorMessage"] = "Invalid CVV.";
                    return RedirectToAction(nameof(Checkout), new { bookingId });
                }

                if (!string.IsNullOrWhiteSpace(expiryDate))
                {
                    if (!IsValidExpiryDate(expiryDate.Trim()))
                    {
                        TempData["ErrorMessage"] = "Invalid expiry date.";
                        return RedirectToAction(nameof(Checkout), new { bookingId });
                    }
                }

                var brand = DetectCardBrand(cleanCard);
                var last4 = cleanCard.Length >= 4 ? cleanCard[^4..] : cleanCard;
                var method = $"{brand} Card (•••• {last4})";
                var txnId = $"PM-CARD-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(100000, 999999)}";

                var success = await _paymentService.CompletePaymentAsync(paymentId, txnId, method);
                if (success)
                {
                    return RedirectToAction(nameof(Success), new { paymentId });
                }
            }

            TempData["ErrorMessage"] = "Unable to complete transaction.";
            return RedirectToAction(nameof(Checkout), new { bookingId });
        }

        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> PaymobCallback()
        {
            var queryDict = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());

            bool isValid = _paymobService.ValidateHmac(queryDict);
            if (!isValid)
            {
                TempData["ErrorMessage"] = "Paymob signature verification failed.";
                return RedirectToAction("MyBookings", "Booking");
            }

            var successStr = queryDict.GetValueOrDefault("success");
            var isSuccess = string.Equals(successStr, "true", StringComparison.OrdinalIgnoreCase);
            var txnId = queryDict.GetValueOrDefault("id") ?? $"PM-{Guid.NewGuid().ToString()[..8]}";
            var orderIdStr = queryDict.GetValueOrDefault("merchant_order_id") ?? queryDict.GetValueOrDefault("order");

            // Extract booking ID
            long bookingId = 0;
            if (!string.IsNullOrEmpty(orderIdStr) && orderIdStr.StartsWith("BOOKING-"))
            {
                var parts = orderIdStr.Split('-');
                if (parts.Length > 1) long.TryParse(parts[1], out bookingId);
            }

            if (bookingId > 0)
            {
                var payment = await _paymentService.CreateOrGetPendingPaymentAsync(bookingId);
                if (isSuccess)
                {
                    await _paymentService.CompletePaymentAsync(payment.PaymentID, txnId, "Paymob Gateway");
                    return RedirectToAction(nameof(Success), new { paymentId = payment.PaymentID });
                }
                else
                {
                    await _paymentService.FailPaymentAsync(payment.PaymentID, "Transaction declined by Paymob.");
                }
            }

            TempData["ErrorMessage"] = "Payment was not successful.";
            return RedirectToAction("MyBookings", "Booking");
        }

        [HttpGet]
        public async Task<IActionResult> Success(long paymentId)
        {
            var receipt = await _paymentService.GetPaymentReceiptAsync(paymentId);
            if (receipt == null)
            {
                return RedirectToAction("MyBookings", "Booking");
            }

            return View(receipt);
        }

        /// <summary>
        /// Validates card number using the standard Luhn algorithm (Mod 10 Checksum).
        /// </summary>
        public static bool IsValidLuhn(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber)) return false;
            var digitsOnly = new string(cardNumber.Where(char.IsDigit).ToArray());
            if (digitsOnly.Length < 13 || digitsOnly.Length > 19) return false;

            int sum = 0;
            bool alternate = false;
            for (int i = digitsOnly.Length - 1; i >= 0; i--)
            {
                int n = digitsOnly[i] - '0';
                if (alternate)
                {
                    n *= 2;
                    if (n > 9) n -= 9;
                }
                sum += n;
                alternate = !alternate;
            }
            return (sum % 10 == 0);
        }

        private static bool IsValidExpiryDate(string expiry)
        {
            var parts = expiry.Split(new[] { '/', '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) return false;

            if (!int.TryParse(parts[0], out int month) || month < 1 || month > 12) return false;
            if (!int.TryParse(parts[1], out int year)) return false;

            if (year < 100) year += 2000; // 28 -> 2028

            var now = DateTime.UtcNow;
            if (year < now.Year) return false;
            if (year == now.Year && month < now.Month) return false;

            return true;
        }

        private static string DetectCardBrand(string cardNumber)
        {
            if (cardNumber.StartsWith("4")) return "Visa";
            if (cardNumber.StartsWith("51") || cardNumber.StartsWith("52") || cardNumber.StartsWith("53") || cardNumber.StartsWith("54") || cardNumber.StartsWith("55") || cardNumber.StartsWith("22") || cardNumber.StartsWith("27")) return "Mastercard";
            if (cardNumber.StartsWith("34") || cardNumber.StartsWith("37")) return "American Express";
            if (cardNumber.StartsWith("5078") || cardNumber.StartsWith("6858")) return "Meeza";
            return "Debit/Credit";
        }
    }
}
