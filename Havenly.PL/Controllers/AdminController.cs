using Microsoft.AspNetCore.Mvc;
using Havenly.BLL.Services.Abstractions;
using Havenly.BLL.ModelVMs;

namespace Havenly.PL.Controllers
{
    // TODO: SECURITY — no authentication/authorization system exists in the app yet.
    public class AdminController : Controller
    {
        private readonly IListingServices listingService;
        private readonly IReportService reportService;
        private readonly IUserManagementService userManagementService;

        public AdminController(
            IListingServices listingService,
            IReportService reportService,
            IUserManagementService userManagementService)
        {
            this.listingService = listingService;
            this.reportService = reportService;
            this.userManagementService = userManagementService;
        }

        public async Task<IActionResult> DashBoard()
        {
            var vm = new AdminDashboardVM
            {
                Stats = await reportService.GetPlatformStats(),
                PendingListings = await listingService.GetPendingListings(),
                Members = await userManagementService.GetAllMembers()
            };
            return View(vm);
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

        [HttpPost]
        public async Task<IActionResult> SuspendUser(string userId)
        {
            var success = await userManagementService.SuspendUser(userId);
            TempData["Message"] = success ? "Member suspended." : "Could not suspend member.";
            return RedirectToAction(nameof(DashBoard));
        }

        [HttpPost]
        public async Task<IActionResult> ReinstateUser(string userId)
        {
            var success = await userManagementService.ReinstateUser(userId);
            TempData["Message"] = success ? "Member reinstated." : "Could not reinstate member.";
            return RedirectToAction(nameof(DashBoard));
        }
    }
}