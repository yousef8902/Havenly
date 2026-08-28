using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Entities;

[Index(nameof(UserID), nameof(BookingID), IsUnique = true)]
public class Review
{
    [Key]
    public  long ReviewID { get; private set; }

    public string UserID { get;  set; }
    [ForeignKey(nameof(UserID))]
    public User User { get;  set; }

    public long BookingID { get;  set; }
    [ForeignKey(nameof(BookingID))]
    public Booking Booking { get;  set; }

    public int Rating { get;  set; }

    public string Comment { get;  set; }

    public string? HostResponse { get;  set; }

    public void Create(long reviewId, string userId, long bookingId, int rating, string comment = null)
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

    public void RespondToReview(string response)
    {
        HostResponse = response;
    }
}