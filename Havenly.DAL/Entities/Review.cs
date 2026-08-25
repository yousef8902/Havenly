using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Entities;

[Index(nameof(UserID), nameof(BookingID), IsUnique = true)]
public class Review
{
    [Key]
    public long ReviewID { get; private set; }

    public long UserID { get; private set; }
    [ForeignKey(nameof(UserID))]
    public User User { get; private set; }

    public long BookingID { get; private set; }
    [ForeignKey(nameof(BookingID))]
    public Booking Booking { get; private set; }

    public int Rating { get; private set; }

    public string Comment { get; private set; }

    public void Create(long reviewId, long userId, long bookingId, int rating, string comment = null)
    {
        ReviewID = reviewId;
        UserID = userId;
        BookingID = bookingId;
        Rating = rating;
        Comment = comment;
    }

    public void Update(int rating, string comment)
    {
        Rating = rating;
        Comment = comment;
    }
}
