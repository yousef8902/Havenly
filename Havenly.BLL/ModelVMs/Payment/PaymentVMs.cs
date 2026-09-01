using System;
using System.Collections.Generic;
using Havenly.DAL.Entities;

namespace Havenly.BLL.ModelVMs.Payment
{
    public class CheckoutVM
    {
        public long BookingId { get; set; }
        public long PaymentId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyImage { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal TotalAmount { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        public bool IsTestMode { get; set; } = true;
        public string? PaymobIframeUrl { get; set; }
    }

    public class PaymentSuccessVM
    {
        public long BookingId { get; set; }
        public long PaymentId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Gateway { get; set; } = "Paymob";
        public DateTime PaidAt { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights { get; set; }
    }

    public class AdminPaymentHistoryVM
    {
        public decimal TotalGrossRevenue { get; set; }
        public decimal TotalPlatformCommission { get; set; }
        public decimal PendingHostPayouts { get; set; }
        public decimal CompletedHostPayouts { get; set; }
        public int TotalTransactions { get; set; }
        public List<PaymentRowVM> Payments { get; set; } = new();
    }

    public class PaymentRowVM
    {
        public long PaymentId { get; set; }
        public long BookingId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        public string HostName { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public decimal GrossAmount { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal HostPayout { get; set; }
        public bool IsPaidToHost { get; set; }
        public PaymentStatus Status { get; set; }
        public string Gateway { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    public class HostPayoutsVM
    {
        public decimal TotalGrossRevenue { get; set; }
        public decimal TotalPlatformFee { get; set; }
        public decimal TotalNetEarnings { get; set; }
        public decimal PaidOutEarnings { get; set; }
        public decimal PendingPayouts { get; set; }
        public int TotalBookingsCount { get; set; }
        public List<PaymentRowVM> PayoutRows { get; set; } = new();
    }
}
