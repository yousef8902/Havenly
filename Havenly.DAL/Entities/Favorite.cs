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

    public string UserID { get;  set; }
    [ForeignKey(nameof(UserID))]
    public User User { get;  set; }

    public long ListingID { get;  set; }
    [ForeignKey(nameof(ListingID))]
    public Listing Listing { get;  set; }

    public DateTime CreatedAt { get;  set; }
    public void Create(string userId, long listingId)
    {
        UserID = userId;
        ListingID = listingId;
        CreatedAt = DateTime.UtcNow;
    }
}