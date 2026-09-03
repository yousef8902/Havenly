using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Havenly.BLL.Services.Implementations
{
    public class PropertyServices : IPropertyServices
    {
        private readonly IPropertyRepository propertyRepository;
        private readonly IAddressRepository addressRepository;
        private readonly IListingRepository listingRepository;
        private readonly IPropertyImageRepository propertyImageRepository;
        private readonly IAmenityRepository amenityRepository;
        private readonly IPropertyAmenityRepository propertyAmenityRepository;
        private readonly IBedroomRepository bedroomRepository;
        private readonly IBedRepository bedRepository;
        private readonly IBookingRepository bookingRepository;

        public PropertyServices(
            IPropertyRepository propertyRepository,
            IAddressRepository addressRepository,
            IListingRepository listingRepository,
            IPropertyImageRepository propertyImageRepository,
            IAmenityRepository amenityRepository,
            IPropertyAmenityRepository propertyAmenityRepository,
            IBedroomRepository bedroomRepository,
            IBedRepository bedRepository,
            IBookingRepository bookingRepository)
        {
            this.propertyRepository = propertyRepository;
            this.addressRepository = addressRepository;
            this.listingRepository = listingRepository;
            this.propertyImageRepository = propertyImageRepository;
            this.amenityRepository = amenityRepository;
            this.propertyAmenityRepository = propertyAmenityRepository;
            this.bedroomRepository = bedroomRepository;
            this.bedRepository = bedRepository;
            this.bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<Property>> GetPropertiesByOwner(string ownerUserId)
        {
            if (string.IsNullOrWhiteSpace(ownerUserId))
            {
                return Enumerable.Empty<Property>();
            }

            return await propertyRepository.GetByOwner(ownerUserId);
        }

        public async Task<Property?> GetPropertyDetails(long propertyId, string ownerUserId)
        {
            var property = await propertyRepository.GetPropertyDetails(propertyId);

            // A host must never be able to read another host's management details.
            if (property is null || property.IsDeleted || property.OwnerUserID != ownerUserId)
            {
                return null;
            }

            return property;
        }

        public async Task<bool> CreateProperty(
            Property property,
            Address address,
            Listing listing,
            IEnumerable<PropertyAmenity> propertyAmenities,
            IEnumerable<Bedroom> bedrooms,
            IEnumerable<PropertyImage> images)
        {
            var amenities = propertyAmenities?.ToList() ?? [];
            var roomList = bedrooms?.ToList() ?? [];
            var imageList = images?.ToList() ?? [];

            if (!IsValidProperty(property) || !IsValidAddress(address) || !IsValidListing(listing) ||
                !AreValidBedrooms(roomList) || !AreValidImages(imageList) ||
                !await AreValidAmenities(amenities))
            {
                return false;
            }

            try
            {
                // Use transaction to ensure all-or-nothing semantics.
                // If any step fails, all changes are rolled back.
                // Address is saved first because Property uses AddressID as its foreign key.
                await addressRepository.Add(address);

                property.Create(
                    property.OwnerUserID,
                    address.AddressID,
                    property.PropertyName,
                    property.Description,
                    property.NumberOfGuests,
                    property.Capacity,
                    property.BathroomCount,
                    property.Category);
                await propertyRepository.Add(property);

                // Every newly submitted host listing starts pending for the admin workflow.
                listing.Create(property.PropertyID, listing.Description, listing.Price);
                await listingRepository.Add(listing);

                await AddAmenities(property.PropertyID, amenities);
                await AddBedroomsAndBeds(property.PropertyID, roomList);
                await AddImages(property.PropertyID, imageList);

                return property.PropertyID > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateProperty(
            long propertyId,
            string ownerUserId,
            Property property,
            Address address,
            Listing listing,
            IEnumerable<PropertyAmenity> propertyAmenities,
            IEnumerable<Bedroom> bedrooms)
        {
            var amenities = propertyAmenities?.ToList() ?? [];
            var roomList = bedrooms?.ToList() ?? [];

            if (!IsValidProperty(property) || !IsValidAddress(address) || !IsValidListing(listing) ||
                !AreValidBedrooms(roomList) || !await AreValidAmenities(amenities))
            {
                return false;
            }

            var existingProperty = await GetPropertyDetails(propertyId, ownerUserId);
            if (existingProperty is null || existingProperty.Address is null || existingProperty.Listing is null)
            {
                return false;
            }

            try
            {
                existingProperty.Update(
                    property.PropertyName,
                    property.Description,
                    property.NumberOfGuests,
                    property.Capacity,
                    property.BathroomCount,
                    property.Category);
                propertyRepository.Update(existingProperty);

                existingProperty.Address.Update(
                    address.Country,
                    address.City,
                    address.Street,
                    address.Latitude,
                    address.Longitude);
                addressRepository.Update(existingProperty.Address);

                existingProperty.Listing.Update(listing.Description, listing.Price);
                existingProperty.Listing.ListingStatus = ListingStatus.Pending;
                listingRepository.Update(existingProperty.Listing);

                await SynchronizeAmenities(existingProperty, amenities);
                await ReplaceBedroomsAndBeds(existingProperty, roomList);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProperty(long propertyId, string ownerUserId)
        {
            var property = await GetPropertyDetails(propertyId, ownerUserId);
            if (property is null)
            {
                return false;
            }

            // Unlist from search and explore so no new bookings can be made
            if (property.Listing != null)
            {
                property.Listing.IsValid = false;
                listingRepository.Update(property.Listing);
            }

            // Soft-delete the property (preserves existing booking records for guests)
            propertyRepository.Delete(property);
            return property.IsDeleted;
        }

        public async Task<bool> UpdateListingPrice(long propertyId, string ownerUserId, decimal newPrice)
        {
            if (newPrice <= 0 || string.IsNullOrWhiteSpace(ownerUserId))
            {
                return false;
            }

            var property = await GetPropertyDetails(propertyId, ownerUserId);
            if (property?.Listing is null)
            {
                return false;
            }

            property.Listing.Price = newPrice;
            listingRepository.Update(property.Listing);
            return true;
        }

        public async Task<(bool Success, bool IsActive, string Message)> ToggleListingSuspension(long propertyId, string ownerUserId)
        {
            if (string.IsNullOrWhiteSpace(ownerUserId))
            {
                return (false, false, "Unauthorized");
            }

            var property = await GetPropertyDetails(propertyId, ownerUserId);
            if (property?.Listing is null)
            {
                return (false, false, "Listing not found");
            }

            // Toggle IsValid
            property.Listing.IsValid = !property.Listing.IsValid;
            listingRepository.Update(property.Listing);

            bool isActive = property.Listing.IsValid;
            string msg = isActive 
                ? "Listing resumed successfully! It is now active and bookable by guests."
                : "Listing paused successfully! It is temporarily unlisted from public search.";

            return (true, isActive, msg);
        }

        public async Task<int> GetUpcomingBookingsCount(long propertyId, string ownerUserId)
        {
            var property = await GetPropertyDetails(propertyId, ownerUserId);
            if (property?.Listing is null)
            {
                return 0;
            }

            var activeStatuses = new[] { BookingStatus.Pending, BookingStatus.Approved };
            var bookings = await bookingRepository.Find(b => 
                b.ListingID == property.Listing.ListingID && 
                activeStatuses.Contains(b.Status) && 
                b.CheckOut >= DateTime.UtcNow);

            return bookings.Count();
        }

        public async Task<bool> AddPropertyImages(long propertyId, string ownerUserId, IEnumerable<PropertyImage> images)
        {
            var imageList = images?.ToList() ?? [];
            if (!AreValidImages(imageList) || await GetPropertyDetails(propertyId, ownerUserId) is null)
            {
                return false;
            }

            try
            {
                await AddImages(propertyId, imageList);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemovePropertyImage(long propertyId, long imageId, string ownerUserId)
        {
            var property = await GetPropertyDetails(propertyId, ownerUserId);
            var image = property?.Images.FirstOrDefault(currentImage => currentImage.ImageID == imageId);
            if (image is null)
            {
                return false;
            }

            propertyImageRepository.Delete(image);
            return true;
        }

        private async Task<bool> AreValidAmenities(IEnumerable<PropertyAmenity> amenities)
        {
            var amenityIds = amenities.Select(propertyAmenity => propertyAmenity.AmenitiesID).ToList();
            if (amenityIds.Count != amenityIds.Distinct().Count() || amenityIds.Any(id => id <= 0))
            {
                return false;
            }

            foreach (var amenityId in amenityIds)
            {
                if (await amenityRepository.GetById(amenityId) is null)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsValidProperty(Property property)
        {
            return property is not null &&
                   !string.IsNullOrWhiteSpace(property.OwnerUserID) &&
                   !string.IsNullOrWhiteSpace(property.PropertyName) &&
                   !string.IsNullOrWhiteSpace(property.Description) &&
                   property.NumberOfGuests > 0 &&
                   property.Capacity > 0 &&
                   property.BathroomCount > 0;
        }

        private static bool IsValidAddress(Address address)
        {
            return address is not null &&
                   !string.IsNullOrWhiteSpace(address.Country) &&
                   !string.IsNullOrWhiteSpace(address.City) &&
                   !string.IsNullOrWhiteSpace(address.Street);
        }

        private static bool IsValidListing(Listing listing)
        {
            return listing is not null &&
                   !string.IsNullOrWhiteSpace(listing.Description) &&
                   listing.Price > 0;
        }

        private static bool AreValidBedrooms(IEnumerable<Bedroom> bedrooms)
        {
            foreach (var bedroom in bedrooms)
            {
                if (bedroom.RoomNumber <= 0 || bedroom.Beds is null)
                {
                    return false;
                }

                if (bedroom.Beds.Any(bed => bed.Quantity <= 0))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool AreValidImages(IEnumerable<PropertyImage> images)
        {
            var paths = images.Select(image => image.ImagePath).ToList();
            return paths.All(path => !string.IsNullOrWhiteSpace(path)) &&
                   paths.Count == paths.Distinct(StringComparer.OrdinalIgnoreCase).Count();
        }

        private async Task AddAmenities(long propertyId, IEnumerable<PropertyAmenity> amenities)
        {
            foreach (var selectedAmenity in amenities)
            {
                var propertyAmenity = new PropertyAmenity();
                propertyAmenity.Create(propertyId, selectedAmenity.AmenitiesID);
                await propertyAmenityRepository.Add(propertyAmenity);
            }
        }

        private async Task SynchronizeAmenities(Property property, IEnumerable<PropertyAmenity> selectedAmenities)
        {
            var selectedAmenityIds = selectedAmenities
                .Select(propertyAmenity => propertyAmenity.AmenitiesID)
                .ToHashSet();
            var currentAmenities = property.PropertyAmenities.ToList();

            foreach (var currentAmenity in currentAmenities.Where(currentAmenity => !selectedAmenityIds.Contains(currentAmenity.AmenitiesID)))
            {
                propertyAmenityRepository.Delete(currentAmenity);
            }

            foreach (var amenityId in selectedAmenityIds.Where(amenityId => currentAmenities.All(currentAmenity => currentAmenity.AmenitiesID != amenityId)))
            {
                var propertyAmenity = new PropertyAmenity();
                propertyAmenity.Create(property.PropertyID, amenityId);
                await propertyAmenityRepository.Add(propertyAmenity);
            }
        }

        private async Task AddBedroomsAndBeds(long propertyId, IEnumerable<Bedroom> bedrooms)
        {
            foreach (var bedroom in bedrooms)
            {
                var beds = bedroom.Beds?.ToList() ?? new List<Bed>();
                var bedCount = bedroom.BedCount > 0 ? bedroom.BedCount : (beds.Any() ? beds.Sum(b => b.Quantity) : 1);
                bedroom.Create(0, propertyId, bedroom.RoomNumber, bedcnt: bedCount, roomName: bedroom.RoomName);
                await bedroomRepository.Add(bedroom);

                foreach (var bed in beds)
                {
                    bed.Create(0, bedroom.BedroomID, bed.BedType, bed.Quantity);
                    await bedRepository.Add(bed);
                }
            }
        }

        private async Task ReplaceBedroomsAndBeds(Property property, IEnumerable<Bedroom> bedrooms)
        {
            // The current data model has no stable form IDs for rooms, so replace the room collection as one unit.
            foreach (var existingBedroom in property.Bedrooms.ToList())
            {
                foreach (var existingBed in existingBedroom.Beds.ToList())
                {
                    bedRepository.Delete(existingBed);
                }

                bedroomRepository.Delete(existingBedroom);
            }

            await AddBedroomsAndBeds(property.PropertyID, bedrooms);
        }

        private async Task AddImages(long propertyId, IEnumerable<PropertyImage> images)
        {
            foreach (var image in images)
            {
                image.Create(0, propertyId, image.ImagePath);
                await propertyImageRepository.Add(image);
            }
        }
    }
}
