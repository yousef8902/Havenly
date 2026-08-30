using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Entities;

[Index(nameof(UserID), nameof(PropertyID), IsUnique = true)]
public class Review
{
    [Key]
    public  long ReviewID { get; private set; }

    public string UserID { get;  set; }
    [ForeignKey(nameof(UserID))]
    public User User { get;  set; }

<<<<<<< HEAD
    public long BookingID { get;  set; }
    [ForeignKey(nameof(BookingID))]
    public Booking Booking { get;  set; }

    public int Rating { get;  set; }

    public string Comment { get;  set; }

    public string? HostResponse { get;  set; }

    public void Create(long reviewId, string userId, long bookingId, int rating, string comment = null)
=======
    public long PropertyID { get;  set; }
    [ForeignKey(nameof(PropertyID))]
    public Property Property { get;  set; }
    
    public int Rating { get;  set; }

    public string Comment { get;  set; }

    public string? HostResponse { get;  set; }

    public void Create(long reviewId, string userId, long PropertyID, int rating, string comment = null)
>>>>>>> 63b1b37 (add search by review and change relation between (review->booking) to (review->user))
    {
        ReviewID = reviewId;
        UserID = userId;
        PropertyID = PropertyID;
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