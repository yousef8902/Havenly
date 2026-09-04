using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Havenly.PL.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class AdminController : Controller
    {
        private readonly IListingServices _listingService;
        private readonly IReportService _reportService;
        private readonly IAdminService _adminService;
        private readonly IUserManagementService _userManagementService;
        private readonly IBookingRepository _bookingRepository;
        private readonly IPaymentService _paymentService;
        private readonly IEmailServices _emailServices;
        private readonly IListingRepository _listingRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly UserManager<User> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IPaymentRepository _paymentRepository;

        public AdminController(
            IListingServices listingService,
            IReportService reportService,
            IAdminService adminService,
            IUserManagementService userManagementService,
            IBookingRepository bookingRepository,
            IPaymentService paymentService,
            IEmailServices emailServices,
            IListingRepository listingRepository,
            IPropertyRepository propertyRepository,
            UserManager<User> userManager,
            INotificationService notificationService,
            IPaymentRepository paymentRepository)
        {
            _listingService = listingService;
            _reportService = reportService;
            _adminService = adminService;
            _userManagementService = userManagementService;
            _bookingRepository = bookingRepository;
            _paymentService = paymentService;
            _emailServices = emailServices;
            _listingRepository = listingRepository;
            _propertyRepository = propertyRepository;
            _userManager = userManager;
            _notificationService = notificationService;
            _paymentRepository = paymentRepository;
        }

        public async Task<IActionResult> DashBoard()
        {
            var vm = new AdminDashboardVM
            {
                Stats = await _reportService.GetPlatformStats(),

                PendingListings = await _listingService.GetPendingListings(),

                MembersResult = await _adminService.GetMembersDataAsync(
                    null,
                    null,
                    1,
                    5),

                MonthlyStats = await _reportService.GetMonthlyStats()
            };

            return View("Index", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(long id)
        {
            var success = await _listingService.ApproveListing(id);
            TempData["Message"] = success ? "Listing approved successfully." : "Listing not found or could not be approved.";

            if (success)
            {
                try
                {
                    var listing = await _listingRepository.GetById(id);
                    var property = listing?.Property ?? (listing != null ? await _propertyRepository.GetById(listing.PropertyID) : null);
                    var host = property?.Owner ?? (property != null ? await _userManager.FindByIdAsync(property.OwnerUserID) : null);

                    if (host != null && !string.IsNullOrEmpty(host.Email))
                    {
                        await _emailServices.SendListingDecisionToHostAsync(
                            host.Email,
                            host.Name ?? "Host",
                            property?.PropertyName ?? "Havenly Stay",
                            true,
                            property?.PropertyID ?? 0);
                    }

                    if (host != null)
                    {
                        await _notificationService.CreateNotificationAsync(
                            host.Id,
                            "Listing Approved! 🎉",
                            $"Congratulations! Your listing '{property?.PropertyName ?? "Havenly Stay"}' has been approved and is now live on Havenly.",
                            "Listing",
                            $"/Property/Detail/{property?.PropertyID}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[HOST APPROVE NOTICE ERROR] {ex.Message}");
                }
            }

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            return RedirectToAction(nameof(DashBoard));
        }

        [HttpPost]
        public async Task<IActionResult> Decline(long id)
        {
            var success = await _listingService.DeclineListing(id);
            TempData["Message"] = success ? "Listing declined." : "Listing not found or could not be declined.";

            if (success)
            {
                try
                {
                    var listing = await _listingRepository.GetById(id);
                    var property = listing?.Property ?? (listing != null ? await _propertyRepository.GetById(listing.PropertyID) : null);
                    var host = property?.Owner ?? (property != null ? await _userManager.FindByIdAsync(property.OwnerUserID) : null);

                    if (host != null && !string.IsNullOrEmpty(host.Email))
                    {
                        await _emailServices.SendListingDecisionToHostAsync(
                            host.Email,
                            host.Name ?? "Host",
                            property?.PropertyName ?? "Havenly Stay",
                            false,
                            property?.PropertyID ?? 0);
                    }

                    if (host != null)
                    {
                        await _notificationService.CreateNotificationAsync(
                            host.Id,
                            "Listing Review Update",
                            $"Your listing '{property?.PropertyName ?? "Havenly Stay"}' was not approved at this time. Please review guidelines or edit your listing.",
                            "Listing",
                            "/Host/ListProperties");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[HOST DECLINE NOTICE ERROR] {ex.Message}");
                }
            }

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            return RedirectToAction(nameof(DashBoard));
        }

        public async Task<IActionResult> Members(UserStatus? status = null, string search = "", int page = 1)
        {
            var model = await _adminService.GetMembersDataAsync(status, search, page, 20);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string userId, bool isActive)
        {
            bool success = isActive
                ? await _userManagementService.ReinstateUser(userId)
                : await _userManagementService.SuspendUser(userId);

            TempData["Message"] = success
                ? (isActive ? "Member account reinstated (Active)." : "Member account suspended.")
                : "Unable to update member status.";

            return RedirectToAction(nameof(Members));
        }

        public async Task<IActionResult> Listings(string status = "all", string search = "", string sort = "newest", int page = 1)
        {
            ListingStatus? listingStatus = status?.ToLower() switch
            {
                "pending" => ListingStatus.Pending,
                "approved" => ListingStatus.Approved,
                "declined" => ListingStatus.Declined,
                "all" => null,
                _ => null
            };

            var model = await _adminService.GetListingsDataAsync(listingStatus, search, sort, page, 20);
            return View(model);
        }

        public async Task<IActionResult> Bookings(
            string status = "all",
            string search = "",
            string sort = "newest",
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1)
        {
            BookingStatus? bookingStatus = status?.ToLower() switch
            {
                "pending" => BookingStatus.Pending,
                "confirmed" => BookingStatus.Approved,
                "cancelled" => BookingStatus.Cancelled,
                "completed" => BookingStatus.Completed,
                "all" => null,
                _ => null
            };

            var model = await _adminService.GetBookingsDataAsync(
                bookingStatus,
                search,
                fromDate,
                toDate,
                page,
                20);

            return View(model);
        }

        [HttpGet]
        public IActionResult MemberDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction(nameof(Members));
            }
            return RedirectToAction("ViewProfile", "Account", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveHost(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Host not found.";
                return RedirectToAction(nameof(Members));
            }

            var success = await _userManagementService.ApproveHost(userId);
            if (success)
            {
                try
                {
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        await _emailServices.SendHostApplicationDecisionAsync(user.Email, user.Name ?? "Host", true);
                    }
                    await _notificationService.CreateNotificationAsync(
                        userId,
                        "Host Account Approved! 🌟",
                        "Congratulations! Your host application has been approved. You can now publish listings and host guests on Havenly.",
                        "Account",
                        "/Host/Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EMAIL HOST APPROVE ERROR] {ex.Message}");
                }

                TempData["Message"] = $"Host {user.Name} has been approved and activated.";
            }
            else
            {
                TempData["Error"] = "Failed to approve host.";
            }

            var referer = Request.Headers["Referer"].ToString();
            return !string.IsNullOrEmpty(referer) ? Redirect(referer) : RedirectToAction(nameof(Members));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectHost(string userId, string? reason = null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Host not found.";
                return RedirectToAction(nameof(Members));
            }

            var success = await _userManagementService.RejectHost(userId);
            if (success)
            {
                try
                {
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        await _emailServices.SendHostApplicationDecisionAsync(user.Email, user.Name ?? "Host", false, reason);
                    }
                    await _notificationService.CreateNotificationAsync(
                        userId,
                        "Host Application Update",
                        $"Your host application was declined. {(string.IsNullOrWhiteSpace(reason) ? "" : $"Reason: {reason}")}",
                        "Account",
                        "/Account/ViewProfile");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EMAIL HOST REJECT ERROR] {ex.Message}");
                }

                TempData["Message"] = $"Host application for {user.Name} has been declined.";
            }
            else
            {
                TempData["Error"] = "Failed to decline host application.";
            }

            var referer = Request.Headers["Referer"].ToString();
            return !string.IsNullOrEmpty(referer) ? Redirect(referer) : RedirectToAction(nameof(Members));
        }

        public async Task<IActionResult> BookingDetails(long id)
        {
            var booking = await _bookingRepository.GetAll()
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Address)
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Images)
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Owner)
                .Include(b => b.Guest)
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction(nameof(Bookings));
            }

            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentHistory()
        {
            var model = await _paymentService.GetPaymentHistoryAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessHostPayout(long paymentId)
        {
            var success = await _paymentService.ProcessHostPayoutAsync(paymentId);
            TempData["Message"] = success
                ? "Host payout marked as paid successfully."
                : "Unable to process host payout.";

            if (success)
            {
                try
                {
                    var payment = (await _paymentRepository.GetPaymentsWithDetailsAsync()).FirstOrDefault(p => p.PaymentID == paymentId);
                    var hostId = payment?.Booking?.Listing?.Property?.OwnerUserID;
                    if (!string.IsNullOrEmpty(hostId))
                    {
                        await _notificationService.CreateNotificationAsync(
                            hostId,
                            "Payout Disbursed 💰",
                            $"A payout of {payment.HostPayoutAmount:N2} EGP for booking #{payment.BookingID} ({payment.Booking?.Listing?.Property?.PropertyName ?? "Listing"}) has been marked as disbursed.",
                            "Payout",
                            "/Host/Payouts");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[NOTIFICATION PAYOUT ERROR] {ex.Message}");
                }
            }

            return RedirectToAction(nameof(PaymentHistory));
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyActivityDetails(int year, int month, string type)
        {
            if (year <= 2000 || month < 1 || month > 12)
            {
                return BadRequest(new { success = false, message = "Invalid month or year parameters." });
            }

            var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endDate = startDate.AddMonths(1);
            var monthName = startDate.ToString("MMMM yyyy");

            if (string.Equals(type, "signups", StringComparison.OrdinalIgnoreCase))
            {
                var users = await _userManager.Users
                    .Where(u => u.JoinedDate >= startDate && u.JoinedDate < endDate)
                    .OrderByDescending(u => u.JoinedDate)
                    .ToListAsync();

                var items = users.Select(u => new
                {
                    id = u.Id,
                    name = u.Name ?? u.UserName ?? "Member",
                    email = u.Email ?? "N/A",
                    role = u.Role ?? "Guest",
                    joinedDate = u.JoinedDate.ToString("MMM dd, yyyy"),
                    status = u.Status.ToString(),
                    statusDisplay = u.Status.ToString(),
                    statusColor = u.Status switch
                    {
                        UserStatus.Active => "bg-emerald-100 text-emerald-800",
                        UserStatus.PendingApproval => "bg-amber-100 text-amber-800",
                        UserStatus.Suspended => "bg-red-100 text-red-800",
                        _ => "bg-gray-100 text-gray-800"
                    }
                }).ToList();

                return Json(new
                {
                    success = true,
                    type = "signups",
                    monthName,
                    year,
                    month,
                    count = items.Count,
                    items
                });
            }
            else
            {
                var bookings = await _bookingRepository.GetAll()
                    .Include(b => b.Listing)
                        .ThenInclude(l => l.Property)
                            .ThenInclude(p => p.Images)
                    .Include(b => b.Listing)
                        .ThenInclude(l => l.Property)
                            .ThenInclude(p => p.Owner)
                    .Include(b => b.Guest)
                    .Where(b => b.CreatedDate >= startDate && b.CreatedDate < endDate)
                    .OrderByDescending(b => b.CreatedDate)
                    .ToListAsync();

                var items = bookings.Select(b => new
                {
                    bookingId = b.BookingID,
                    propertyId = b.Listing?.PropertyID ?? 0,
                    propertyName = b.Listing?.Property?.PropertyName ?? ("Listing #" + b.ListingID),
                    imageUrl = b.Listing?.Property?.Images?.FirstOrDefault(i => i.IsPrimary == true)?.ImagePath
                               ?? b.Listing?.Property?.Images?.FirstOrDefault()?.ImagePath
                               ?? "/images/p1.jpg",
                    guestName = b.Guest?.Name ?? b.Guest?.UserName ?? "Guest",
                    hostName = b.Listing?.Property?.Owner?.Name ?? "Host",
                    checkIn = b.CheckIn.ToString("MMM dd, yyyy"),
                    checkOut = b.CheckOut.ToString("MMM dd, yyyy"),
                    total = b.TotalPrice.ToString("N2"),
                    status = b.Status.ToString(),
                    statusColor = b.Status switch
                    {
                        BookingStatus.Completed => "bg-emerald-100 text-emerald-800",
                        BookingStatus.Approved => "bg-blue-100 text-blue-800",
                        BookingStatus.Pending => "bg-amber-100 text-amber-800",
                        BookingStatus.Cancelled => "bg-red-100 text-red-800",
                        _ => "bg-gray-100 text-gray-800"
                    }
                }).ToList();

                var totalRevenue = bookings
                    .Where(b => b.Status != BookingStatus.Cancelled)
                    .Sum(b => b.TotalPrice)
                    .ToString("N2");

                return Json(new
                {
                    success = true,
                    type = "bookings",
                    monthName,
                    year,
                    month,
                    count = items.Count,
                    totalRevenue,
                    items
                });
            }
        }
    }
}
