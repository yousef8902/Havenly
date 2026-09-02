using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class BookingController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly UserManager<User> _userManager;
    private readonly IEmailServices _emailServices;
    private readonly IListingRepository _listingRepository;
    private readonly IPropertyRepository _propertyRepository;

    public BookingController(
        IBookingService bookingService,
        UserManager<User> userManager,
        IEmailServices emailServices,
        IListingRepository listingRepository,
        IPropertyRepository propertyRepository)
    {
        _bookingService = bookingService;
        _userManager = userManager;
        _emailServices = emailServices;
        _listingRepository = listingRepository;
        _propertyRepository = propertyRepository;
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
            return View(model);
        }

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

        // Send Email Notification to Host
        try
        {
            var listing = await _listingRepository.GetById(model.ListingID);
            var property = listing?.Property ?? (listing != null ? await _propertyRepository.GetById(listing.PropertyID) : null);
            var host = property?.Owner ?? (property != null ? await _userManager.FindByIdAsync(property.OwnerUserID) : null);
            var guest = await _userManager.GetUserAsync(User);

            if (host != null && !string.IsNullOrEmpty(host.Email))
            {
                int totalNights = Math.Max(1, (model.CheckOut.Date - model.CheckIn.Date).Days);
                decimal subtotal = totalNights * model.PricePerNight;
                decimal totalPrice = subtotal + Math.Round(subtotal * 0.09m, 2);

                await _emailServices.SendBookingRequestToHostAsync(
                    host.Email,
                    host.Name ?? "Host",
                    guest?.Name ?? "Guest",
                    property?.PropertyName ?? model.PropertyName ?? "Property",
                    model.CheckIn,
                    model.CheckOut,
                    totalPrice,
                    totalNights,
                    result.BookingID ?? 0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EMAIL NOTICE ERROR] {ex.Message}");
        }

        if (result.BookingID.HasValue && result.BookingID.Value > 0)
        {
            return RedirectToAction("Checkout", "Payment", new { bookingId = result.BookingID.Value });
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(MyBookings));
    }

    [HttpGet]
    public async Task<IActionResult> MyBookings()
    {
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