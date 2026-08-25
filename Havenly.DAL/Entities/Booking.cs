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

    public long GuestUserID { get; private set; }
    [ForeignKey(nameof(GuestUserID))]
    public User Guest { get; private set; }

    public long ListingID { get; private set; }
    [ForeignKey(nameof(ListingID))]
    public Listing Listing { get; private set; }

    [Column(TypeName = "datetime2")]
    public DateTime CheckIn { get; private set; }

    [Column(TypeName = "datetime2")]
    public DateTime CheckOut { get; private set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; private set; }

    [Required]
    public BookingStatus Status { get; private set; }

    public Payment Payment { get; private set; }
    public ICollection<Review> Reviews { get; private set; }
    public void Create(long bookingId, long guestUserId, long listingId, DateTime checkIn, DateTime checkOut, decimal totalPrice, BookingStatus status)
    {
        BookingID = bookingId;
        GuestUserID = guestUserId;
        ListingID = listingId;
        CheckIn = checkIn;
        CheckOut = checkOut;
        TotalPrice = totalPrice;
        Status = status;
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
