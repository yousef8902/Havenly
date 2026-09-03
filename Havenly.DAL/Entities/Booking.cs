using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Havenly.DAL.Enums;

namespace Havenly.DAL.Entities;

public class Booking
{
    [Key]
    public long BookingID { get; private set; }

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

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public Payment Payment { get; set; }

    public ICollection<Review> Reviews { get; set; }

    public void Create(
        string guestUserId,
        long listingId,
        DateTime checkIn,
        DateTime checkOut,
        decimal totalPrice,
        BookingStatus status)
    {
        GuestUserID = guestUserId;
        ListingID = listingId;
        CheckIn = checkIn;
        CheckOut = checkOut;
        TotalPrice = totalPrice;
        Status = status;
        CreatedDate = DateTime.UtcNow;
        Reviews = new List<Review>();
    }



    public void UpdateDates(DateTime checkIn, DateTime checkOut)
    {
        CheckIn = checkIn;
        CheckOut = checkOut;
    }

    public void UpdateStatus(BookingStatus status)
    {
        Status = status;
    }

    public void AssignPayment(Payment payment)
    {
        Payment = payment;
    }
}