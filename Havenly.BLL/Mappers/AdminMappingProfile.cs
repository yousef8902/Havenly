using AutoMapper;
using Havenly.BLL.ModelVMs;
using Havenly.DAL.Entities;

namespace Havenly.BLL.Mappers
{
    public class AdminMappingProfile : Profile
    {
        public AdminMappingProfile()
        {
            CreateMap<Listing, PendingListing>()
                .ForMember(dest => dest.ListingId, opt => opt.MapFrom(src => src.ListingID))
                .ForMember(dest => dest.PropertyName, opt => opt.MapFrom(src => src.Property.PropertyName))
                .ForMember(dest => dest.HostName, opt => opt.MapFrom(src => src.Property.Owner.Name))
                .ForMember(dest => dest.HostId, opt => opt.MapFrom(src => src.Property.OwnerUserID.ToString()))
                
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src =>
                    src.Property.Address != null
                        ? $"{src.Property.Address.City}, {src.Property.Address.Country}"
                        : "Location not specified"))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Property.Images))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));
        }
    }
}