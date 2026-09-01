using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Havenly.PL.Controllers;

public class PropertyController : Controller
{
    private readonly IPropertyService _propertyService;
    private readonly IListingServices _listingService;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IFavoriteRepository _favoriteRepository;

    public PropertyController(
        IPropertyService propertyService,
        IListingServices listingService,
        IPropertyRepository propertyRepository,
        IFavoriteRepository favoriteRepository)
    {
        _propertyService = propertyService;
        _listingService = listingService;
        _propertyRepository = propertyRepository;
        _favoriteRepository = favoriteRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? city,
        string? category,
        string? checkIn,
        string? checkOut,
        int? guests,
        decimal? maxPrice,
        string? minRooms,
        string? minRating,
        List<string>? amenities,
        string? sort)
    {
        var allProperties = await _propertyRepository.GetAll();
        var query = allProperties.Where(p => p.Listing != null && p.Listing.ListingStatus == ListingStatus.Approved);

        // 1. City / Destination Filter
        if (!string.IsNullOrWhiteSpace(city))
        {
            var term = city.Trim().ToLowerInvariant();
            query = query.Where(p =>
                (p.Address != null && ((p.Address.City != null && p.Address.City.ToLowerInvariant().Contains(term)) ||
                                        (p.Address.Country != null && p.Address.Country.ToLowerInvariant().Contains(term)) ||
                                        (p.Address.Street != null && p.Address.Street.ToLowerInvariant().Contains(term)))) ||
                (p.PropertyName != null && p.PropertyName.ToLowerInvariant().Contains(term)));
        }

        // 2. Guests Filter
        if (guests.HasValue && guests.Value > 0)
        {
            query = query.Where(p => p.NumberOfGuests >= guests.Value);
        }

        // 3. Max Price Filter
        if (maxPrice.HasValue && maxPrice.Value > 0)
        {
            query = query.Where(p => p.Listing != null && p.Listing.Price <= maxPrice.Value);
        }

        // 4. Bedrooms (minRooms) Filter
        if (!string.IsNullOrWhiteSpace(minRooms) && minRooms != "any" && int.TryParse(minRooms, out int requiredRooms))
        {
            query = query.Where(p => (p.Bedrooms != null && p.Bedrooms.Count >= requiredRooms) || p.Capacity >= requiredRooms);
        }

        // 5. Rating (minRating) Filter (3.0 to 5.0)
        if (!string.IsNullOrWhiteSpace(minRating) && minRating != "any" && double.TryParse(minRating, NumberStyles.Any, CultureInfo.InvariantCulture, out double reqRating))
        {
            if (reqRating > 3.01)
            {
                query = query.Where(p => p.Rating >= reqRating);
            }
        }

        // 6. Category Filter
        if (!string.IsNullOrWhiteSpace(category) && category != "Any category" && category != "all")
        {
            var catTerm = category.Trim().ToLowerInvariant();
            query = query.Where(p =>
                (p.Category != null && p.Category.ToLowerInvariant().Contains(catTerm)) ||
                (p.PropertyName != null && p.PropertyName.ToLowerInvariant().Contains(catTerm)) ||
                (p.Description != null && p.Description.ToLowerInvariant().Contains(catTerm)) ||
                (p.Address != null && p.Address.City != null && p.Address.City.ToLowerInvariant().Contains(catTerm)));
        }

        // 7. Amenities Filter
        if (amenities != null && amenities.Any())
        {
            var activeAmenities = amenities.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => a.Trim().ToLowerInvariant()).ToList();
            foreach (var req in activeAmenities)
            {
                query = query.Where(p => p.PropertyAmenities != null &&
                    p.PropertyAmenities.Any(pa => pa.Amenity != null && pa.Amenity.Name != null && pa.Amenity.Name.Trim().ToLowerInvariant() == req));
            }
        }

        // 8. Sorting
        query = sort switch
        {
            "price-asc" => query.OrderBy(p => p.Listing!.Price),
            "price-desc" => query.OrderByDescending(p => p.Listing!.Price),
            "rating" => query.OrderByDescending(p => p.Rating).ThenBy(p => p.Listing!.Price),
            _ => query.OrderByDescending(p => p.Rating).ThenBy(p => p.Listing!.Price)
        };

        var propertyList = query.ToList();

        // Check user favorites
        var userFavoriteIds = new HashSet<long>();
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var favs = await _favoriteRepository.Find(f => f.UserID == userId);
                userFavoriteIds = favs.Select(f => f.ListingID).ToHashSet();
            }
        }

        var properties = propertyList.Select(p => new PropertyCardVM
        {
            Id = p.Listing?.ListingID ?? p.PropertyID,
            Title = p.PropertyName,
            City = p.Address?.City ?? "Unknown City",
            Country = p.Address?.Country ?? "Unknown Country",
            Price = p.Listing?.Price ?? 0,
            Rating = p.Rating > 0 ? p.Rating : 4.9,
            MaxGuests = p.NumberOfGuests,
            Bedrooms = p.Bedrooms != null && p.Bedrooms.Any() ? p.Bedrooms.Count : (p.Capacity > 0 ? p.Capacity : 1),
            ImageUrl = p.Images?.FirstOrDefault(i => i.IsPrimary == true)?.ImagePath
                       ?? p.Images?.FirstOrDefault()?.ImagePath
                       ?? "/images/placeholder.jpg",
            Category = p.Category ?? "Design homes",
            IsFavorite = p.Listing != null && userFavoriteIds.Contains(p.Listing.ListingID)
        }).ToList();

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

        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var isFav = (await _favoriteRepository.Find(f => f.UserID == userId && f.ListingID == propertyVm.ListingID)).Any();
                propertyVm.IsFavorite = isFav;
            }
        }

        return View(propertyVm);
    }
}
