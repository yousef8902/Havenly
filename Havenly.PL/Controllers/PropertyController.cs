using Havenly.BLL.ModelVMs;
using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Havenly.PL.Data;

using Microsoft.AspNetCore.Mvc;

namespace Havenly.PL.Controllers;

public class PropertyController : Controller
   
{
    private readonly IPropertyService _propertyService;

    // Inject your real database/BLL property service
    public PropertyController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [HttpGet]
    public async Task<IActionResult> Details(long id)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        // Fetch real PropertyDetailsVM populated from your database entity
        var propertyVm = await _propertyService.GetPropertyDetailsByIdAsync(id);

        if (propertyVm == null)
        {
            return NotFound();
        }

        // Ensure default collections/nested objects are non-null to prevent Razor errors
        propertyVm.Images ??= new List<string>();
        propertyVm.Amenities ??= new List<string>();
        propertyVm.Rules ??= new List<string>();
        propertyVm.BookedDates ??= new List<string>();
        propertyVm.PropertyReviews ??= new List<ReviewVM>();
        propertyVm.Similar ??= new List<PropertyCardVM>();

        propertyVm.Host ??= new HostVM
        {
            Name = "Host",
            Since = "2024",
            Superhost = false,
            ResponseRate = 100
        };

        // Initialize default booking dates for the form if not populated by service
        propertyVm.Booking ??= new BookingRequestVM
        {
            ListingID = propertyVm.ListingID,
            PricePerNight = propertyVm.Price,
            CheckIn = DateTime.Today.AddDays(1),
            CheckOut = DateTime.Today.AddDays(3)
        };

        return View(propertyVm);
    }
}
}

