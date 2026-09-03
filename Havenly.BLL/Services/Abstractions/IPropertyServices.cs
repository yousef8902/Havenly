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

        Task<bool> UpdateListingPrice(long propertyId, string ownerUserId, decimal newPrice);
        Task<(bool Success, bool IsActive, string Message)> ToggleListingSuspension(long propertyId, string ownerUserId);
        Task<int> GetUpcomingBookingsCount(long propertyId, string ownerUserId);
    }
}
