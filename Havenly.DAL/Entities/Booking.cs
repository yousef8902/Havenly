using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Havenly.DAL.Enums;

namespace Havenly.DAL.Entities;

public class Booking
{
    [Key]
    public long BookingID { get; set; }

    public string GuestUserID { get; set; }
    [ForeignKey(nameof(GuestUserID))]
    public User Guest { get; set; }

    public long ListingID { get; set; }
    [ForeignKey(nameof(ListingID))]
    public Listing Listing { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime CheckIn { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime CheckOut { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    [Required]
    public BookingStatus Status { get; set; }

    public Payment Payment { get; set; }
    public ICollection<Review> Reviews { get; set; }   
}