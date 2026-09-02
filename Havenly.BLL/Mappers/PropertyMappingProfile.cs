using AutoMapper;
using Havenly.BLL.ModelVMs;
using Havenly.DAL.Entities;

namespace Havenly.BLL.Mappers
{
    public class PropertyMappingProfile : Profile
    {
        public PropertyMappingProfile()
        {
            // Entity -> PropertyDetailsVM
            CreateMap<Property, PropertyDetailsVM>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PropertyID.ToString()))
                .ForMember(dest => dest.ListingID, opt => opt.MapFrom(src => src.Listing != null ? src.Listing.ListingID : src.PropertyID))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.PropertyName))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address != null ? src.Address.City : "Unknown"))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Address != null ? src.Address.Country : "Unknown"))
                .ForMember(dest => dest.Neighbourhood, opt => opt.MapFrom(src => src.Address != null ? (src.Address.Street ?? src.Address.City) : "City Centre"))

                // Financial & Pricing details from Listing
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Listing!= null ? src.Listing.Price : 0m))
                
              
                //.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Listing != null ? src.Listing. : "General"))

                // Counts & Capacities
                .ForMember(dest => dest.Guests, opt => opt.MapFrom(src => src.NumberOfGuests))
                .ForMember(dest => dest.Bedrooms, opt => opt.MapFrom(src => src.Bedrooms != null && src.Bedrooms.Any() ? src.Bedrooms.Count : src.Capacity))
                .ForMember(dest => dest.Beds, opt => opt.MapFrom(src => src.Capacity))
                .ForMember(dest => dest.Baths, opt => opt.MapFrom(src => src.BathroomCount))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? "No description available."))

                // Collections & Navigation Properties
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images != null && src.Images.Any()
                    ? src.Images.Select(img => img.ImagePath).ToList()
                    : new List<string> { "/images/hero.jpg", "/images/p2.jpg", "/images/p3.jpg" }))

                .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => src.PropertyAmenities != null && src.PropertyAmenities.Any()
                    ? src.PropertyAmenities.Select(pa => pa.Amenity != null ? pa.Amenity.Name : "Amenity").ToList()
                    : new List<string> { "Wi-Fi", "Kitchen", "Air conditioning", "Parking", "TV" }))

                // Owner / Host Details
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerUserID ?? (src.Owner != null ? src.Owner.Id : string.Empty)))
                .ForMember(dest => dest.Host, opt => opt.MapFrom(src => new HostVM
                {
                    HostId = src.Owner != null ? src.Owner.Id : (src.OwnerUserID ?? string.Empty),
                    Name = src.Owner != null ? src.Owner.Name : "Havenly Host",
                    AvatarUrl = src.Owner != null ? (src.Owner.ProfilePictureUrl ?? string.Empty) : string.Empty,
                    Bio = src.Owner != null ? (src.Owner.Bio ?? string.Empty) : string.Empty,
                    Since = src.Owner != null ? src.Owner.JoinedDate.ToString("MMMM yyyy") : "2026",
                    Superhost = true,
                    ResponseRate = 98
                }))

                // Fallbacks for optional sections
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => (int)src.NumberOfReviews))
                .ForMember(dest => dest.Rules, opt => opt.MapFrom(src => new List<string> { "Check-in after 3:00 PM", "Check-out before 11:00 AM", "No smoking indoors" }))
                .ForMember(dest => dest.BookedDates, opt => opt.MapFrom(src => new List<string>()))
                .ForMember(dest => dest.PropertyReviews, opt => opt.MapFrom(src => src.Reviews != null
                    ? src.Reviews.Select(r => new ReviewItemVM
                    {
                        Id = r.ReviewID.ToString(),
                        PropertyId = r.PropertyID.ToString(),
                        Author = r.User != null ? r.User.Name : "Guest",
                        Rating = r.Rating,
                        Body = r.Comment,
                        HostResponse = r.HostResponse
                    }).ToList()
                    : new List<ReviewItemVM>()))
                .ForMember(dest => dest.Similar, opt => opt.MapFrom(src => new List<PropertyCardVM>()))
                .ForMember(dest => dest.Booking, opt => opt.MapFrom(src => new BookingRequestFormVM
                {
                    ListingID = src.Listing != null ? src.Listing.ListingID : src.PropertyID,
                    PricePerNight = src.Listing != null ? src.Listing.Price: 0m,
                    CheckOut = DateTime.Today.AddDays(3)
                }));

            // Entity -> PropertyCardVM
            CreateMap<Property, PropertyCardVM>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PropertyID.ToString()))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.PropertyName))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address != null ? src.Address.City : "Unknown"))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Address != null ? src.Address.Country : "Unknown"))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Listing != null ? src.Listing.Price: 0m))
                .ForMember(dest => dest.MaxGuests, opt => opt.MapFrom(src => src.NumberOfGuests))
                .ForMember(dest => dest.Bedrooms, opt => opt.MapFrom(src => src.Bedrooms != null && src.Bedrooms.Any() ? src.Bedrooms.Count : src.Capacity))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Images != null && src.Images.Any() ? src.Images.First().ImagePath : "/images/p1.jpg"))

                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => (int)src.NumberOfReviews))
                .ForMember(dest => dest.IsFavorite, opt => opt.MapFrom(src => false));
        }
    }
}