using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Entities;

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4
}

[Index(nameof(BookingID), IsUnique = true)]
public class Payment
{
    [Key]
    public long PaymentID { get; private set; }

    public long BookingID { get; private set; }
    [ForeignKey(nameof(BookingID))]
    public Booking Booking { get; private set; }

    [Required]
    [StringLength(50)]
    public string Gateway { get; private set; } = "Paymob";

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; private set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PlatformFee { get; private set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal HostPayoutAmount { get; private set; } = 0;

    public bool IsPaidToHost { get; private set; } = false;

    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;

    [Required]
    [StringLength(100)]
    public string TransactionID { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? PaidAt { get; private set; }

    public void Create(
        long paymentId,
        long bookingId,
        string gateway,
        decimal amount,
        string transactionId,
        decimal platformFee = 0,
        decimal hostPayoutAmount = 0,
        PaymentStatus status = PaymentStatus.Pending)
    {
        PaymentID = paymentId;
        BookingID = bookingId;
        Gateway = string.IsNullOrWhiteSpace(gateway) ? "Paymob" : gateway;
        Amount = amount;
        TransactionID = string.IsNullOrWhiteSpace(transactionId) ? $"TXN-{Guid.NewGuid().ToString()[..8].ToUpper()}" : transactionId;
        PlatformFee = platformFee > 0 ? platformFee : Math.Round(amount * 0.10m, 2); // 10% platform commission default
        HostPayoutAmount = hostPayoutAmount > 0 ? hostPayoutAmount : (amount - PlatformFee);
        Status = status;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkCompleted(string transactionId, string? gateway = null)
    {
        if (!string.IsNullOrWhiteSpace(transactionId))
            TransactionID = transactionId;

        if (!string.IsNullOrWhiteSpace(gateway))
            Gateway = gateway;

        Status = PaymentStatus.Completed;
        PaidAt = DateTime.UtcNow;
    }

    public void MarkFailed(string? reason = null)
    {
        Status = PaymentStatus.Failed;
    }

    public void MarkPayoutToHost()
    {
        IsPaidToHost = true;
    }

    public void Update(string gateway, decimal amount, string transactionId)
    {
        Gateway = gateway;
        Amount = amount;
        TransactionID = transactionId;
    }
}
