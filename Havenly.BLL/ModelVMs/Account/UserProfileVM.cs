using System;
using System.Collections.Generic;
using Havenly.BLL.ModelVMs;

namespace Havenly.BLL.ModelVMs.Account
{
    public class UserProfileVM
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
        public int TotalBookings { get; set; }
        public int UpcomingBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int TotalFavorites { get; set; }
        public int TotalReviews { get; set; }
        public List<BookingDetailsVM> RecentBookings { get; set; } = new();
    }
}
