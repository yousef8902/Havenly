using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Entities;

[Index(nameof(UserID), nameof(ListingID), IsUnique = true)]
public class Favorite
{
    [Key]
    public long FavoriteID { get; private set; }

    public long UserID { get; private set; }
    [ForeignKey(nameof(UserID))]
    public User User { get; private set; }

    public long ListingID { get; private set; }
    [ForeignKey(nameof(ListingID))]
    public Listing Listing { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public void Create(long userId, long listingId)
    {
        UserID = userId;
        ListingID = listingId;
        CreatedAt = DateTime.UtcNow;
    }
}
