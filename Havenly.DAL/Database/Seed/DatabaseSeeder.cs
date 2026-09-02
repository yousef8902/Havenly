using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Database.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(
            HavenlyDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Seed roles first so they are available when creating users
            await SeedRolesAsync(roleManager);

            // Ensure Category column and User Profile columns exist in SQL Server
            try
            {
                await context.Database.ExecuteSqlRawAsync(
                    @"IF NOT EXISTS (
                        SELECT 1 FROM sys.columns 
                        WHERE object_id = OBJECT_ID('Properties') AND name = 'Category'
                    )
                    BEGIN
                        ALTER TABLE Properties ADD Category NVARCHAR(100) NULL DEFAULT 'Design homes';
                    END
                    
                    IF NOT EXISTS (
                        SELECT 1 FROM sys.columns 
                        WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'ProfilePictureUrl'
                    )
                    BEGIN
                        ALTER TABLE AspNetUsers ADD ProfilePictureUrl NVARCHAR(300) NULL;
                    END

                    IF NOT EXISTS (
                        SELECT 1 FROM sys.columns 
                        WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Bio'
                    )
                    BEGIN
                        ALTER TABLE AspNetUsers ADD Bio NVARCHAR(1000) NULL;
                    END

                    IF NOT EXISTS (
                        SELECT 1 FROM sys.columns 
                        WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'JoinedDate'
                    )
                    BEGIN
                        ALTER TABLE AspNetUsers ADD JoinedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE();
                    END"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SQL column check note: {ex.Message}");
            }

            bool hasUsers = await context.Users.AnyAsync();
            if (!hasUsers)
            {
                // Seed in correct order to respect foreign key constraints
                await SeedAddresses(context);
                await SeedUsersAsync(userManager);
                await SeedProperties(context);
                await SeedBedrooms(context);
                await SeedPropertyImages(context);
                await SeedListings(context);
            }

            // Ensure amenities & property amenities are seeded
            await SeedAmenities(context);
            await SeedPropertyAmenities(context);

            // Ensure varied property ratings (from 3.8 to 5.0) and matching categories from sidebar options
            await UpdatePropertyRatingsAndCategories(context);

            // Ensure all seeded listings are approved for home & search
            var listingsToApprove = await context.Listings.ToListAsync();
            if (listingsToApprove.Any())
            {
                foreach (var l in listingsToApprove)
                {
                    l.IsValid = true;
                    l.Approve();
                }
                await context.SaveChangesAsync();
            }

            // Ensure bookings, payments, reviews and favorites exist
            await SeedBookings(context);
            await SeedPayments(context);
            await SeedReviews(context);
            await SeedFavorites(context);

            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { UserRoles.Admin, UserRoles.Host, UserRoles.Guest };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAddresses(HavenlyDbContext context)
        {
            var addresses = new List<Address>
            {
                new Address { City = "El Gouna", Country = "Egypt", Street = "Tawila Island Lagoon, El Gouna", Latitude = 27.3949m, Longitude = 33.6766m },
                new Address { City = "Dahab", Country = "Egypt", Street = "Lighthouse Reef Bay, Dahab", Latitude = 28.5095m, Longitude = 34.5137m },
                new Address { City = "Cairo", Country = "Egypt", Street = "Abou El Feda St, Zamalek", Latitude = 30.0626m, Longitude = 31.2197m },
                new Address { City = "Siwa Oasis", Country = "Egypt", Street = "Aghurmi Salt Lake Valley, Siwa", Latitude = 29.2032m, Longitude = 25.5195m },
                new Address { City = "North Coast (Sahel)", Country = "Egypt", Street = "Sidi Abdel Rahman Bay, Sahel", Latitude = 30.9575m, Longitude = 28.7180m },
                new Address { City = "Fayoum", Country = "Egypt", Street = "Tunis Pottery Village, Lake Qarun", Latitude = 29.4140m, Longitude = 30.4900m },
                new Address { City = "Luxor", Country = "Egypt", Street = "Al-Gharbi Nile West Bank, Luxor", Latitude = 25.7202m, Longitude = 32.6105m },
                new Address { City = "Aswan", Country = "Egypt", Street = "Elephantine Island, Aswan", Latitude = 24.0889m, Longitude = 32.8998m }
            };

            await context.Addresses.AddRangeAsync(addresses);
            await context.SaveChangesAsync();
        }

        private static async Task SeedUsersAsync(UserManager<User> userManager)
        {
            var usersToSeed = new List<(string Name, string Email, string Role)>
            {
                ("Test Guest", "guest@test.com", UserRoles.Guest),
                ("Test Host", "host@test.com", UserRoles.Host),
                ("Test Admin", "admin@test.com", UserRoles.Admin),
                ("Elena Marinos", "elena.marinos@havenly.co", UserRoles.Host),
                ("Nadia Rahman", "nadia.rahman@mail.com", UserRoles.Guest),
                ("Giulia Ferrari", "giulia@casafiora.it", UserRoles.Host),
                ("Tom Bergman", "t.bergman@mail.com", UserRoles.Guest),
                ("Rui Almeida", "rui.almeida@mail.com", UserRoles.Host),
                ("Yara Fahmy", "yara.fahmy@mail.com", UserRoles.Guest),
                ("Mikkel Sørensen", "mikkel@northloft.dk", UserRoles.Host),
                ("Chloe Deveraux", "chloe.d@mail.com", UserRoles.Guest),
                ("Omar Khalil", "omar.khalil@havenly.co", UserRoles.Admin)
            };

            foreach (var item in usersToSeed)
            {
                try
                {
                    var existingUser = await userManager.FindByEmailAsync(item.Email);
                    if (existingUser != null)
                    {
                        continue;
                    }

                    var user = new User
                    {
                        UserName = item.Email,
                        Email = item.Email,
                        Name = item.Name,
                        Role = item.Role,
                        EmailConfirmed = true,
                        Status = UserStatus.Active,
                        LockoutEnabled = false,
                        PhoneNumberConfirmed = false,
                        TwoFactorEnabled = false,
                        AccessFailedCount = 0
                    };

                    var result = await userManager.CreateAsync(user, "P@ssword123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, item.Role);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating user '{item.Name}': {ex.Message}");
                }
            }
        }

        private static async Task SeedProperties(HavenlyDbContext context)
        {
            var users = await context.Users.ToListAsync();
            var userMap = users.ToDictionary(u => u.Name, u => u.Id);
            var addresses = await context.Addresses.ToListAsync();
            var hostId = userMap.ContainsKey("Test Host") ? userMap["Test Host"] : userMap.Values.First();

            var properties = new List<Property>
            {
                new Property
                {
                    PropertyName = "Villa Turquoise — Lagoon Beachfront Villa with Private Jetty",
                    Description = "Nestled along the crystal-clear lagoons of El Gouna, this open-concept architectural villa features a private heated infinity pool, private lagoon beach access, and spacious sun decks.",
                    Category = "Beachfront",
                    NumberOfGuests = 6,
                    Capacity = 6,
                    BathroomCount = 3,
                    Rating = 4.97,
                    NumberOfReviews = 54,
                    OwnerUserID = hostId,
                    AddressID = addresses[0].AddressID
                },
                new Property
                {
                    PropertyName = "Blue Coral Sanctuary — Boho Beach House by the Reef",
                    Description = "Right on the shores of the Gulf of Aqaba in Dahab, enjoy bohemian interiors with natural palm-leaf ceilings, direct reef snorkeling steps from your patio, and Sinai mountain sunset views.",
                    Category = "Islands",
                    NumberOfGuests = 4,
                    Capacity = 4,
                    BathroomCount = 2,
                    Rating = 4.88,
                    NumberOfReviews = 42,
                    OwnerUserID = hostId,
                    AddressID = addresses[1].AddressID
                },
                new Property
                {
                    PropertyName = "The Nile View Loft — Modern Art Deco Penthouse in Zamalek",
                    Description = "Perched high above leafy Zamalek with sweeping panoramic vistas of the River Nile and Cairo skyline. Designed with heritage parquet floors, contemporary art, and a wrap-around sunset balcony.",
                    Category = "City lofts",
                    NumberOfGuests = 3,
                    Capacity = 3,
                    BathroomCount = 2,
                    Rating = 4.92,
                    NumberOfReviews = 68,
                    OwnerUserID = hostId,
                    AddressID = addresses[2].AddressID
                },
                new Property
                {
                    PropertyName = "Salt Lake Eco-Lodge — Kershef Adobe Chalet in Date Palms",
                    Description = "Constructed entirely from authentic Siwan Kershef (salt rock and clay) amidst tranquil olive and date palm groves. Features private natural spring plunge pool and starry desert night skies.",
                    Category = "Cabins",
                    NumberOfGuests = 4,
                    Capacity = 4,
                    BathroomCount = 2,
                    Rating = 4.95,
                    NumberOfReviews = 31,
                    OwnerUserID = hostId,
                    AddressID = addresses[3].AddressID
                },
                new Property
                {
                    PropertyName = "The White Sands Manor — Mediterranean Beachfront Haven",
                    Description = "Overlooking the crystal-clear turquoise waters and white sandy beaches of the North Coast (Sahel). Minimalist Mediterranean lines, expansive outdoor terrace, and private beach cabana.",
                    Category = "Beachfront",
                    NumberOfGuests = 8,
                    Capacity = 8,
                    BathroomCount = 4,
                    Rating = 4.98,
                    NumberOfReviews = 43,
                    OwnerUserID = hostId,
                    AddressID = addresses[4].AddressID
                },
                new Property
                {
                    PropertyName = "Dar El Qamar — Artistic Country Farmhouse near Lake Qarun",
                    Description = "Located in the famous potters' village of Tunis in Fayoum. Built with dome architecture, limestone walls, lush private gardens, and rooftop views overlooking Lake Qarun and desert dunes.",
                    Category = "Countryside",
                    NumberOfGuests = 4,
                    Capacity = 4,
                    BathroomCount = 2,
                    Rating = 4.75,
                    NumberOfReviews = 29,
                    OwnerUserID = hostId,
                    AddressID = addresses[5].AddressID
                },
                new Property
                {
                    PropertyName = "Al-Qurna Heritage Palace — West Bank Villa with Ancient Views",
                    Description = "Nestled on the tranquil West Bank of Luxor facing the Theban hills and Valley of the Nobles. Traditional Egyptian courtyard, hand-carved stone arches, and rooftop stargazing over the Nile.",
                    Category = "Design homes",
                    NumberOfGuests = 4,
                    Capacity = 4,
                    BathroomCount = 2,
                    Rating = 4.91,
                    NumberOfReviews = 36,
                    OwnerUserID = hostId,
                    AddressID = addresses[6].AddressID
                },
                new Property
                {
                    PropertyName = "Nubian Sun Villa — Colorful Island Retreat on the Nile",
                    Description = "Immerse yourself in authentic Nubian warmth and vibrant color palettes on the banks of the First Cataract in Aswan. Traditional vaulted ceilings, breezy terrace, and private felucca landing.",
                    Category = "Islands",
                    NumberOfGuests = 5,
                    Capacity = 5,
                    BathroomCount = 2,
                    Rating = 4.89,
                    NumberOfReviews = 48,
                    OwnerUserID = hostId,
                    AddressID = addresses[7].AddressID
                }
            };

            await context.Properties.AddRangeAsync(properties);
            await context.SaveChangesAsync();
        }

        private static async Task UpdatePropertyRatingsAndCategories(HavenlyDbContext context)
        {
            var properties = await context.Properties.ToListAsync();
            var metaMap = new Dictionary<string, (double Rating, string Category)>
            {
                { "Villa Turquoise — Lagoon Beachfront Villa with Private Jetty", (4.97, "Beachfront") },
                { "Blue Coral Sanctuary — Boho Beach House by the Reef", (4.88, "Islands") },
                { "The Nile View Loft — Modern Art Deco Penthouse in Zamalek", (4.92, "City lofts") },
                { "Salt Lake Eco-Lodge — Kershef Adobe Chalet in Date Palms", (4.95, "Cabins") },
                { "The White Sands Manor — Mediterranean Beachfront Haven", (4.98, "Beachfront") },
                { "Dar El Qamar — Artistic Country Farmhouse near Lake Qarun", (4.75, "Countryside") },
                { "Al-Qurna Heritage Palace — West Bank Villa with Ancient Views", (4.91, "Design homes") },
                { "Nubian Sun Villa — Colorful Island Retreat on the Nile", (4.89, "Islands") }
            };

            bool changed = false;
            foreach (var prop in properties)
            {
                if (metaMap.TryGetValue(prop.PropertyName, out var meta))
                {
                    if (Math.Abs(prop.Rating - meta.Rating) > 0.01)
                    {
                        prop.UpdateReview(meta.Rating);
                        changed = true;
                    }
                    if (prop.Category != meta.Category)
                    {
                        prop.Category = meta.Category;
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedAmenities(HavenlyDbContext context)
        {
            if (await context.Amenities.AnyAsync()) return;

            var amenityNames = new[]
            {
                "Wi-Fi", "Parking", "Kitchen", "Air conditioning",
                "Pool", "Hot tub", "Patio or balcony", "Dedicated workspace",
                "EV charger", "Pet friendly"
            };

            var amenities = new List<Amenity>();
            foreach (var name in amenityNames)
            {
                var a = new Amenity();
                a.Create(0, name);
                amenities.Add(a);
            }

            await context.Amenities.AddRangeAsync(amenities);
            await context.SaveChangesAsync();
        }

        private static async Task SeedPropertyAmenities(HavenlyDbContext context)
        {
            if (await context.PropertyAmenities.AnyAsync()) return;

            var properties = await context.Properties.ToListAsync();
            var amenities = await context.Amenities.ToListAsync();

            var propMap = properties.ToDictionary(p => p.PropertyName);
            var amMap = amenities.ToDictionary(a => a.Name, a => a.AmenitiesID);

            var propertyAmenities = new List<PropertyAmenity>();

            void AddPropertyAmenities(string propName, params string[] amenityNames)
            {
                if (!propMap.TryGetValue(propName, out var prop)) return;
                foreach (var amName in amenityNames)
                {
                    if (amMap.TryGetValue(amName, out var amId))
                    {
                        var pa = new PropertyAmenity();
                        pa.Create(prop.PropertyID, amId);
                        propertyAmenities.Add(pa);
                    }
                }
            }

            AddPropertyAmenities("Olive Ridge — Cliffside Villa with Infinity Pool",
                "Wi-Fi", "Parking", "Kitchen", "Air conditioning", "Pool", "Patio or balcony", "Dedicated workspace");

            AddPropertyAmenities("North Loft — Bright Oak Apartment in the Old Town",
                "Wi-Fi", "Kitchen", "Air conditioning", "Dedicated workspace");

            AddPropertyAmenities("Casa Fiora — Restored Stone Farmhouse",
                "Wi-Fi", "Parking", "Kitchen", "Pool", "Patio or balcony");

            AddPropertyAmenities("Pine Hollow — Glass Cabin in the Forest",
                "Wi-Fi", "Parking", "Kitchen", "Hot tub", "Dedicated workspace");

            AddPropertyAmenities("Salt House — Beachfront Home with Open Terrace",
                "Wi-Fi", "Parking", "Kitchen", "Air conditioning", "Patio or balcony");

            AddPropertyAmenities("Skyline Nine — Penthouse Terrace above the River",
                "Wi-Fi", "Kitchen", "Air conditioning", "Patio or balcony", "Dedicated workspace");

            AddPropertyAmenities("Barn Eleven — Converted Hay Barn with Beams",
                "Wi-Fi", "Parking", "Kitchen", "Patio or balcony", "Pet friendly");

            AddPropertyAmenities("Cala Blanca — Village House with Blue Shutters",
                "Wi-Fi", "Kitchen", "Air conditioning", "Patio or balcony");

            if (propertyAmenities.Any())
            {
                await context.PropertyAmenities.AddRangeAsync(propertyAmenities);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedBedrooms(HavenlyDbContext context)
        {
            var properties = await context.Properties.ToListAsync();
            var propertyMap = properties.ToDictionary(p => p.PropertyName);

            var bedrooms = new List<Bedroom>
            {
                new Bedroom { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, RoomNumber = 1, BedCount = 1, RoomName = "Primary suite — King bed, sea view, ensuite with freestanding tub" },
                new Bedroom { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, RoomNumber = 2, BedCount = 1, RoomName = "Guest suite — Queen bed, terrace access, ensuite shower" },
                new Bedroom { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, RoomNumber = 3, BedCount = 2, RoomName = "Twin room — Two single beds, shared bathroom, mountain view" },
                new Bedroom { PropertyID = propertyMap["North Loft — Bright Oak Apartment in the Old Town"].PropertyID, RoomNumber = 1, BedCount = 1, RoomName = "Main bedroom — King bed, vaulted ceiling with original timber beams" },
                new Bedroom { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, RoomNumber = 1, BedCount = 1, RoomName = "Master bedroom — King four-poster bed, valley view, private terrace" },
                new Bedroom { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, RoomNumber = 2, BedCount = 1, RoomName = "Suite Giardino — Queen bed, stone fireplace, garden access" },
                new Bedroom { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, RoomNumber = 3, BedCount = 2, RoomName = "Twin bedroom — Two single beds, beamed ceiling" },
                new Bedroom { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, RoomNumber = 4, BedCount = 1, RoomName = "Tower room — Double bed, 360° countryside view" },
                new Bedroom { PropertyID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID, RoomNumber = 1, BedCount = 1, RoomName = "Glass bedroom — King bed beneath glass roof, blackout blinds" },
                new Bedroom { PropertyID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID, RoomNumber = 2, BedCount = 2, RoomName = "Loft space — Two single futon beds, forest views" },
                new Bedroom { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, RoomNumber = 1, BedCount = 1, RoomName = "Ocean suite — King bed, dune-facing private terrace, outdoor shower" },
                new Bedroom { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, RoomNumber = 2, BedCount = 1, RoomName = "East room — Queen bed, morning sun, polished concrete ensuite" },
                new Bedroom { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, RoomNumber = 3, BedCount = 2, RoomName = "Bunk room — Two single built-in bunks, shared bathroom" }
            };

            await context.Bedrooms.AddRangeAsync(bedrooms);
            await context.SaveChangesAsync();
        }

        private static async Task SeedPropertyImages(HavenlyDbContext context)
        {
            var properties = await context.Properties.ToListAsync();
            var propertyMap = properties.ToDictionary(p => p.PropertyName);

            var images = new List<PropertyImage>
            {
                new PropertyImage { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, ImagePath = "/images/p1.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, ImagePath = "/images/p2.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["North Loft — Bright Oak Apartment in the Old Town"].PropertyID, ImagePath = "/images/p3.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, ImagePath = "/images/p4.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID, ImagePath = "/images/p5.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, ImagePath = "/images/p6.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Skyline Nine — Penthouse Terrace above the River"].PropertyID, ImagePath = "/images/p7.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Barn Eleven — Converted Hay Barn with Beams"].PropertyID, ImagePath = "/images/p8.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Cala Blanca — Village House with Blue Shutters"].PropertyID, ImagePath = "/images/p9.jpg", IsPrimary = true }
            };

            await context.PropertyImages.AddRangeAsync(images);
            await context.SaveChangesAsync();
        }

        private static async Task SeedListings(HavenlyDbContext context)
        {
            var properties = await context.Properties.ToListAsync();
            var propertyMap = properties.ToDictionary(p => p.PropertyName);

            var listings = new List<Listing>
            {
                new Listing { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, Description = "Cliffside villa with infinity pool", Price = 340, IsValid = true, ListingStatus = ListingStatus.Approved },
                new Listing { PropertyID = propertyMap["North Loft — Bright Oak Apartment in the Old Town"].PropertyID, Description = "Bright oak apartment in old town", Price = 165, IsValid = true, ListingStatus = ListingStatus.Approved },
                new Listing { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, Description = "Restored stone farmhouse", Price = 220, IsValid = true, ListingStatus = ListingStatus.Approved },
                new Listing { PropertyID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID, Description = "Glass cabin in the forest", Price = 275, IsValid = true, ListingStatus = ListingStatus.Approved },
                new Listing { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, Description = "Beachfront home with open terrace", Price = 295, IsValid = true, ListingStatus = ListingStatus.Approved },
                new Listing { PropertyID = propertyMap["Skyline Nine — Penthouse Terrace above the River"].PropertyID, Description = "Penthouse terrace above the river", Price = 410, IsValid = true, ListingStatus = ListingStatus.Approved },
                new Listing { PropertyID = propertyMap["Barn Eleven — Converted Hay Barn with Beams"].PropertyID, Description = "Converted hay barn with beams", Price = 190, IsValid = true, ListingStatus = ListingStatus.Approved },
                new Listing { PropertyID = propertyMap["Cala Blanca — Village House with Blue Shutters"].PropertyID, Description = "Village house with blue shutters", Price = 145, IsValid = true, ListingStatus = ListingStatus.Approved }
            };

            await context.Listings.AddRangeAsync(listings);
            await context.SaveChangesAsync();
        }

        private static async Task SeedBookings(HavenlyDbContext context)
        {
            if (await context.Bookings.AnyAsync()) return;

            var users = await context.Users.ToListAsync();
            var listings = await context.Listings.ToListAsync();

            var userMap = users.ToDictionary(u => u.Name, u => u.Id);
            var listingMap = listings.ToDictionary(l => l.Description);

            var testGuestId = userMap.ContainsKey("Test Guest") ? userMap["Test Guest"] : userMap.Values.First();

            var bookings = new List<Booking>
            {
                // Test Guest's Bookings
                new Booking
                {
                    GuestUserID = testGuestId,
                    ListingID = listingMap["Cliffside villa with infinity pool"].ListingID,
                    CheckIn = DateTime.Today.AddDays(7),
                    CheckOut = DateTime.Today.AddDays(12),
                    TotalPrice = 1700,
                    Status = BookingStatus.Approved
                },
                new Booking
                {
                    GuestUserID = testGuestId,
                    ListingID = listingMap["Glass cabin in the forest"].ListingID,
                    CheckIn = DateTime.Today.AddDays(20),
                    CheckOut = DateTime.Today.AddDays(23),
                    TotalPrice = 825,
                    Status = BookingStatus.Pending
                },
                new Booking
                {
                    GuestUserID = testGuestId,
                    ListingID = listingMap["Restored stone farmhouse"].ListingID,
                    CheckIn = DateTime.Today.AddDays(-30),
                    CheckOut = DateTime.Today.AddDays(-26),
                    TotalPrice = 880,
                    Status = BookingStatus.Completed
                },
                // Other Guest Bookings
                new Booking
                {
                    GuestUserID = userMap["Nadia Rahman"],
                    ListingID = listingMap["Cliffside villa with infinity pool"].ListingID,
                    CheckIn = DateTime.Today.AddDays(15),
                    CheckOut = DateTime.Today.AddDays(20),
                    TotalPrice = 1700,
                    Status = BookingStatus.Approved
                },
                new Booking
                {
                    GuestUserID = userMap["Tom Bergman"],
                    ListingID = listingMap["Bright oak apartment in old town"].ListingID,
                    CheckIn = DateTime.Today.AddDays(-15),
                    CheckOut = DateTime.Today.AddDays(-10),
                    TotalPrice = 825,
                    Status = BookingStatus.Completed
                },
                new Booking
                {
                    GuestUserID = userMap["Yara Fahmy"],
                    ListingID = listingMap["Beachfront home with open terrace"].ListingID,
                    CheckIn = DateTime.Today.AddDays(2),
                    CheckOut = DateTime.Today.AddDays(5),
                    TotalPrice = 885,
                    Status = BookingStatus.Approved
                }
            };

            await context.Bookings.AddRangeAsync(bookings);
            await context.SaveChangesAsync();
        }

        private static async Task SeedPayments(HavenlyDbContext context)
        {
            if (await context.Payments.AnyAsync()) return;

            var bookings = await context.Bookings.ToListAsync();
            var payments = new List<Payment>();

            foreach (var booking in bookings)
            {
                var payment = new Payment();
                if (booking.Status == BookingStatus.Approved || booking.Status == BookingStatus.Completed)
                {
                    payment.Create(
                        paymentId: 0,
                        bookingId: booking.BookingID,
                        gateway: "Paymob Card",
                        amount: booking.TotalPrice,
                        transactionId: $"PM-{DateTime.UtcNow:yyyyMMdd}-{booking.BookingID * 1000 + 421}",
                        platformFee: Math.Round(booking.TotalPrice * 0.10m, 2),
                        hostPayoutAmount: Math.Round(booking.TotalPrice * 0.90m, 2),
                        status: PaymentStatus.Completed
                    );
                    payment.MarkCompleted($"PM-TXN-{booking.BookingID * 1000 + 421}", "Paymob Gateway");
                    if (booking.Status == BookingStatus.Completed)
                    {
                        payment.MarkPayoutToHost();
                    }
                }
                else
                {
                    payment.Create(
                        paymentId: 0,
                        bookingId: booking.BookingID,
                        gateway: "Paymob",
                        amount: booking.TotalPrice,
                        transactionId: $"PM-PENDING-{booking.BookingID}",
                        platformFee: Math.Round(booking.TotalPrice * 0.10m, 2),
                        hostPayoutAmount: Math.Round(booking.TotalPrice * 0.90m, 2),
                        status: PaymentStatus.Pending
                    );
                }

                payments.Add(payment);
            }

            await context.Payments.AddRangeAsync(payments);
            await context.SaveChangesAsync();
        }

        private static async Task SeedReviews(HavenlyDbContext context)
        {
            if (await context.Reviews.AnyAsync()) return;

            var users = await context.Users.ToListAsync();
            var userMap = users.ToDictionary(u => u.Name, u => u.Id);
            var properties = await context.Properties.ToListAsync();
            var propertyMap = properties.ToDictionary(p => p.PropertyName);

            var reviews = new List<Review>();

            void TryAddReview(string propName, string userName, int rating, string comment)
            {
                if (!propertyMap.TryGetValue(propName, out var prop) || !userMap.TryGetValue(userName, out var userId)) return;
                reviews.Add(new Review
                {
                    UserID = userId,
                    PropertyID = prop.PropertyID,
                    Rating = rating,
                    Comment = comment
                });
            }

            TryAddReview("Olive Ridge — Cliffside Villa with Infinity Pool", "Nadia Rahman", 5, "The photos undersell the view. Elena left a bottle of local wine and a hand-drawn map of the swimming coves. The pool is genuinely as good as it looks.");
            TryAddReview("Casa Fiora — Restored Stone Farmhouse", "Tom Bergman", 5, "Four adults and four kids and nobody felt crowded. The kitchen is well equipped and the drive down to Naoussa is only ten minutes.");
            TryAddReview("Olive Ridge — Cliffside Villa with Infinity Pool", "Yara Fahmy", 4, "Beautiful house and a very responsive host. The road up is steep — take a proper car, not a scooter.");
            TryAddReview("Casa Fiora — Restored Stone Farmhouse", "Chloe Deveraux", 5, "Giulia's breakfast baskets alone are worth the booking. We spent every evening on the terrace watching the light go over the valley.");

            if (reviews.Any())
            {
                await context.Reviews.AddRangeAsync(reviews);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedFavorites(HavenlyDbContext context)
        {
            if (await context.Favorites.AnyAsync()) return;

            var users = await context.Users.ToListAsync();
            var properties = await context.Properties.ToListAsync();
            var listings = await context.Listings.ToListAsync();

            var userMap = users.ToDictionary(u => u.Name, u => u.Id);
            var propertyToListing = properties
                .Join(listings, p => p.PropertyID, l => l.PropertyID, (p, l) => new { p.PropertyName, l.ListingID })
                .ToDictionary(x => x.PropertyName, x => x.ListingID);

            var testGuestId = userMap.ContainsKey("Test Guest") ? userMap["Test Guest"] : userMap.Values.First();

            var favorites = new List<Favorite>
            {
                new Favorite { UserID = testGuestId, ListingID = propertyToListing["Olive Ridge — Cliffside Villa with Infinity Pool"] },
                new Favorite { UserID = testGuestId, ListingID = propertyToListing["Casa Fiora — Restored Stone Farmhouse"] },
                new Favorite { UserID = testGuestId, ListingID = propertyToListing["North Loft — Bright Oak Apartment in the Old Town"] },
                new Favorite { UserID = userMap["Nadia Rahman"], ListingID = propertyToListing["Casa Fiora — Restored Stone Farmhouse"] },
                new Favorite { UserID = userMap["Nadia Rahman"], ListingID = propertyToListing["Pine Hollow — Glass Cabin in the Forest"] }
            };

            await context.Favorites.AddRangeAsync(favorites);
            await context.SaveChangesAsync();
        }
    }
}