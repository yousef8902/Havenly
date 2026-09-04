using System.Security.Claims;
using System.Threading.Tasks;
using Havenly.BLL.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Havenly.PL.Controllers
{
    [Authorize]
    [Route("Notification")]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpGet("GetDropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var data = await _notificationService.GetDropdownDataAsync(userId);
            return Json(new { success = true, unreadCount = data.UnreadCount, notifications = data.Notifications });
        }

        [HttpGet("UnreadCount")]
        public async Task<IActionResult> UnreadCount()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Json(new { success = true, unreadCount = count });
        }

        [HttpPost("MarkRead/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(long id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _notificationService.MarkAsReadAsync(id, userId);
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId);
            return Json(new { success, unreadCount });
        }

        [HttpPost("MarkAllRead")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllRead()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _notificationService.MarkAllAsReadAsync(userId);
            return Json(new { success, unreadCount = 0 });
        }
    }
}
