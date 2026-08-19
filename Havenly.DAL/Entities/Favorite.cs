using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Entities;

[Index(nameof(UserID), nameof(ListingID), IsUnique = true)]
public class Favorite
{
    [Key]
    public long FavoriteID { get; set; }

    public long UserID { get; set; }
    [ForeignKey(nameof(UserID))]
    public User User { get; set; }

    public long ListingID { get; set; }
    [ForeignKey(nameof(ListingID))]
    public Listing Listing { get; set; }

    public DateTime CreatedAt { get; set; }   
}