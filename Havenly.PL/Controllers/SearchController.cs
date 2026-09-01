using AutoMapper;
using Havenly.BLL.ModelVMs;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Havenly.PL.Controllers
{
    public class SearchController : Controller
    {
        private readonly IPropertyRepository _propertyRepo;
        private readonly IListingRepository _listingRepo;
        private readonly IMapper _mapper;

        public SearchController(
            IPropertyRepository propertyRepo,
            IListingRepository listingRepo,
            IMapper mapper)
        {
            _propertyRepo = propertyRepo;
            _listingRepo = listingRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(SearchFilterVM filters)
        {
            filters ??= new SearchFilterVM();

            var listings = (await _listingRepo.Find(l => l.ListingStatus == ListingStatus.Approved && l.IsValid == true)).ToList();
            var matchedCards = new List<PropertyCardVM>();

            foreach (var listing in listings)
            {
                var property = await _propertyRepo.GetDetailbyId(listing.PropertyID);
                if (property == null || property.IsDeleted) continue;

                // Filter by City
                if (!string.IsNullOrWhiteSpace(filters.City))
                {
                    var searchCity = filters.City.Trim().ToLower();
                    var propCity = property.Address?.City?.ToLower() ?? "";
                    var propCountry = property.Address?.Country?.ToLower() ?? "";
                    var propTitle = property.PropertyName?.ToLower() ?? "";

                    if (!propCity.Contains(searchCity) && !propCountry.Contains(searchCity) && !propTitle.Contains(searchCity))
                        continue;
                }

                // Filter by Guests
                if (filters.Guests > 0 && property.NumberOfGuests < filters.Guests)
                    continue;

                // Filter by MaxPrice
                if (filters.MaxPrice > 0 && listing.Price > filters.MaxPrice)
                    continue;

                // Filter by MinRooms
                if (int.TryParse(filters.MinRooms, out int minRooms) && minRooms > 0)
                {
                    if (property.Bedrooms.Count < minRooms)
                        continue;
                }

                // Filter by MinRating
                if (double.TryParse(filters.MinRating, out double minRating) && minRating > 0)
                {
                    if (property.Rating < minRating)
                        continue;
                }

                var card = _mapper.Map<PropertyCardVM>(property);
                if (card != null)
                {
                    card.Id = listing.ListingID;
                    card.Price = listing.Price;
                    matchedCards.Add(card);
                }
            }

            int pageSize = 6;
            int page = filters.Page > 0 ? filters.Page : 1;
            int totalCount = matchedCards.Count;
            int pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

            var pagedResults = matchedCards
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new SearchPageVM
            {
                Filters = filters,
                Results = pagedResults,
                TotalCount = totalCount,
                PageSize = pageSize,
                PageCount = Math.Max(pageCount, 1)
            };

            return View(viewModel);
        }
    }
}
