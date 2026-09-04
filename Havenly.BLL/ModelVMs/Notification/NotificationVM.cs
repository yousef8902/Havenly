using System;
using System.Collections.Generic;

namespace Havenly.BLL.ModelVMs.Notification
{
    public class NotificationItemVM
    {
        public long NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "System"; // Booking, Listing, Account, Payout, System
        public string? TargetUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
        public string IconType { get; set; } = "info"; // booking, approval, payout, alert, info
    }

    public class NotificationDropdownVM
    {
        public int UnreadCount { get; set; }
        public List<NotificationItemVM> Notifications { get; set; } = new();
    }
}
