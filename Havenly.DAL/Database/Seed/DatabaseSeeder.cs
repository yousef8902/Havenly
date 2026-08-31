
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

            // Check if users already exist
            if (await context.Users.AnyAsync())
                return;

            // Seed in correct order to respect foreign key constraints
            await SeedAddresses(context);
            await SeedUsersAsync(userManager);
            await SeedProperties(context);
            await SeedBedrooms(context);
            await SeedPropertyImages(context);
            await SeedListings(context);
            //await SeedBookings(context);
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
                // Provide latitude/longitude for the new non-nullable columns
                new Address { City = "Paros", Country = "Greece", Street = "Naoussa Bay", Latitude = 37.085000m, Longitude = 25.131000m },
                new Address { City = "Copenhagen", Country = "Denmark", Street = "Nyhavn", Latitude = 55.676100m, Longitude = 12.568300m },
                new Address { City = "Val d'Orcia", Country = "Italy", Street = "Pienza", Latitude = 43.066700m, Longitude = 11.633300m },
                new Address { City = "Åre", Country = "Sweden", Street = "Björnänge", Latitude = 63.398000m, Longitude = 13.096000m },
                new Address { City = "Comporta", Country = "Portugal", Street = "Carvalhal", Latitude = 38.354700m, Longitude = -8.771700m },
                new Address { City = "Lisbon", Country = "Portugal", Street = "Príncipe Real", Latitude = 38.722300m, Longitude = -9.139300m },
                new Address { City = "Cotswolds", Country = "United Kingdom", Street = "Stow-on-the-Wold", Latitude = 51.871900m, Longitude = -1.783200m },
                new Address { City = "Menorca", Country = "Spain", Street = "Binibeca", Latitude = 39.954900m, Longitude = 4.123100m }
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
                        Console.WriteLine($" User '{item.Email}' already exists, skipping...");
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

                    if (string.IsNullOrEmpty(user.Email))
                    {
                        Console.WriteLine($"Email is null or empty for user '{item.Name}'");
                        continue;
                    }

                    if (string.IsNullOrEmpty(user.UserName))
                    {
                        Console.WriteLine($" UserName is null or empty for user '{item.Name}'");
                        continue;
                    }


                    Console.WriteLine($"Creating user: {item.Name} ({item.Email})");

                    var result = await userManager.CreateAsync(user, "P@ssword123!");

                    if (result.Succeeded)
                    {
                       
                        var roleResult = await userManager.AddToRoleAsync(user, item.Role);
                        if (roleResult.Succeeded)
                        {
                            Console.WriteLine($" Created user: {item.Name} ({item.Email}) with role '{item.Role}'");
                        }
                        else
                        {
                            var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                            Console.WriteLine($" User '{item.Name}' created but failed to add role: {roleErrors}");
                        }
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        Console.WriteLine($" Failed to create user '{item.Name}': {errors}");

                       
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"   - {error.Code}: {error.Description}");
                        }

                        throw new Exception($"Failed to create user '{item.Name}': {errors}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Exception creating user '{item.Name}': {ex.Message}");
                    throw;
                }
            }
        }

        private static async Task SeedProperties(HavenlyDbContext context)
        {
            var users = await context.Users.ToListAsync();
            var addresses = await context.Addresses.ToListAsync();

            var userMap = users.ToDictionary(u => u.Name, u => u.Id);
            var addressMap = addresses.ToDictionary(a => $"{a.City},{a.Country}", a => a.AddressID);

            var properties = new List<Property>
            {
                new Property
                {
                    OwnerUserID = userMap["Elena Marinos"],
                    AddressID = addressMap["Paros,Greece"],
                    PropertyName = "Olive Ridge — Cliffside Villa with Infinity Pool",
                    Description = "Perched above Naoussa Bay, Olive Ridge pairs warm timber interiors with a 14-metre infinity pool that meets the horizon at sunset.",
                    NumberOfGuests = 8,
                    Capacity = 10,
                    BathroomCount = 5,
                    IsDeleted = false
                },
                new Property
                {
                    OwnerUserID = userMap["Mikkel Sørensen"],
                    AddressID = addressMap["Copenhagen,Denmark"],
                    PropertyName = "North Loft — Bright Oak Apartment in the Old Town",
                    Description = "A calm two-bedroom loft two streets from the harbour. Herringbone oak floors, tall windows and a proper desk.",
                    NumberOfGuests = 4,
                    Capacity = 5,
                    BathroomCount = 2,
                    IsDeleted = false
                },
                new Property
                {
                    OwnerUserID = userMap["Giulia Ferrari"],
                    AddressID = addressMap["Val d'Orcia,Italy"],
                    PropertyName = "Casa Fiora — Restored Stone Farmhouse",
                    Description = "Seventeenth-century stone, cypress avenue, and a kitchen built for long lunches.",
                    NumberOfGuests = 6,
                    Capacity = 8,
                    BathroomCount = 4,
                    IsDeleted = false
                },
                new Property
                {
                    OwnerUserID = userMap["Rui Almeida"],
                    AddressID = addressMap["Åre,Sweden"],
                    PropertyName = "Pine Hollow — Glass Cabin in the Forest",
                    Description = "Floor-to-ceiling glass facing a wall of pines, a wood stove that heats the whole cabin.",
                    NumberOfGuests = 5,
                    Capacity = 6,
                    BathroomCount = 3,
                    IsDeleted = false
                },
                new Property
                {
                    OwnerUserID = userMap["Rui Almeida"],
                    AddressID = addressMap["Comporta,Portugal"],
                    PropertyName = "Salt House — Beachfront Home with Open Terrace",
                    Description = "Two minutes of soft sand between the terrace and the Atlantic.",
                    NumberOfGuests = 7,
                    Capacity = 8,
                    BathroomCount = 4,
                    IsDeleted = false
                },
                new Property
                {
                    OwnerUserID = userMap["Omar Khalil"],
                    AddressID = addressMap["Lisbon,Portugal"],
                    PropertyName = "Skyline Nine — Penthouse Terrace above the River",
                    Description = "A ninth-floor apartment with a wraparound terrace, a fire bowl, and the whole city glittering below after dark.",
                    NumberOfGuests = 4,
                    Capacity = 5,
                    BathroomCount = 2,
                    IsDeleted = false
                },
                new Property
                {
                    OwnerUserID = userMap["Omar Khalil"],
                    AddressID = addressMap["Cotswolds,United Kingdom"],
                    PropertyName = "Barn Eleven — Converted Hay Barn with Beams",
                    Description = "Original oak trusses, exposed brick, and wool blankets on every bed.",
                    NumberOfGuests = 6,
                    Capacity = 7,
                    BathroomCount = 3,
                    IsDeleted = false
                },
                new Property
                {
                    OwnerUserID = userMap["Rui Almeida"],
                    AddressID = addressMap["Menorca,Spain"],
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
            var properties = await context.Properties.ToListAsync();
            var propertyMap = properties.ToDictionary(p => p.PropertyName);

            var bedrooms = new List<Bedroom>
            {
                new Bedroom { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, RoomName = "Master Suite", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, RoomName = "Garden Room", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, RoomName = "Twin Room", BedCount = 2 },
                new Bedroom { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, RoomName = "Bunk Room", BedCount = 2 },

                new Bedroom { PropertyID = propertyMap["North Loft — Bright Oak Apartment in the Old Town"].PropertyID, RoomName = "Main Bedroom", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["North Loft — Bright Oak Apartment in the Old Town"].PropertyID, RoomName = "Guest Room", BedCount = 1 },

                new Bedroom { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, RoomName = "Master", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, RoomName = "Double Room", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, RoomName = "Twin Room", BedCount = 2 },

                new Bedroom { PropertyID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID, RoomName = "Main", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID, RoomName = "Loft", BedCount = 1 },

                new Bedroom { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, RoomName = "Master", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, RoomName = "Double", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, RoomName = "Twin", BedCount = 2 },

                new Bedroom { PropertyID = propertyMap["Skyline Nine — Penthouse Terrace above the River"].PropertyID, RoomName = "Master", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Skyline Nine — Penthouse Terrace above the River"].PropertyID, RoomName = "Guest", BedCount = 1 },

                new Bedroom { PropertyID = propertyMap["Barn Eleven — Converted Hay Barn with Beams"].PropertyID, RoomName = "Master", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Barn Eleven — Converted Hay Barn with Beams"].PropertyID, RoomName = "Double", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Barn Eleven — Converted Hay Barn with Beams"].PropertyID, RoomName = "Twin", BedCount = 2 },

                new Bedroom { PropertyID = propertyMap["Cala Blanca — Village House with Blue Shutters"].PropertyID, RoomName = "Main", BedCount = 1 },
                new Bedroom { PropertyID = propertyMap["Cala Blanca — Village House with Blue Shutters"].PropertyID, RoomName = "Guest", BedCount = 1 }
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
                new PropertyImage { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, ImagePath = "/images/hero.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, ImagePath = "/images/p6.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, ImagePath = "/images/p4.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Olive Ridge — Cliffside Villa with Infinity Pool"].PropertyID, ImagePath = "/images/p1.jpg", IsPrimary = false },

                new PropertyImage { PropertyID = propertyMap["North Loft — Bright Oak Apartment in the Old Town"].PropertyID, ImagePath = "/images/p1.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["North Loft — Bright Oak Apartment in the Old Town"].PropertyID, ImagePath = "/images/p7.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["North Loft — Bright Oak Apartment in the Old Town"].PropertyID, ImagePath = "/images/p5.jpg", IsPrimary = false },

                new PropertyImage { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, ImagePath = "/images/p2.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, ImagePath = "/images/p7.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Casa Fiora — Restored Stone Farmhouse"].PropertyID, ImagePath = "/images/p4.jpg", IsPrimary = false },

                new PropertyImage { PropertyID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID, ImagePath = "/images/p3.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID, ImagePath = "/images/p7.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Pine Hollow — Glass Cabin in the Forest"].PropertyID, ImagePath = "/images/p1.jpg", IsPrimary = false },

                new PropertyImage { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, ImagePath = "/images/p4.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, ImagePath = "/images/p6.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Salt House — Beachfront Home with Open Terrace"].PropertyID, ImagePath = "/images/p2.jpg", IsPrimary = false },

                new PropertyImage { PropertyID = propertyMap["Skyline Nine — Penthouse Terrace above the River"].PropertyID, ImagePath = "/images/p5.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Skyline Nine — Penthouse Terrace above the River"].PropertyID, ImagePath = "/images/p1.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Skyline Nine — Penthouse Terrace above the River"].PropertyID, ImagePath = "/images/p4.jpg", IsPrimary = false },

                new PropertyImage { PropertyID = propertyMap["Barn Eleven — Converted Hay Barn with Beams"].PropertyID, ImagePath = "/images/p7.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Barn Eleven — Converted Hay Barn with Beams"].PropertyID, ImagePath = "/images/p2.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Barn Eleven — Converted Hay Barn with Beams"].PropertyID, ImagePath = "/images/p3.jpg", IsPrimary = false },

                new PropertyImage { PropertyID = propertyMap["Cala Blanca — Village House with Blue Shutters"].PropertyID, ImagePath = "/images/p6.jpg", IsPrimary = true },
                new PropertyImage { PropertyID = propertyMap["Cala Blanca — Village House with Blue Shutters"].PropertyID, ImagePath = "/images/p4.jpg", IsPrimary = false },
                new PropertyImage { PropertyID = propertyMap["Cala Blanca — Village House with Blue Shutters"].PropertyID, ImagePath = "/images/p2.jpg", IsPrimary = false }
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

        //private static async Task SeedBookings(HavenlyDbContext context)
        //{
        //    var users = await context.Users.ToListAsync();
        //    var listings = await context.Listings.ToListAsync();

        //    var userMap = users.ToDictionary(u => u.Name, u => u.Id);
        //    var listingMap = listings.ToDictionary(l => l.Description);

        //    var bookings = new List<Booking>
        //    {
        //        new Booking
        //        {
        //            GuestUserID = userMap["Nadia Rahman"],
        //            ListingID = listingMap["Cliffside villa with infinity pool"].ListingID,
        //            CheckIn = DateTime.Parse("2026-09-04"),
        //            CheckOut = DateTime.Parse("2026-09-10"),
        //            TotalPrice = 2196,
        //            Status = BookingStatus.Approved
        //        },
        //        new Booking
        //        {
        //            GuestUserID = userMap["Tom Bergman"],
        //            ListingID = listingMap["Glass cabin in the forest"].ListingID,
        //            CheckIn = DateTime.Parse("2026-08-20"),
        //            CheckOut = DateTime.Parse("2026-08-23"),
        //            TotalPrice = 897,
        //            Status = BookingStatus.Pending
        //        },
        //        new Booking
        //        {
        //            GuestUserID = userMap["Yara Fahmy"],
        //            ListingID = listingMap["Bright oak apartment in old town"].ListingID,
        //            CheckIn = DateTime.Parse("2026-06-11"),
        //            CheckOut = DateTime.Parse("2026-06-15"),
        //            TotalPrice = 712,
        //            Status = BookingStatus.Completed
        //        },
        //        new Booking
        //        {
        //            GuestUserID = userMap["Tom Bergman"],
        //            ListingID = listingMap["Restored stone farmhouse"].ListingID,
        //            CheckIn = DateTime.Parse("2026-05-02"),
        //            CheckOut = DateTime.Parse("2026-05-07"),
        //            TotalPrice = 1188,
        //            Status = BookingStatus.Completed
        //        },
        //        new Booking
        //        {
        //            GuestUserID = userMap["Yara Fahmy"],
        //            ListingID = listingMap["Beachfront home with open terrace"].ListingID,
        //            CheckIn = DateTime.Parse("2026-09-01"),
        //            CheckOut = DateTime.Parse("2026-09-03"),
        //            TotalPrice = 654,
        //            Status = BookingStatus.Cancelled
        //        },
        //        new Booking
        //        {
        //            GuestUserID = userMap["Chloe Deveraux"],
        //            ListingID = listingMap["Converted hay barn with beams"].ListingID,
        //            CheckIn = DateTime.Parse("2026-08-27"),
        //            CheckOut = DateTime.Parse("2026-08-31"),
        //            TotalPrice = 836,
        //            Status = BookingStatus.Pending
        //        }
        //    };

        //    await context.Bookings.AddRangeAsync(bookings);
        //    await context.SaveChangesAsync();
        //}

        private static async Task SeedReviews(HavenlyDbContext context)
        {
            var users = await context.Users.ToListAsync();
            var bookings = await context.Bookings.ToListAsync();

            var userMap = users.ToDictionary(u => u.Name, u => u.Id);
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
            // The Review entity now associates with PropertyID (not BookingID).
            // Build a lookup of listings to resolve property IDs for the reviews.
            var listings = await context.Listings.ToListAsync();

            var reviews = new List<Review>();

            void TryAddReview(string bookingKey, string userName, int rating, string comment)
            {
                if (!bookingMap.TryGetValue(bookingKey, out var booking)) return;
                var listing = listings.FirstOrDefault(l => l.ListingID == booking.ListingID);
                if (listing == null) return;
                reviews.Add(new Review
                {
                    UserID = userMap[userName],
                    PropertyID = listing.PropertyID,
                    Rating = rating,
                    Comment = comment
                });
            }

            TryAddReview("Nadia Rahman_Cliffside villa with infinity pool", "Nadia Rahman", 5, "The photos undersell the view. Elena left a bottle of local wine and a hand-drawn map of the swimming coves. The pool is genuinely as good as it looks.");
            TryAddReview("Tom Bergman_Restored stone farmhouse", "Tom Bergman", 5, "Four adults and four kids and nobody felt crowded. The kitchen is well equipped and the drive down to Naoussa is only ten minutes.");
            TryAddReview("Nadia Rahman_Cliffside villa with infinity pool", "Yara Fahmy", 4, "Beautiful house and a very responsive host. The road up is steep — take a proper car, not a scooter.");
            TryAddReview("Tom Bergman_Restored stone farmhouse", "Chloe Deveraux", 5, "Giulia's breakfast baskets alone are worth the booking. We spent every evening on the terrace watching the light go over the valley.");

            if (reviews.Any())
            {
                await context.Reviews.AddRangeAsync(reviews);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedFavorites(HavenlyDbContext context)
        {
            var users = await context.Users.ToListAsync();
            var properties = await context.Properties.ToListAsync();
            var listings = await context.Listings.ToListAsync();

            var userMap = users.ToDictionary(u => u.Name, u => u.Id);

            // Map property name -> listing id (many-to-one: a listing belongs to a property)
            var propertyToListing = properties
                .Join(listings, p => p.PropertyID, l => l.PropertyID, (p, l) => new { p.PropertyName, l.ListingID })
                .ToDictionary(x => x.PropertyName, x => x.ListingID);

            var favorites = new List<Favorite>
            {
                new Favorite { UserID = userMap["Nadia Rahman"], ListingID = propertyToListing["Casa Fiora — Restored Stone Farmhouse"] },
                new Favorite { UserID = userMap["Nadia Rahman"], ListingID = propertyToListing["Pine Hollow — Glass Cabin in the Forest"] },
                new Favorite { UserID = userMap["Tom Bergman"], ListingID = propertyToListing["Olive Ridge — Cliffside Villa with Infinity Pool"] },
                new Favorite { UserID = userMap["Yara Fahmy"], ListingID = propertyToListing["North Loft — Bright Oak Apartment in the Old Town"] },
                new Favorite { UserID = userMap["Chloe Deveraux"], ListingID = propertyToListing["Salt House — Beachfront Home with Open Terrace"] }
            };

            await context.Favorites.AddRangeAsync(favorites);
            await context.SaveChangesAsync();
        }
    }
}