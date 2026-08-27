using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Entities;

[Index(nameof(UserID), nameof(BookingID), IsUnique = true)]
public class Review
{
    [Key]
    public long ReviewID { get; set; }

    public string UserID { get; set; }
    [ForeignKey(nameof(UserID))]
    public User User { get; set; }

    public long BookingID { get; set; }
    [ForeignKey(nameof(BookingID))]
    public Booking Booking { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; }   
}