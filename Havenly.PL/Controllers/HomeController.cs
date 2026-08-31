using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Enums;
using Havenly.PL.Data;

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

        
        var approvedListings = await _listingService.GetListingsAsync(ListingStatus.Approved);

        
        var featuredCards = approvedListings
            .Take(4)
            .Select(l => new PropertyCardVM
            {
                Id = l.ListingID,
                Title = l.Property?.PropertyName ?? "Title",
                City = l.Property?.Address?.City ?? "Unknown City",
                Country = l.Property?.Address?.Country ?? "Unknown Country",
                Price = l.Price,
                //Rating = l.AverageRating > 0 ? l.AverageRating : 4.9,
                ImageUrl = l.Property?.Images?.FirstOrDefault()?.ImagePath ?? "/images/p7.jpg",
               
            })
            .ToList();

        var recommendedCards = approvedListings
            .Skip(1)
            .Take(2)
            .Select(l => new PropertyCardVM
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

