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
    }
}