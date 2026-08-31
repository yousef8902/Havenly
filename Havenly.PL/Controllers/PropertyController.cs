using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.PL.Data;

using Microsoft.AspNetCore.Mvc;

namespace Havenly.PL.Controllers;

public class PropertyController : Controller
{
    private readonly IPropertyService _propertyService;
    private readonly IListingServices
        _listingService;

    public PropertyController(IPropertyService propertyService, IListingServices listingService)
    {
        _propertyService = propertyService;
        _listingService = listingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? city, string? category, int guests = 2)
    {
        var prop = await _listingService.GetListingsAsync(ListingStatus.Approved);

        var properties = prop.Select(l => new PropertyCardVM
            {
                Id = l.ListingID,
                Title = l.Property?.PropertyName ?? "Title",
                City = l.Property?.Address?.City ?? "Unknown City",
                Country = l.Property?.Address?.Country ?? "Unknown Country",
                Price = l.Price,
                //Rating = l.AverageRating > 0 ? l.AverageRating : 4.8,
                ImageUrl = l.Property?.Images?.FirstOrDefault()?.ImagePath ?? "/images/placeholder.jpg",

            })
            .ToList();
        return View(properties);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(long id)
    {
        if (id == 0)
        {
            return NotFound();
        }

        var propertyVm = await _propertyService.GetPropertyDetailsByIdAsync(id);

        if (propertyVm == null)
        {
            return NotFound();
        }


      

        return View(propertyVm);
    }
}


