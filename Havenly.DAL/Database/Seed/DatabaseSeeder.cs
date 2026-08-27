using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Database.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(HavenlyDbContext context)
        {
            // Check if data already exists
            if (await context.Users.AnyAsync())
                return;

            // Seed in the correct order to respect foreign key constraints
            await SeedAddresses(context);
            await SeedUsers(context);
            await SeedProperties(context);
            await SeedBedrooms(context);
            await SeedPropertyImages(context);
            await SeedListings(context);
            await SeedBookings(context);
            await SeedReviews(context);
            await SeedFavorites(context);

            await context.SaveChangesAsync();
        }

        private static async Task SeedAddresses(HavenlyDbContext context)
        {
            var addresses = new List<Address>
            {
               
                new Address { City = "Paros", Country = "Greece", Street = "Naoussa Bay" },
                new Address { City = "Copenhagen", Country = "Denmark", Street = "Nyhavn" },
                new Address { City = "Val d'Orcia", Country = "Italy", Street = "Pienza" },
                new Address { City = "Åre", Country = "Sweden", Street = "Björnänge" },
                new Address { City = "Comporta", Country = "Portugal", Street = "Carvalhal" },
                new Address { City = "Lisbon", Country = "Portugal", Street = "Príncipe Real" },
                new Address { City = "Cotswolds", Country = "United Kingdom", Street = "Stow-on-the-Wold" },
                new Address { City = "Menorca", Country = "Spain", Street = "Binibeca" }
            };

            await context.Addresses.AddRangeAsync(addresses);
            await context.SaveChangesAsync(); 
        }

        private static async Task SeedUsers(HavenlyDbContext context)
        {
            var users = new List<User>
            {
                
                new User
                {
                    Name = "Elena Marinos",
                    Email = "elena.marinos@havenly.co",
                    PasswordHash = "hashed_password_placeholder",
                    Role = "Host",
                    //CreatedAt = DateTime.Parse("2019-04-12"),
                    //IsActive = true
                },
                new User
                {
                    Name = "Nadia Rahman",
                    Email = "nadia.rahman@mail.com",
                    PasswordHash = "hashed_password_placeholder",
                    Role ="Guest",
                    //CreatedAt = DateTime.Parse("2024-02-03"),
                    //IsActive = true
                },
                new User
                {
                    Name = "Giulia Ferrari",
                    Email = "giulia@casafiora.it",
                    PasswordHash = "hashed_password_placeholder",
                    Role = "Host",
                    //CreatedAt = DateTime.Parse("2018-09-21"),
                    //IsActive = true
                },
                new User
                {
                    Name = "Tom Bergman",
                    Email = "t.bergman@mail.com",
                    PasswordHash = "hashed_password_placeholder",
                    Role = "Guest",
                    //CreatedAt = DateTime.Parse("2025-06-18"),
                    //IsActive = true
                },
                new User
                {
                    Name = "Rui Almeida",
                    Email = "rui.almeida@mail.com",
                    PasswordHash = "hashed_password_placeholder",
                    Role = "Host",
                    //CreatedAt = DateTime.Parse("2022-11-05"),
                    //IsActive = false // Suspended
                },
                new User
                {
                    Name = "Yara Fahmy",
                    Email = "yara.fahmy@mail.com",
                    PasswordHash = "hashed_password_placeholder",
                    Role = "Guest",
                    //CreatedAt = DateTime.Parse("2026-01-09"),
                    //IsActive = true
                },
                new User
                {
                    Name = "Mikkel Sørensen",
                    Email = "mikkel@northloft.dk",
                    PasswordHash = "hashed_password_placeholder",
                    Role = "Host",
                    //CreatedAt = DateTime.Parse("2021-03-30"),
                    //IsActive = true
                },
                new User
                {
                    Name = "Chloe Deveraux",
                    Email = "chloe.d@mail.com",
                    PasswordHash = "hashed_password_placeholder",
                    Role = "Guest",
                    //CreatedAt = DateTime.Parse("2026-08-10"),
                    //IsActive = true
                },
                new User
                {
                    Name = "Omar Khalil",
                    Email = "omar.khalil@havenly.co",
                    PasswordHash = "hashed_password_placeholder",
                    Role = "Admin",
                    //CreatedAt = DateTime.Parse("2023-05-02"),
                    //IsActive = true
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync(); 
        }

        private static async Task SeedProperties(HavenlyDbContext context)
        {
            // Get the users and addresses that were just created
            var users = await context.Users.ToListAsync();
            var addresses = await context.Addresses.ToListAsync();

            // Map users by name for easy lookup
            var userMap = users.ToDictionary(u => u.Name);
            var addressMap = addresses.ToDictionary(a => $"{a.City},{a.Country}");

            var properties = new List<Property>
            {
                // ✅ REMOVED PropertyID - let SQL Server auto-generate
                new Property
                {
                    OwnerUserID = userMap["Elena Marinos"].UserID,
                    AddressID = addressMap["Paros,Greece"].AddressID,
                    PropertyName = "Olive Ridge — Cliffside Villa with Infinity Pool",
                    Description = "Perched above Naoussa Bay, Olive Ridge pairs warm timber interiors with a 14-metre infinity pool that meets the horizon at sunset.",
                    NumberOfGuests = 8,
                    Capacity = 10,
                    BathroomCount = 5,
                    IsDeleted = false
                },
                new Property
                {
                    OwnerUserID = userMap["Mikkel Sørensen"].UserID,
                    AddressID = addressMap["Copenhagen,Denmark"].AddressID,
                    PropertyName = "North Loft — Bright Oak Apartment in the Old Town",
                    Description = "A calm two-bedroom loft two streets from the harbour. Herringbone oak floors, tall windows and a proper desk.",
                    NumberOfGuests = 4,
                    Capacity = 5,
                    BathroomCount = 2,
                    IsDeleted = false
                },
                new Property
                {
                    OwnerUserID = userMap["Giulia Ferrari"].UserID,
                    AddressID = addressMap["Val d'Orcia,Italy"].AddressID,
                    PropertyName = "Casa Fiora — Restored Stone Farmhouse",
                    Description = "Seventeenth-century stone, cypress avenue, and a kitchen built for long lunches.",
                    NumberOfGuests = 6,
                    Capacity = 8,
                    BathroomCount = 4,
                    IsDeleted = false
                },
                new Property
                {
                    OwnerUserID = userMap["Rui Almeida"].UserID,
                    AddressID = addressMap["Åre,Sweden"].AddressID,
                    PropertyName = "Pine Hollow — Glass Cabin in the Forest",
                    Description = "Floor-to-ceiling glass facing a wall of pines, a wood stove that heats the whole cabin.",
                    NumberOfGuests = 5,
                    Capacity = 6,
                    BathroomCount = 3,
                    IsDeleted = false
                },
                new Property
                {
                    OwnerUserID = userMap["Rui Almeida"].UserID,
                    AddressID = addressMap["Comporta,Portugal"].AddressID,
                    PropertyName = "Salt House — Beachfront Home with Open Terrace",
                    Description = "Two minutes of soft sand between the terrace and the Atlantic.",
                    NumberOfGuests = 7,
                    Capacity = 8,
                    BathroomCount = 4,
                    IsDeleted = false
                },
                new Property
                {
                    OwnerUserID = userMap["Omar Khalil"].UserID,
                    AddressID = addressMap["Lisbon,Portugal"].AddressID,
                    PropertyName = "Skyline Nine — Penthouse Terrace above the River",
                    Description = "A ninth-floor apartment with a wraparound terrace, a fire bowl, and the whole city glittering below after dark.",
                    NumberOfGuests = 4,
                    Capacity = 5,
                    BathroomCount = 2,
                    IsDeleted = false
                },
                new Property
                {
                    OwnerUserID = userMap["Omar Khalil"].UserID,
                    AddressID = addressMap["Cotswolds,United Kingdom"].AddressID,
                    PropertyName = "Barn Eleven — Converted Hay Barn with Beams",
                    Description = "Original oak trusses, exposed brick, and wool blankets on every bed.",
                    NumberOfGuests = 6,
                    Capacity = 7,
                    BathroomCount = 3,
                    IsDeleted = false
                },
                new Property
                {
                    OwnerUserID = userMap["Rui Almeida"].UserID,
                    AddressID = addressMap["Menorca,Spain"].AddressID,
                    PropertyName = "Cala Blanca — Village House with Blue Shutters",
                    Description = "A whitewashed fisherman's house on a quiet lane, bougainvillea over the door.",
                    NumberOfGuests = 4,
                    Capacity = 5,
                    BathroomCount = 3,
                    IsDeleted = false
                }
            };

            await context.Properties.AddRangeAsync(properties);
            await context.SaveChangesAsync(); 
        }

        private static async Task SeedBedrooms(HavenlyDbContext context)
        {
            // Get the properties that were just created
            var properties = await context.Properties.ToListAsync();
            var propertyMap = properties.ToDictionary(p => p.PropertyName);

            var bedrooms = new List<Bedroom>
            {
                // Property 1 - Olive Ridge (4 bedrooms)  
                new Bedroom { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, RoomName = "Master Suite", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, RoomName = "Garden Room", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, RoomName = "Twin Room", BedCount = 2 },
                new Bedroom { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, RoomName = "Bunk Room", BedCount = 2 },
                
                // Property 2 - North Loft (2 bedrooms)
                new Bedroom { PropertyID = propertyMap["North Loft — Bright Oak Apartment in the Old Town"].PropertyID, RoomName = "Main Bedroom", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["North Loft — Bright Oak Apartment in the Old Town"].PropertyID, RoomName = "Guest Room", BedCount = 1 },
                
                // Property 3 - Casa Fiora (3 bedrooms)
                new Bedroom { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, RoomName = "Master", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, RoomName = "Double Room", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, RoomName = "Twin Room", BedCount = 2 },
                
                // Property 4 - Pine Hollow (2 bedrooms)
                new Bedroom { PropertyID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID, RoomName = "Main", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID, RoomName = "Loft", BedCount = 1 },
                
                // Property 5 - Salt House (3 bedrooms)
                new Bedroom { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, RoomName = "Master", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, RoomName = "Double", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, RoomName = "Twin", BedCount = 2 },
                
                // Property 6 - Skyline Nine (2 bedrooms)
                new Bedroom { PropertyID = propertyMap["Skyline Nine — Penthouse Terrace above the River"].PropertyID, RoomName = "Master", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Skyline Nine — Penthouse Terrace above the River"].PropertyID, RoomName = "Guest", BedCount = 1 },
                
                // Property 7 - Barn Eleven (3 bedrooms)
                new Bedroom { PropertyID = propertyMap["Barn Eleven — Converted Hay Barn with Beams"].PropertyID, RoomName = "Master", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Barn Eleven — Converted Hay Barn with Beams"].PropertyID, RoomName = "Double", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Barn Eleven — Converted Hay Barn with Beams"].PropertyID, RoomName = "Twin", BedCount = 2 },
                
                // Property 8 - Cala Blanca (2 bedrooms)
                new Bedroom { PropertyID = propertyMap["Cala Blanca — Village House with Blue Shutters"].PropertyID, RoomName = "Main", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Cala Blanca — Village House with Blue Shutters"].PropertyID, RoomName = "Guest", BedCount = 1 }
            };

            await context.Bedrooms.AddRangeAsync(bedrooms);
            await context.SaveChangesAsync(); 
        }

        private static async Task SeedPropertyImages(HavenlyDbContext context)
        {
            // Get the properties that were just created
            var properties = await context.Properties.ToListAsync();
            var propertyMap = properties.ToDictionary(p => p.PropertyName);

            var images = new List<PropertyImage>
            {
                // Olive Ridge
                
                new PropertyImage { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, ImagePath = "/images/hero.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, ImagePath = "/images/p6.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, ImagePath = "/images/p4.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, ImagePath = "/images/p1.jpg", IsPrimary = false },
                                                            
                // North Loft                                  
                new PropertyImage { PropertyID = propertyMap["North Loft — Bright Oak Apartment in the Old Town"].PropertyID, ImagePath = "/images/p1.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["North Loft — Bright Oak Apartment in the Old Town"].PropertyID, ImagePath = "/images/p7.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["North Loft — Bright Oak Apartment in the Old Town"].PropertyID, ImagePath = "/images/p5.jpg", IsPrimary = false },
                                                            
                // Casa Fiora                                  
                new PropertyImage { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, ImagePath = "/images/p2.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, ImagePath = "/images/p7.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, ImagePath = "/images/p4.jpg", IsPrimary = false },
                                                            
                // Pine Hollow                                 
                new PropertyImage { PropertyID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID, ImagePath = "/images/p3.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID, ImagePath = "/images/p7.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID, ImagePath = "/images/p1.jpg", IsPrimary = false },
                                                            
                // Salt House                                  
                new PropertyImage { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, ImagePath = "/images/p4.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, ImagePath = "/images/p6.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, ImagePath = "/images/p2.jpg", IsPrimary = false },
                                                            
                // Skyline Nine                                
                new PropertyImage { PropertyID = propertyMap["Skyline Nine — Penthouse Terrace above the River"].PropertyID, ImagePath = "/images/p5.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Skyline Nine — Penthouse Terrace above the River"].PropertyID, ImagePath = "/images/p1.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Skyline Nine — Penthouse Terrace above the River"].PropertyID, ImagePath = "/images/p4.jpg", IsPrimary = false },
                                                            
                // Barn Eleven                                 
                new PropertyImage { PropertyID = propertyMap["Barn Eleven — Converted Hay Barn with Beams"].PropertyID, ImagePath = "/images/p7.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Barn Eleven — Converted Hay Barn with Beams"].PropertyID, ImagePath = "/images/p2.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Barn Eleven — Converted Hay Barn with Beams"].PropertyID, ImagePath = "/images/p3.jpg", IsPrimary = false },
                                                            
                // Cala Blanca                                 
                new PropertyImage { PropertyID = propertyMap["Cala Blanca — Village House with Blue Shutters"].PropertyID, ImagePath = "/images/p6.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Cala Blanca — Village House with Blue Shutters"].PropertyID, ImagePath = "/images/p4.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Cala Blanca — Village House with Blue Shutters"].PropertyID, ImagePath = "/images/p2.jpg", IsPrimary = false }
            };

            await context.PropertyImages.AddRangeAsync(images);
            await context.SaveChangesAsync();
        }

        private static async Task SeedListings(HavenlyDbContext context)
        {
            // Get the properties that were just created
            var properties = await context.Properties.ToListAsync();
            var propertyMap = properties.ToDictionary(p => p.PropertyName);

            var listings = new List<Listing>
            {
              
                new Listing { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, Description = "Cliffside villa with infinity pool", Price = 340, IsValid = true },
                new Listing { PropertyID = propertyMap["North Loft — Bright Oak Apartment in the Old Town"].PropertyID, Description = "Bright oak apartment in old town", Price = 165, IsValid = true },
                new Listing { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, Description = "Restored stone farmhouse", Price = 220, IsValid = true },
                new Listing { PropertyID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID, Description = "Glass cabin in the forest", Price = 275, IsValid = true },
                new Listing { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, Description = "Beachfront home with open terrace", Price = 295, IsValid = true },
                new Listing { PropertyID = propertyMap["Skyline Nine — Penthouse Terrace above the River"].PropertyID, Description = "Penthouse terrace above the river", Price = 410, IsValid = false },
                new Listing { PropertyID = propertyMap["Barn Eleven — Converted Hay Barn with Beams"].PropertyID, Description = "Converted hay barn with beams", Price = 190, IsValid = true },
                new Listing { PropertyID = propertyMap["Cala Blanca — Village House with Blue Shutters"].PropertyID, Description = "Village house with blue shutters", Price = 145, IsValid = false }
            };

            await context.Listings.AddRangeAsync(listings);
            await context.SaveChangesAsync();
        }

        private static async Task SeedBookings(HavenlyDbContext context)
        {
            // Get the users and listings that were just created
            var users = await context.Users.ToListAsync();
            var listings = await context.Listings.ToListAsync();

            var userMap = users.ToDictionary(u => u.Name);
            var listingMap = listings.ToDictionary(l => l.Description);

            var bookings = new List<Booking>
            {
                new Booking
                {
                    GuestUserID = userMap["Nadia Rahman"].UserID,
                    ListingID = listingMap["Cliffside villa with infinity pool"].ListingID,
                    CheckIn = DateTime.Parse("2026-09-04"),
                    CheckOut = DateTime.Parse("2026-09-10"),
                    TotalPrice = 2196,
                    Status = BookingStatus.Approved
                },
                new Booking
                {
                    GuestUserID = userMap["Tom Bergman"].UserID,
                    ListingID = listingMap["Glass cabin in the forest"].ListingID,
                    CheckIn = DateTime.Parse("2026-08-20"),
                    CheckOut = DateTime.Parse("2026-08-23"),
                    TotalPrice = 897,
                    Status = BookingStatus.Pending
                },
                new Booking
                {
                    GuestUserID = userMap["Yara Fahmy"].UserID,
                    ListingID = listingMap["Bright oak apartment in old town"].ListingID,
                    CheckIn = DateTime.Parse("2026-06-11"),
                    CheckOut = DateTime.Parse("2026-06-15"),
                    TotalPrice = 712,
                    Status = BookingStatus.Completed
                },
                new Booking
                {
                    GuestUserID = userMap["Tom Bergman"].UserID,
                    ListingID = listingMap["Restored stone farmhouse"].ListingID,
                    CheckIn = DateTime.Parse("2026-05-02"),
                    CheckOut = DateTime.Parse("2026-05-07"),
                    TotalPrice = 1188,
                    Status = BookingStatus.Completed
                },
                new Booking
                {
                    GuestUserID = userMap["Yara Fahmy"].UserID,
                    ListingID = listingMap["Beachfront home with open terrace"].ListingID,
                    CheckIn = DateTime.Parse("2026-09-01"),
                    CheckOut = DateTime.Parse("2026-09-03"),
                    TotalPrice = 654,
                    Status = BookingStatus.Cancelled
                },
                new Booking
                {
                    GuestUserID = userMap["Chloe Deveraux"].UserID,
                    ListingID = listingMap["Converted hay barn with beams"].ListingID,
                    CheckIn = DateTime.Parse("2026-08-27"),
                    CheckOut = DateTime.Parse("2026-08-31"),
                    TotalPrice = 836,
                    Status = BookingStatus.Pending
                }
            };

            await context.Bookings.AddRangeAsync(bookings);
            await context.SaveChangesAsync(); 
        }

        private static async Task SeedReviews(HavenlyDbContext context)
        {
            // Get the users and bookings that were just created
            var users = await context.Users.ToListAsync();
            var bookings = await context.Bookings.ToListAsync();

            var userMap = users.ToDictionary(u => u.Name);
            var bookingMap = new Dictionary<string, Booking>();

            foreach (var booking in bookings)
            {
                var guest = await context.Users.FindAsync(booking.GuestUserID);
                var listing = await context.Listings.FindAsync(booking.ListingID);
                if (guest != null && listing != null)
                {
                    var key = $"{guest.Name}_{listing.Description}";
                    bookingMap[key] = booking;
                }
            }

            var reviews = new List<Review>
    {
        new Review
        {
            UserID = userMap["Nadia Rahman"].UserID, 
            BookingID = bookingMap["Nadia Rahman_Cliffside villa with infinity pool"].BookingID,
            Rating = 5,
            Comment = "The photos undersell the view. Elena left a bottle of local wine and a hand-drawn map of the swimming coves. The pool is genuinely as good as it looks.",
            //CreatedAt = DateTime.Parse("2026-07-15"),
           
        },
        new Review
        {
            UserID = userMap["Tom Bergman"].UserID, 
            BookingID = bookingMap["Tom Bergman_Restored stone farmhouse"].BookingID,
            Rating = 5,
            Comment = "Four adults and four kids and nobody felt crowded. The kitchen is well equipped and the drive down to Naoussa is only ten minutes.",
            //CreatedAt = DateTime.Parse("2026-06-01"),
           
        },
       
        
        new Review
        {
            UserID = userMap["Yara Fahmy"].UserID, // ← Changed to Yara
            BookingID = bookingMap["Nadia Rahman_Cliffside villa with infinity pool"].BookingID,
            Rating = 4,
            Comment = "Beautiful house and a very responsive host. The road up is steep — take a proper car, not a scooter.",
            
        },
        

        new Review
        {
            UserID = userMap["Chloe Deveraux"].UserID, // ← Changed to Chloe
            BookingID = bookingMap["Tom Bergman_Restored stone farmhouse"].BookingID,
            Rating = 5,
            Comment = "Giulia's breakfast baskets alone are worth the booking. We spent every evening on the terrace watching the light go over the valley.",
            
        }
    };

            await context.Reviews.AddRangeAsync(reviews);
            await context.SaveChangesAsync(); 
        }
        private static async Task SeedFavorites(HavenlyDbContext context)
        {
            // Get the users and properties that were just created
            var users = await context.Users.ToListAsync();
            var properties = await context.Properties.ToListAsync();

            var userMap = users.ToDictionary(u => u.Name);
            var propertyMap = properties.ToDictionary(p => p.PropertyName);

            var favorites = new List<Favorite>
            {
                
                new Favorite { UserID = userMap["Nadia Rahman"].UserID, ListingID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID },
                new Favorite { UserID = userMap["Nadia Rahman"].UserID, ListingID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID },
                new Favorite { UserID = userMap["Tom Bergman"].UserID, ListingID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID },
                new Favorite { UserID = userMap["Yara Fahmy"].UserID, ListingID = propertyMap["North Loft — Bright Oak Apartment in the Old Town"].PropertyID },
                new Favorite { UserID = userMap["Chloe Deveraux"].UserID, ListingID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID }
            };

            await context.Favorites.AddRangeAsync(favorites);
            
        }
    }
}