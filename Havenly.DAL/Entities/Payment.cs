using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Entities;

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
    public string Gateway { get; private set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; private set; }

    [Required]
    [StringLength(100)]
    public string TransactionID { get; private set; }

    public void Create(long paymentId, long  bookingId, string gateway, decimal amount, string transactionId)
    {
        PaymentID = paymentId;
        BookingID = bookingId;
        Gateway = gateway;
        Amount = amount;
        TransactionID = transactionId;
    }

    public void Update(string gateway, decimal amount, string transactionId)
    {
        Gateway = gateway;
        Amount = amount;
        TransactionID = transactionId;
    }
}
