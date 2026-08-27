using AutoMapper;
using Havenly.BLL.ModelVMs;
using Havenly.DAL.Entities;

namespace Havenly.BLL.Mappers
{
    public class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            CreateMap<Booking, BookingDetailsVM>()
                // Basic ID mappings
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => (int)src.BookingID))
                .ForMember(dest => dest.PropertyId, opt => opt.MapFrom(src =>
                    src.Listing != null ? src.Listing.PropertyID.ToString() : "0"))

                // Property details
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src =>
                    src.Listing != null && src.Listing.Property != null
                        ? src.Listing.Property.PropertyName
                        : "Unknown Property"))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src =>
                    src.Listing != null && src.Listing.Property != null && src.Listing.Property.Address != null
                        ? src.Listing.Property.Address.City
                        : "Unknown City"))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src =>
                    src.Listing != null && src.Listing.Property != null && src.Listing.Property.Address != null
                        ? src.Listing.Property.Address.Country
                        : "Unknown Country"))

                // Guest information
                .ForMember(dest => dest.Guest, opt => opt.MapFrom(src =>
                    src.Guest != null
                        ? src.Guest.Name
                        : "Unknown Guest"))
                .ForMember(dest => dest.GuestEmail, opt => opt.MapFrom(src =>
                    src.Guest != null ? src.Guest.Email : ""))

                // Booking details
                .ForMember(dest => dest.CheckIn, opt => opt.MapFrom(src => src.CheckIn))
                .ForMember(dest => dest.CheckOut, opt => opt.MapFrom(src => src.CheckOut))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.Guests, opt => opt.MapFrom(src =>
                    src.Listing != null && src.Listing.Property != null
                        ? src.Listing.Property.NumberOfGuests
                        : 1))

                // Status mapping (convert enum to string)
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                    src.Status.ToString()))

                // Image URL
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src =>
                    src.Listing != null && src.Listing.Property != null && src.Listing.Property.Images != null && src.Listing.Property.Images.Any()
                        ? src.Listing.Property.Images.First().ImagePath
                        : "/images/default-property.jpg"));
        }
    }
}