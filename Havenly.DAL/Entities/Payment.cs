using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Entities;

[Index(nameof(BookingID), IsUnique = true)]
public class Payment
{
    [Key]
    public long PaymentID { get; set; }

    public long BookingID { get; set; }
    [ForeignKey(nameof(BookingID))]
    public Booking Booking { get; set; }

    [Required]
    [StringLength(50)]
    public string Gateway { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(100)]
    public string TransactionID { get; set; }   
}