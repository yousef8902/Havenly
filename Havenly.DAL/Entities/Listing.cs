using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Entities;

[Index(nameof(PropertyID), IsUnique = true)]
public class Listing
{
    [Key]
    public long ListingID { get; set; }

    public long PropertyID { get; set; }
    [ForeignKey(nameof(PropertyID))]
    public Property Property { get; set; }

    public string Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public bool IsValid { get; set; }

    public ICollection<Favorite> Favorites { get; set; }
    public ICollection<Booking> Bookings { get; set; }
}