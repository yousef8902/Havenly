// TODO [Identity Integration]: Add [Authorize] to restrict access to logged-in users only
// [Authorize]
using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class BookingController : Controller
{
    private readonly IBookingService _bookingService;

    // TODO [Identity Integration]: Declare UserManager field to query logged-in user details
     private readonly UserManager<User> _userManager;


    // TODO [Identity Integration]: Update constructor to inject UserManager<ApplicationUser>
    
    public BookingController(IBookingService bookingService, UserManager<User> userManager)
    {
        _bookingService = bookingService;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Create(long listingId, string propertyName, decimal pricePerNight, int maxGuests)
    {

        var viewModel = new BookingRequestVM
        {
            ListingID = listingId,
            PropertyName = propertyName ?? "Property",
            PricePerNight = pricePerNight,
            CheckIn = DateTime.Today.AddDays(1),
            CheckOut = DateTime.Today.AddDays(3)
        };


        ViewBag.MaxGuests = maxGuests > 0 ? maxGuests : 5;

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingRequestVM model)
    {
        if (!ModelState.IsValid)
        {
            return View( model);
        }

        // TODO [Identity Integration]: Retrieve the real logged-in user's ID dynamically
        string resolvedUserId = _userManager.GetUserId(User);
      


        var createVm = new BookingCreateVM
        {
            ListingID = model.ListingID,
            CheckIn = model.CheckIn,
            CheckOut = model.CheckOut,
            PricePerNight = model.PricePerNight,
            GuestUserID = resolvedUserId
        };


        BookingResultVM result = await _bookingService.CreateBookingAsync(createVm);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(MyBookings));
    }

    [HttpGet]
    public async Task<IActionResult> MyBookings()
    {
        // TODO [Identity Integration]: Fetch bookings for the currently authenticated user instead of hardcoded '1'
         string resolvedUserId = _userManager.GetUserId(User);
      
         var bookings = await _bookingService.GetBookingsByUserAsync(resolvedUserId);

   
        var viewModel = new BookingsPageVM
        {
            Bookings = bookings.ToList() 
        };

        return View(viewModel);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelBooking(long id)
    {
        string resolvedUserId = _userManager.GetUserId(User);
        bool cancelled = await _bookingService.CancelBookingAsync(id, resolvedUserId);

        if (cancelled)
        {
            TempData["SuccessMessage"] = $"Booking #{id} has been cancelled.";
        }
        else
        {
            TempData["ErrorMessage"] = "Unable to cancel this booking.";
        }

        return RedirectToAction(nameof(MyBookings));
    }
}