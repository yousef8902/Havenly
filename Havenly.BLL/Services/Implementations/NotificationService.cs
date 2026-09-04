using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Havenly.BLL.ModelVMs.Notification;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Havenly.BLL.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly HavenlyDbContext _context;

        public NotificationService(HavenlyDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(string userId, string title, string message, string type = "System", string? targetUrl = null)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;

            try
            {
                var notification = new Notification
                {
                    UserID = userId,
                    Title = title,
                    Message = message,
                    Type = type,
                    TargetUrl = targetUrl,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NOTIFICATION ERROR] {ex.Message}");
            }
        }

        public async Task<NotificationDropdownVM> GetDropdownDataAsync(string userId, int count = 10)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new NotificationDropdownVM();
            }

            var unreadCount = await _context.Notifications
                .Where(n => n.UserID == userId && !n.IsRead)
                .CountAsync();

            var entities = await _context.Notifications
                .Where(n => n.UserID == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToListAsync();

            var now = DateTime.UtcNow;

            var items = entities.Select(n => new NotificationItemVM
            {
                NotificationId = n.NotificationID,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                TargetUrl = n.TargetUrl,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                TimeAgo = GetRelativeTime(n.CreatedAt, now),
                IconType = GetIconType(n.Type)
            }).ToList();

            return new NotificationDropdownVM
            {
                UnreadCount = unreadCount,
                Notifications = items
            };
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return 0;

            return await _context.Notifications
                .Where(n => n.UserID == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task<bool> MarkAsReadAsync(long notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationID == notificationId && n.UserID == userId);

            if (notification == null) return false;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;

            var unread = await _context.Notifications
                .Where(n => n.UserID == userId && !n.IsRead)
                .ToListAsync();

            if (!unread.Any()) return true;

            foreach (var item in unread)
            {
                item.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private static string GetRelativeTime(DateTime date, DateTime now)
        {
            var span = now - date;

            if (span.TotalSeconds < 60) return "just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
            return date.ToString("MMM dd");
        }

        private static string GetIconType(string type)
        {
            return (type?.ToLower()) switch
            {
                "booking" => "booking",
                "listing" => "listing",
                "account" => "account",
                "payout" => "payout",
                _ => "info"
            };
        }
    }
}
