using System.Collections.Generic;
using System.Threading.Tasks;
using Havenly.BLL.ModelVMs.Notification;

namespace Havenly.BLL.Services.Abstractions
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string userId, string title, string message, string type = "System", string? targetUrl = null);
        Task<NotificationDropdownVM> GetDropdownDataAsync(string userId, int count = 10);
        Task<int> GetUnreadCountAsync(string userId);
        Task<bool> MarkAsReadAsync(long notificationId, string userId);
        Task<bool> MarkAllAsReadAsync(string userId);
    }
}
