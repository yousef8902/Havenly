using System.Linq;
using System.Threading.Tasks;
using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Havenly.PL.Controllers;

public class HomeController : Controller
{
    private readonly IListingServices _listingService;

    public HomeController(IListingServices listingService)
    {
        _listingService = listingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Havenly — Find a place that feels like home";
        ViewData["Description"] = "Havenly is a short-term rental marketplace for considered homes.";
        ViewData["TransparentHeader"] = true;

        var listings = (await _listingService.GetListingsAsync(ListingStatus.Approved)).ToList();
        if (!listings.Any())
        {
            listings = (await _listingService.GetListingsAsync(ListingStatus.Pending)).ToList();
        }

        var featuredCards = listings
            .Take(4)
            .Select(l => new PropertyCardVM
            {
                Id = l.ListingID,
                Title = l.Property?.PropertyName ?? "Havenly Stay",
                City = l.Property?.Address?.City ?? "Cairo",
                Country = l.Property?.Address?.Country ?? "Egypt",
                Price = l.Price,
                Rating = l.Property != null && l.Property.Rating > 0 ? l.Property.Rating : 4.9,
                ImageUrl = l.Property?.Images?.FirstOrDefault(i => i.IsPrimary == true)?.ImagePath
                           ?? l.Property?.Images?.FirstOrDefault()?.ImagePath
                           ?? "/images/p1.jpg"
            })
            .ToList();

        var recommendedCards = listings
            .Skip(2)
            .Take(2)
            .Select(l => new PropertyCardVM
            {
                Id = l.ListingID,
                Title = l.Property?.PropertyName ?? "Havenly Stay",
                City = l.Property?.Address?.City ?? "Alexandria",
                Country = l.Property?.Address?.Country ?? "Egypt",
                Price = l.Price,
                Rating = l.Property != null && l.Property.Rating > 0 ? l.Property.Rating : 4.8,
                ImageUrl = l.Property?.Images?.FirstOrDefault(i => i.IsPrimary == true)?.ImagePath
                           ?? l.Property?.Images?.FirstOrDefault()?.ImagePath
                           ?? "/images/p3.jpg"
            })
            .ToList();

        var viewModel = new HomeIndexVM
        {
            Search = new SearchFilterVM { Guests = 2 },
            Featured = featuredCards,
            Recommended = recommendedCards
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Error()
    {
        ViewData["Title"] = "This page didn't load";
        return View();
    }

    [HttpGet]
    public IActionResult NotFoundPage()
    {
        Response.StatusCode = 404;
        ViewData["Title"] = "Page not found";
        return View("NotFound");
    }
}
