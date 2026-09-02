using System;
using System.Collections.Generic;

namespace Havenly.BLL.ModelVMs.Account
{
    public class PublicUserProfileVM
    {
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
        public string Role { get; set; } = "Guest";
        public string Status { get; set; } = "Active";

        // Statistics
        public int TotalProperties { get; set; }
        public int TotalCompletedStays { get; set; }
        public int TotalReviewsReceived { get; set; }
        public double AverageHostRating { get; set; }

        // Host Listings (if host)
        public List<PublicPropertyCardVM> Properties { get; set; } = new();

        // Guest Past Stays (if guest)
        public List<PublicGuestStayVM> GuestStays { get; set; } = new();

        // Reviews Received
        public List<PublicUserReviewVM> Reviews { get; set; } = new();
    }

    public class PublicGuestStayVM
    {
        public long BookingId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = "/images/p1.jpg";
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string Status { get; set; } = "Completed";
    }

    public class PublicPropertyCardVM
    {
        public long PropertyId { get; set; }
        public long ListingId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string Category { get; set; } = "Design homes";
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public int NumberOfReviews { get; set; }
        public string ImageUrl { get; set; } = "/images/p1.jpg";
    }

    public class PublicUserReviewVM
    {
        public string ReviewerName { get; set; } = string.Empty;
        public string? ReviewerAvatar { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
