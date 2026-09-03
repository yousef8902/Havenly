using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Havenly.DAL.Repos.Implementations;

namespace Havenly.DAL.Tests
{
    public class RepositoryCrudTests
    {
        private HavenlyDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<HavenlyDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new HavenlyDbContext(options);
        }

        [Fact]
        public async Task User_CRUD_Works()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var repo = new UserRepository(context);

            var user = new User();
            user.Create("Alice", "hash", "alice@example.com", "Guest");

            await repo.Add(user);

            // IdentityUser uses a string Id.
            var id = user.Id;

            var fetched = await repo.GetById(id);
            Assert.NotNull(fetched);

            user.Update("Alice2", "hash2", "alice2@example.com", "Host");
            repo.Update(user);

            var updated = await repo.GetById(id);
            Assert.Equal("Alice2", updated?.Name);

            repo.Delete(user);

            var deleted = await repo.GetById(id);

            // User uses Status instead of the old IsDeleted property.
            Assert.Equal(Havenly.DAL.Enums.UserStatus.Deleted, deleted?.Status);
        }

        [Fact]
        public async Task Property_CRUD_Works()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var repo = new PropertyRepository(context);

            var property = new Property();
            property.Create(
                ownerUserId: "1",
                addressId: 1,
                propertyName: "Prop1",
                description: "Desc",
                numberOfGuests: 2,
                capacity: 2,
                bathroomCount: 1);

            await repo.Add(property);

            var id = property.PropertyID;
            var fetched = await repo.GetById(id);
            Assert.NotNull(fetched);

            property.Update("Prop1-upd", "Desc-upd", 3, 3, 2);
            repo.Update(property);

            var updated = await repo.GetById(id);
            Assert.Equal("Prop1-upd", updated?.PropertyName);

            repo.Delete(property);
            var deleted = await repo.GetById(id);
            Assert.True(deleted?.IsDeleted == true);
        }

        [Fact]
        public async Task Listing_CRUD_Works()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var repo = new ListingRepository(context);

            var listing = new Listing();
            listing.Create(propertyId: 1, description: "L1", price: 10m);

            await repo.Add(listing);

            var id = listing.ListingID;
            var fetched = await repo.GetById(id);
            Assert.NotNull(fetched);

            listing.Update("L1-upd", 12m);
            repo.Update(listing);

            var updated = await repo.GetById(id);
            Assert.Equal(12m, updated?.Price);

            repo.Delete(listing);
            var deleted = await repo.GetById(id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task Favorite_CRUD_Works()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var repo = new FavoriteRepository(context);

            var fav = new Favorite();
            fav.Create(userId: "1", listingId: 1);

            await repo.Add(fav);

            var id = fav.FavoriteID;
            var fetched = await repo.GetById(id);
            Assert.NotNull(fetched);

            repo.Delete(fav);
            var deleted = await repo.GetById(id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task Review_CRUD_Works()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var repo = new ReviewRepository(context);

            var review = new Review();
            review.Create(
                reviewId: 0,
                userId: "1",
                propertyId: 1,
                rating: 5,
                comment: "good");

            await repo.Add(review);

            var id = review.ReviewID;
            var fetched = await repo.GetById(id);
            Assert.NotNull(fetched);

            review.Update(4, "ok");
            repo.Update(review);

            var updated = await repo.GetById(id);
            Assert.Equal(4, updated?.Rating);

            repo.Delete(review);
            var deleted = await repo.GetById(id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task Review_Respond_And_GetDetailById_Works()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var repo = new ReviewRepository(context);

            var user = new User();
            user.Create("Guest User", "hash", "guest@example.com", "Guest");
            await context.Users.AddAsync(user);

            var host = new User();
            host.Create("Host User", "hash", "host@example.com", "Host");
            await context.Users.AddAsync(host);

            var prop = new Property();
            prop.Create(host.Id, 0, "Luxury Dahab Villa", "Desc", 4, 4, 2);
            await context.Properties.AddAsync(prop);
            await context.SaveChangesAsync();

            var review = new Review();
            review.Create(0, user.Id, prop.PropertyID, 5, "Amazing experience in South Sinai!");
            await repo.Add(review);

            var detail = await repo.GetDetailById(review.ReviewID);
            Assert.NotNull(detail);
            Assert.Equal("Guest User", detail.User?.Name);
            Assert.Equal("Luxury Dahab Villa", detail.Property?.PropertyName);

            var respondSuccess = await repo.RespondToReview(review.ReviewID, "Thank you so much for staying with us!");
            Assert.True(respondSuccess);

            var updated = await repo.GetById(review.ReviewID);
            Assert.NotNull(updated);
            Assert.Equal("Thank you so much for staying with us!", updated.HostResponse);
        }

        [Fact]
        public async Task Address_CRUD_Works()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var repo = new AddressRepository(context);

            var addr = new Address();
            addr.Create(
                addressID: 0,
                country: "C",
                city: "City",
                street: "St",
                latitude: 0m,
                longitude: 0m);

            await repo.Add(addr);

            var id = addr.AddressID;
            var fetched = await repo.GetById(id);
            Assert.NotNull(fetched);

            addr.Update("C2", "City2", "St2", 1m, 1m);
            repo.Update(addr);

            var updated = await repo.GetById(id);
            Assert.Equal("C2", updated?.Country);

            repo.Delete(addr);
            var deleted = await repo.GetById(id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task Payment_CRUD_Works()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);
            var repo = new PaymentRepository(context);

            var booking = new Booking();
            booking.Create("u1", 1, DateTime.Today, DateTime.Today.AddDays(1), 10m, Havenly.DAL.Enums.BookingStatus.Pending);
            await context.Bookings.AddAsync(booking);
            await context.SaveChangesAsync();

            var pay = new Payment();
            pay.Create(
                paymentId: 0,
                bookingId: booking.BookingID,
                gateway: "G",
                amount: 10m,
                transactionId: "T");

            await repo.Add(pay);

            var id = pay.PaymentID;
            var fetched = await repo.GetById(id);
            Assert.NotNull(fetched);

            pay.Update("G2", 12m, "T2");
            repo.Update(pay);

            var updated = await repo.GetById(id);
            Assert.Equal("G2", updated?.Gateway);

            repo.Delete(pay);
            var deleted = await repo.GetById(id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task Bedroom_Bed_PropertyImage_Amenity_PropertyAmenity_CRUD_Works()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);

            var prop = new Property();
            prop.Create("u1", 0, "Test Prop", "Desc", 2, 2, 1);
            await context.Properties.AddAsync(prop);
            await context.SaveChangesAsync();

            var bedroomRepo = new BedroomRepository(context);
            var bedRepo = new BedRepository(context);
            var imgRepo = new PropertyImageRepository(context);
            var amenRepo = new AmenityRepository(context);
            var paRepo = new PropertyAmenityRepository(context);

            var bedroom = new Bedroom();
            bedroom.Create(
                bedroomId: 0,
                propertyId: prop.PropertyID,
                roomNumber: 1,
                bedcnt: 1,
                roomName: "R");

            await bedroomRepo.Add(bedroom);
            Assert.NotNull(await bedroomRepo.GetById(bedroom.BedroomID));

            var bed = new Bed();
            bed.Create(
                bedId: 0,
                bedroomId: bedroom.BedroomID,
                bedType: Havenly.DAL.Enums.BedType.Single,
                quantity: 1);

            await bedRepo.Add(bed);
            Assert.NotNull(await bedRepo.GetById(bed.BedID));

            var img = new PropertyImage();
            img.Create(
                imageId: 0,
                propertyId: prop.PropertyID,
                imagePath: "p.jpg");

            await imgRepo.Add(img);
            Assert.NotNull(await imgRepo.GetById(img.ImageID));

            var amen = new Amenity();
            amen.Create(
                amenitiesId: 1,
                name: "A");

            await amenRepo.Add(amen);
            Assert.NotNull(await amenRepo.GetById(amen.AmenitiesID));

            var pa = new PropertyAmenity();
            pa.Create(
                propertyId: prop.PropertyID,
                amenityId: amen.AmenitiesID);

            await paRepo.Add(pa);

            // Composite key; cannot rely on a single ID.
            var all = await paRepo.GetAll();
            Assert.NotEmpty(all);
        }

        [Fact]
        public async Task Chatbot_RecommendationEngine_ScopeGuard_And_Search_Works()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateContext(dbName);

            // 1. Seed Property in Dahab
            var addr = new Address();
            addr.Create(0, "Egypt", "Dahab", "Lighthouse Road", 28.5m, 34.5m);
            await context.Addresses.AddAsync(addr);
            await context.SaveChangesAsync();

            var prop = new Property();
            prop.Create("host1", addr.AddressID, "Sea Breeze Dahab Villa", "Beachfront villa with stunning Red Sea views",
                numberOfGuests: 4, capacity: 4, bathroomCount: 2, category: "Design homes");
            prop.Rating = 4.9;
            prop.NumberOfReviews = 10;
            await context.Properties.AddAsync(prop);
            await context.SaveChangesAsync();

            var img = new PropertyImage();
            img.Create(0, prop.PropertyID, "/images/dahab_villa.jpg", isPrimary: true);
            await context.PropertyImages.AddAsync(img);

            var listing = new Listing();
            listing.Create(prop.PropertyID, "Dahab Luxury Listing", 900m, isValid: true);
            listing.ListingStatus = Havenly.DAL.Enums.ListingStatus.Approved;
            await context.Listings.AddAsync(listing);
            await context.SaveChangesAsync();

            var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<Havenly.BLL.Services.Implementations.PropertyRecommendationEngine>.Instance;
            var engine = new Havenly.BLL.Services.Implementations.PropertyRecommendationEngine(context, logger);

            // Test 1: Out-of-scope query guard (e.g. coding or war)
            var outOfScopeRes = await engine.ProcessChatQueryAsync("Can you write python code to reverse a binary tree?");
            Assert.True(outOfScopeRes.Success);
            Assert.Contains("Havenly's Travel & Property Concierge", outOfScopeRes.Message);
            Assert.Contains("consult external resources", outOfScopeRes.Message);
            Assert.Null(outOfScopeRes.RecommendedProperties);
            Assert.True(outOfScopeRes.Metadata?.ContainsKey("IsHandledLocally") == true);

            var warRes = await engine.ProcessChatQueryAsync("tell me about the war");
            Assert.True(warRes.Success);
            Assert.Contains("Havenly's Travel & Property Concierge", warRes.Message);
            Assert.Contains("consult external resources", warRes.Message);
            Assert.Null(warRes.RecommendedProperties);
            Assert.True(warRes.Metadata?.ContainsKey("IsHandledLocally") == true);

            // Test 2: Platform FAQ
            var faqRes = await engine.ProcessChatQueryAsync("How do payments work?");
            Assert.True(faqRes.Success);
            Assert.Contains("Paymob", faqRes.Message);
            Assert.Contains("Egyptian Pounds", faqRes.Message);
            Assert.True(faqRes.Metadata?.ContainsKey("IsHandledLocally") == true);

            // Test 3: Recommendation Search in Dahab
            var recRes = await engine.ProcessChatQueryAsync("Recommend a place in Dahab under 1000 EGP for 2 guests");
            Assert.True(recRes.Success);
            Assert.NotNull(recRes.RecommendedProperties);
            Assert.Single(recRes.RecommendedProperties);

            var card = recRes.RecommendedProperties[0];
            Assert.Equal(prop.PropertyID, card.PropertyId);
            Assert.Equal("Sea Breeze Dahab Villa", card.Title);
            Assert.Equal("Dahab", card.City);
            Assert.Equal(900m, card.PricePerNight);
            Assert.Equal("/images/dahab_villa.jpg", card.ImageUrl);
            Assert.Equal($"/Property/Detail/{prop.PropertyID}", card.DetailUrl);

            // Test 4: Recommendation Search in Giza (verifying Giza is not lumped with Cairo)
            var gizaAddr = new Address();
            gizaAddr.Create(0, "Egypt", "Giza", "Pyramids View St", 29.9m, 31.1m);
            await context.Addresses.AddAsync(gizaAddr);
            await context.SaveChangesAsync();

            var gizaProp = new Property();
            gizaProp.Create("host2", gizaAddr.AddressID, "a nice unit", "Modern apartment close to the Pyramids",
                numberOfGuests: 5, capacity: 5, bathroomCount: 1, category: "Design homes");
            gizaProp.Rating = 5.0;
            gizaProp.NumberOfReviews = 1;
            await context.Properties.AddAsync(gizaProp);
            await context.SaveChangesAsync();

            var gizaListing = new Listing();
            gizaListing.Create(gizaProp.PropertyID, "Giza Stay", 750m, isValid: true);
            gizaListing.ListingStatus = Havenly.DAL.Enums.ListingStatus.Approved;
            await context.Listings.AddAsync(gizaListing);
            await context.SaveChangesAsync();

            var gizaRes = await engine.ProcessChatQueryAsync("stays in Giza");
            Assert.True(gizaRes.Success);
            Assert.NotNull(gizaRes.RecommendedProperties);
            Assert.Single(gizaRes.RecommendedProperties);
            Assert.Equal(gizaProp.PropertyID, gizaRes.RecommendedProperties[0].PropertyId);
            Assert.Equal("a nice unit", gizaRes.RecommendedProperties[0].Title);
            Assert.Equal("Giza", gizaRes.RecommendedProperties[0].City);
            Assert.Equal(750m, gizaRes.RecommendedProperties[0].PricePerNight);
        }
    }
}