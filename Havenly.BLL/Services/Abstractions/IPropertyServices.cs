using Havenly.DAL.Entities;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IPropertyServices
    {
        Task<IEnumerable<Property>> GetPropertiesByOwner(string ownerUserId);
        Task<Property?> GetPropertyDetails(long propertyId, string ownerUserId);

        Task<bool> CreateProperty(
            Property property,
            Address address,
            Listing listing,
            IEnumerable<PropertyAmenity> propertyAmenities,
            IEnumerable<Bedroom> bedrooms,
            IEnumerable<PropertyImage> images);

        Task<bool> UpdateProperty(
            long propertyId,
            string ownerUserId,
            Property property,
            Address address,
            Listing listing,
            IEnumerable<PropertyAmenity> propertyAmenities,
            IEnumerable<Bedroom> bedrooms);

        Task<bool> DeleteProperty(long propertyId, string ownerUserId);
        Task<bool> AddPropertyImages(long propertyId, string ownerUserId, IEnumerable<PropertyImage> images);
        Task<bool> RemovePropertyImage(long propertyId, long imageId, string ownerUserId);
    }
}
