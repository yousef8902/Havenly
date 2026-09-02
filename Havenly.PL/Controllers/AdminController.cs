using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Identity;

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
            UserManager<User> userManager)
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
        }

        public async Task<IActionResult> DashBoard()
        {
            var vm = new AdminDashboardVM
            {
                Stats = await _reportService.GetPlatformStats(),
                PendingListings = await _listingService.GetPendingListings(),
                MembersResult = await _adminService.GetMembersDataAsync(null, null, 1, 5)
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

        public async Task<IActionResult> BookingDetails(long id)
        {
            var booking = await _bookingRepository.GetById(id);
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

            return RedirectToAction(nameof(PaymentHistory));
        }
    }
}
