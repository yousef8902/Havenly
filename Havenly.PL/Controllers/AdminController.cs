using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.BLL.Services.Implementations;
using Havenly.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Havenly.PL.Controllers
{


    // TODO: SECURITY — no authentication/authorization system exists in the app yet.
    // This controller and all its actions are currently publicly accessible.
    // Must add [Authorize(Roles = "Admin")] once login/cookie auth is implemented.
    // See: Havenly.DAL.Entities.User.Role (custom field, not ASP.NET Identity).
    public class AdminController : Controller
    {

        private readonly IListingServices listingService;
        private readonly IReportService reportService;
        private readonly IAdminService _adminService;



        public AdminController(IListingServices listingService, IReportService reportService, IAdminService adminService)
        {
            this.listingService = listingService;
            this.reportService = reportService;
            this._adminService = adminService;
        }
        public async Task<IActionResult> DashBoard() 

            
        {
            var vm = new AdminDashboardVM
            {
                Stats = await reportService.GetPlatformStats(),
                PendingListings = await listingService.GetPendingListings(),
                MembersResult = await _adminService.GetMembersDataAsync(null, null, 1, 5)//mariam

            };
            return View("Index", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(long id)
        {
            var success = await listingService.ApproveListing(id);
            TempData["Message"] = success ? "Listing approved." : "Listing not found or could not be approved.";
            return RedirectToAction(nameof(DashBoard));
        }

        [HttpPost]
        public async Task<IActionResult> Decline(long id)
        {
            var success = await listingService.DeclineListing(id);
            TempData["Message"] = success ? "Listing declined." : "Listing not found or could not be declined.";
            return RedirectToAction(nameof(DashBoard));
        }

    public async Task<IActionResult> Members( UserStatus? status = null, string search = "", int page = 1)//mariam
        {
            var result = await _adminService.GetMembersDataAsync( status, search, page, 20);

            var model = new AdminMembersVM
            {
                Members = result.Members,
                Pagination = result.Pagination,
                Filters = new MemberFilters
                {
                    //Role = role,
                    Status = status?.ToString().ToLower() ?? "all",
                    SearchTerm = search
                }
            };

            return View(model);
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
            // Convert string status to enum
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
    } }
