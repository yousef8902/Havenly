using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Havenly.PL.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class AdminController : Controller
    {

        private readonly IListingServices listingService;
        private readonly IReportService reportService;


        public AdminController(IListingServices listingService, IReportService reportService)
        {
            this.listingService = listingService;
            this.reportService = reportService;
        }
        public async Task<IActionResult> DashBoard()
        {
            var vm = new AdminDashboardVM
            {
                Stats = await reportService.GetPlatformStats(),
                PendingListings = await listingService.GetPendingListings()
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
    }
}
