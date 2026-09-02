//using System;
//using System.Threading.Tasks;
//using Havenly.BLL.Services.Implementations;
//using Havenly.DAL.Database;
//using Havenly.DAL.Entities;
//using Havenly.DAL.Enums;
//using Havenly.DAL.Repos.Implementations;
//using Microsoft.EntityFrameworkCore;
//using System.Linq;
//using Xunit;

//namespace Havenly.BLL.Tests
//{
//    public class PropertyServicesTests
//    {
//        private HavenlyDbContext CreateContext(string databaseName)
//        {
//            var options = new DbContextOptionsBuilder<HavenlyDbContext>()
//                .UseInMemoryDatabase(databaseName)
//                .Options;

//            return new HavenlyDbContext(options);
//        }

//        private static PropertyServices CreateService(HavenlyDbContext context)
//        {
//            return new PropertyServices(
//                new PropertyRepository(context),
//                new AddressRepository(context),
//                new ListingRepository(context),
//                new PropertyImageRepository(context),
//                new AmenityRepository(context),
//                new PropertyAmenityRepository(context),
//                new BedroomRepository(context),
//                new BedRepository(context));
//        }

//        [Fact]
//        public async Task CreateProperty_SavesTheCompletePropertyData()
//        {
//            using var context = CreateContext(Guid.NewGuid().ToString());
//            var service = CreateService(context);

//            var wifi = new Amenity();
//            wifi.Create(0, "Wi-Fi");
//            await new AmenityRepository(context).Add(wifi);

//            var property = CreateProperty("host-1", "Sea View Apartment", "A bright apartment near the sea.", 4, 4, 2);
//            var address = CreateAddress("Egypt", "Alexandria", "Corniche Road");
//            var listing = CreateListing("A bright apartment near the sea.", 150m);
//            var amenities = new[] { CreatePropertyAmenity(wifi.AmenitiesID) };
//            var bedrooms = new[] { CreateBedroomWithBed(1, "Master bedroom", BedType.Double, 1) };
//            var images = new[] { CreateImage("/uploads/properties/sea-view.jpg") };

//            var created = await service.CreateProperty(property, address, listing, amenities, bedrooms, images);
//            var details = await service.GetPropertyDetails(property.PropertyID, "host-1");

//            Assert.True(created);
//            Assert.NotNull(details);
//            Assert.Equal("Alexandria", details!.Address.City);
//            Assert.Equal(150m, details.Listing.Price);
//            Assert.Equal(ListingStatus.Pending, details.Listing.ListingStatus);
//            Assert.Single(details.PropertyAmenities);
//            Assert.Single(details.Bedrooms);
//            Assert.Single(details.Bedrooms.First().Beds);
//            Assert.Single(details.Images);
//        }

//        [Fact]
//        public async Task UpdateAndDeleteProperty_OnlyWorkForItsOwner()
//        {
//            using var context = CreateContext(Guid.NewGuid().ToString());
//            var service = CreateService(context);
//            var propertyId = await CreateTestProperty(service, "host-1");

//            var updateProperty = CreateProperty("host-1", "Updated Apartment", "An updated apartment description.", 6, 6, 2);
//            var updateAddress = CreateAddress("Egypt", "Cairo", "Tahrir Square");
//            var updateListing = CreateListing("Updated listing description.", 220m);

//            var updatedByAnotherHost = await service.UpdateProperty(
//                propertyId,
//                "host-2",
//                updateProperty,
//                updateAddress,
//                updateListing,
//                [],
//                []);
//            var deletedByAnotherHost = await service.DeleteProperty(propertyId, "host-2");

//            var updatedByOwner = await service.UpdateProperty(
//                propertyId,
//                "host-1",
//                updateProperty,
//                updateAddress,
//                updateListing,
//                [],
//                []);
//            var updatedDetails = await service.GetPropertyDetails(propertyId, "host-1");
//            var deletedByOwner = await service.DeleteProperty(propertyId, "host-1");
//            var hostProperties = await service.GetPropertiesByOwner("host-1");

//            Assert.False(updatedByAnotherHost);
//            Assert.False(deletedByAnotherHost);
//            Assert.True(updatedByOwner);
//            Assert.NotNull(updatedDetails);
//            Assert.Equal("Updated Apartment", updatedDetails!.PropertyName);
//            Assert.Equal("Cairo", updatedDetails.Address.City);
//            Assert.Equal(220m, updatedDetails.Listing.Price);
//            Assert.True(deletedByOwner);
//            Assert.Empty(hostProperties);
//        }

//        [Fact]
//        public async Task AddAndRemoveImages_RequirePropertyOwnership()
//        {
//            using var context = CreateContext(Guid.NewGuid().ToString());
//            var service = CreateService(context);
//            var propertyId = await CreateTestProperty(service, "host-1");

//            var addedByAnotherHost = await service.AddPropertyImages(
//                propertyId,
//                "host-2",
//                new[] { CreateImage("/uploads/properties/not-allowed.jpg") });
//            var addedByOwner = await service.AddPropertyImages(
//                propertyId,
//                "host-1",
//                new[] { CreateImage("/uploads/properties/allowed.jpg") });
//            var details = await service.GetPropertyDetails(propertyId, "host-1");
//            var imageId = details!.Images.Single().ImageID;

//            var removedByAnotherHost = await service.RemovePropertyImage(propertyId, imageId, "host-2");
//            var removedByOwner = await service.RemovePropertyImage(propertyId, imageId, "host-1");

//            Assert.False(addedByAnotherHost);
//            Assert.True(addedByOwner);
//            Assert.False(removedByAnotherHost);
//            Assert.True(removedByOwner);
//        }

//        [Fact]
//        public async Task UpdateProperty_ResetsApprovedListingToPendingStatus()
//        {
//            using var context = CreateContext(Guid.NewGuid().ToString());
//            var service = CreateService(context);
//            var propertyId = await CreateTestProperty(service, "host-1");

//            // Simulate admin approval
//            var listingRepo = new ListingRepository(context);
//            var listing = await listingRepo.GetById(propertyId);
//            listing!.Approve();
//            listingRepo.Update(listing);

//            var approvedDetails = await service.GetPropertyDetails(propertyId, "host-1");
//            Assert.Equal(ListingStatus.Approved, approvedDetails!.Listing.ListingStatus);

//            // Host edits property
//            var updateProperty = CreateProperty("host-1", "Updated Title", "Updated Description", 3, 3, 2);
//            var updateAddress = CreateAddress("Egypt", "Giza", "Updated Street");
//            var updateListing = CreateListing("Updated Description", 180m);

//            var updated = await service.UpdateProperty(propertyId, "host-1", updateProperty, updateAddress, updateListing, [], []);

//            var updatedDetails = await service.GetPropertyDetails(propertyId, "host-1");
//            Assert.True(updated);
//            Assert.NotNull(updatedDetails);
//            Assert.Equal(ListingStatus.Pending, updatedDetails!.Listing.ListingStatus);
//        }

//        [Fact]
//        public async Task CreateProperty_WithInvalidData_ReturnsFalse()
//        {
//            using var context = CreateContext(Guid.NewGuid().ToString());
//            var service = CreateService(context);

//            // Negative price
//            var property = CreateProperty("host-1", "Prop", "Desc", 2, 2, 1);
//            var address = CreateAddress("Egypt", "Cairo", "Street");
//            var invalidListing = CreateListing("Desc", -50m);

//            var result = await service.CreateProperty(property, address, invalidListing, [], [], []);
//            Assert.False(result);
//        }

//        [Fact]
//        public async Task CreateProperty_WithNonExistentAmenity_ReturnsFalse()
//        {
//            using var context = CreateContext(Guid.NewGuid().ToString());
//            var service = CreateService(context);

//            var property = CreateProperty("host-1", "Prop", "Desc", 2, 2, 1);
//            var address = CreateAddress("Egypt", "Cairo", "Street");
//            var listing = CreateListing("Desc", 100m);
//            var invalidAmenities = new[] { CreatePropertyAmenity(99999) }; // Amenity ID 99999 does not exist

//            var result = await service.CreateProperty(property, address, listing, invalidAmenities, [], []);
//            Assert.False(result);
//        }

//        private static async Task<long> CreateTestProperty(PropertyServices service, string ownerUserId)
//        {
//            var property = CreateProperty(ownerUserId, "Test Property", "A property used by a service test.", 2, 2, 1);
//            var address = CreateAddress("Egypt", "Giza", "Pyramids Street");
//            var listing = CreateListing("A property used by a service test.", 100m);

//            var created = await service.CreateProperty(property, address, listing, [], [], []);
//            Assert.True(created);
//            return property.PropertyID;
//        }

//        private static Property CreateProperty(string ownerUserId, string name, string description, int guests, int capacity, int bathrooms)
//        {
//            var property = new Property();
//            property.Create(ownerUserId, 0, name, description, guests, capacity, bathrooms);
//            return property;
//        }

//        private static Address CreateAddress(string country, string city, string street)
//        {
//            var address = new Address();
//            address.Create(0, country, city, street, 0m, 0m);
//            return address;
//        }

//        private static Listing CreateListing(string description, decimal price)
//        {
//            var listing = new Listing();
//            listing.Create(0, description, price);
//            return listing;
//        }

//        private static PropertyAmenity CreatePropertyAmenity(long amenityId)
//        {
//            var propertyAmenity = new PropertyAmenity();
//            propertyAmenity.Create(0, amenityId);
//            return propertyAmenity;
//        }

//        private static Bedroom CreateBedroomWithBed(int roomNumber, string roomName, BedType bedType, int quantity)
//        {
//            var bedroom = new Bedroom();
//            bedroom.Create(0, 0, roomNumber, bedcnt: quantity, roomName: roomName);

//            var bed = new Bed();
//            bed.Create(0, 0, bedType, quantity);
//            bedroom.Beds.Add(bed);
//            return bedroom;
//        }

//        private static PropertyImage CreateImage(string path)
//        {
//            var image = new PropertyImage();
//            image.Create(0, 0, path);
//            return image;
//        }
//    }
//}
