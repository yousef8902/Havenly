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
            // ============================================================
            // 1. ROLES
            // ============================================================
            await SeedRolesAsync(roleManager);

            // ============================================================
            // 2. DATABASE COMPATIBILITY COLUMNS
            // ============================================================
            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF COL_LENGTH('Properties', 'Category') IS NULL
                        ALTER TABLE Properties ADD Category nvarchar(100) NULL;

                    IF COL_LENGTH('AspNetUsers', 'ProfilePictureUrl') IS NULL
                        ALTER TABLE AspNetUsers ADD ProfilePictureUrl nvarchar(500) NULL;

                    IF COL_LENGTH('AspNetUsers', 'Bio') IS NULL
                        ALTER TABLE AspNetUsers ADD Bio nvarchar(max) NULL;

                    IF COL_LENGTH('AspNetUsers', 'JoinedDate') IS NULL
                        ALTER TABLE AspNetUsers ADD JoinedDate datetime2 NOT NULL
                            CONSTRAINT DF_AspNetUsers_JoinedDate DEFAULT GETUTCDATE();

                    IF COL_LENGTH('Bookings', 'CreatedDate') IS NULL
                        ALTER TABLE Bookings ADD CreatedDate datetime2 NOT NULL
                            CONSTRAINT DF_Bookings_CreatedDate DEFAULT GETUTCDATE();
                ");
            }
            catch
            {
                // Database may already contain these columns.
            }

            // ============================================================
            // 3. ADDRESSES
            // ============================================================
            await SeedAddresses(context);

            // ============================================================
            // 4. USERS
            // ============================================================
            await SeedUsersAsync(userManager);

            // ============================================================
            // 5. PROPERTIES
            // ============================================================
            await SeedProperties(context);

            // ============================================================
            // 6. BEDROOMS
            // ============================================================
            await SeedBedrooms(context);

            // ============================================================
            // 7. PROPERTY IMAGES
            // ============================================================
            await SeedPropertyImages(context);

            // ============================================================
            // 8. LISTINGS
            // ============================================================
            await SeedListings(context);

            // ============================================================
            // 9. AMENITIES
            // ============================================================
            await SeedAmenities(context);

            // ============================================================
            // 10. PROPERTY AMENITIES
            // ============================================================
            await SeedPropertyAmenities(context);

            // ============================================================
            // 11. RATINGS / CATEGORIES
            // ============================================================
            await UpdatePropertyRatingsAndCategories(context);

            // ============================================================
            // 12. APPROVE ALL SEEDED LISTINGS
            // ============================================================
            var listingsToApprove = await context.Listings.ToListAsync();

            foreach (var listing in listingsToApprove)
            {
                listing.IsValid = true;

                if (listing.ListingStatus != ListingStatus.Approved)
                {
                    listing.Approve();
                }
            }

            await context.SaveChangesAsync();

            // ============================================================
            // 13. BOOKINGS
            // ============================================================
            await SeedBookings(context);

            // ============================================================
            // 14. PAYMENTS
            // ============================================================
            await SeedPayments(context);

            // ============================================================
            // 15. REVIEWS
            // ============================================================
            await SeedReviews(context);

            // ============================================================
            // 16. FAVORITES
            // ============================================================
            await SeedFavorites(context);

            await context.SaveChangesAsync();
        }

        // ================================================================
        // ROLES
        // ================================================================

        private static async Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles =
            {
                UserRoles.Admin,
                UserRoles.Host,
                UserRoles.Guest
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }
        }

        // ================================================================
        // ADDRESSES
        // ================================================================

        private static async Task SeedAddresses(
            HavenlyDbContext context)
        {
            var addresses = new[]
            {
                new
                {
                    City = "El Gouna",
                    Street = "Tawila Island Lagoon, El Gouna",
                    Latitude = 27.3949m,
                    Longitude = 33.6766m
                },

                new
                {
                    City = "Dahab",
                    Street = "Lighthouse Reef Bay, Dahab",
                    Latitude = 28.5095m,
                    Longitude = 34.5137m
                },

                new
                {
                    City = "Cairo",
                    Street = "Abou El Feda St, Zamalek",
                    Latitude = 30.0626m,
                    Longitude = 31.2197m
                },

                new
                {
                    City = "Siwa Oasis",
                    Street = "Aghurmi Salt Lake Valley, Siwa",
                    Latitude = 29.2032m,
                    Longitude = 25.5195m
                },

                new
                {
                    City = "North Coast",
                    Street = "Sidi Abdel Rahman Bay, Sahel",
                    Latitude = 30.9575m,
                    Longitude = 28.7180m
                },

                new
                {
                    City = "Fayoum",
                    Street = "Tunis Pottery Village, Lake Qarun",
                    Latitude = 29.4140m,
                    Longitude = 30.4900m
                },

                new
                {
                    City = "Luxor",
                    Street = "Al-Gharbi Nile West Bank, Luxor",
                    Latitude = 25.7202m,
                    Longitude = 32.6105m
                },

                new
                {
                    City = "Aswan",
                    Street = "Elephantine Island, Aswan",
                    Latitude = 24.0889m,
                    Longitude = 32.8998m
                },

                new
                {
                    City = "El Gouna",
                    Street = "Mangroovy Lagoon, El Gouna",
                    Latitude = 27.4070m,
                    Longitude = 33.6775m
                },

                new
                {
                    City = "Soma Bay",
                    Street = "Kite Beach Road, Soma Bay",
                    Latitude = 26.8490m,
                    Longitude = 33.9950m
                },

                new
                {
                    City = "Dahab",
                    Street = "Blue Hole Road, Dahab",
                    Latitude = 28.5735m,
                    Longitude = 34.5367m
                },

                new
                {
                    City = "Ain Sokhna",
                    Street = "Azha Coastal Road, Ain Sokhna",
                    Latitude = 29.5625m,
                    Longitude = 32.6540m
                },

                new
                {
                    City = "Sharm El Sheikh",
                    Street = "Nabq Desert Edge, Sharm El Sheikh",
                    Latitude = 28.0220m,
                    Longitude = 34.4370m
                },

                new
                {
                    City = "Makadi Bay",
                    Street = "Makadi Bay Palm District",
                    Latitude = 26.9870m,
                    Longitude = 33.9080m
                },

                new
                {
                    City = "Cairo",
                    Street = "Corniche El Nil, Garden City",
                    Latitude = 30.0366m,
                    Longitude = 31.2296m
                },

                new
                {
                    City = "Cairo",
                    Street = "26th of July Corridor, Zamalek",
                    Latitude = 30.0605m,
                    Longitude = 31.2204m
                },

                new
                {
                    City = "Cairo",
                    Street = "Road 9, Maadi",
                    Latitude = 29.9602m,
                    Longitude = 31.2569m
                },

                new
                {
                    City = "Giza",
                    Street = "Al Haram Gardens, Giza",
                    Latitude = 29.9952m,
                    Longitude = 31.1402m
                },

                new
                {
                    City = "Cairo",
                    Street = "Al-Muizz Street, Gamaleya",
                    Latitude = 30.0488m,
                    Longitude = 31.2610m
                },

                new
                {
                    City = "Siwa Oasis",
                    Street = "Shali Oasis Road, Siwa",
                    Latitude = 29.2040m,
                    Longitude = 25.5197m
                },

                new
                {
                    City = "Siwa Oasis",
                    Street = "Fatnas Lake Road, Siwa",
                    Latitude = 29.1960m,
                    Longitude = 25.5360m
                },

                new
                {
                    City = "Siwa Oasis",
                    Street = "Ain Safi Road, Siwa",
                    Latitude = 29.1995m,
                    Longitude = 25.5265m
                },

                new
                {
                    City = "Fayoum",
                    Street = "Lake Qarun Shore, Fayoum",
                    Latitude = 29.4720m,
                    Longitude = 30.6300m
                },

                new
                {
                    City = "Fayoum",
                    Street = "Tunis Village Arts District",
                    Latitude = 29.4147m,
                    Longitude = 30.4892m
                },

                new
                {
                    City = "Fayoum",
                    Street = "Wadi El Hitan Desert Road",
                    Latitude = 29.3430m,
                    Longitude = 30.1770m
                },

                new
                {
                    City = "Luxor",
                    Street = "Nile Corniche, West Bank Luxor",
                    Latitude = 25.7115m,
                    Longitude = 32.6310m
                },

                new
                {
                    City = "Luxor",
                    Street = "Theban Foothills, Sheikh Abd el-Qurna",
                    Latitude = 25.7400m,
                    Longitude = 32.6005m
                },

                new
                {
                    City = "Luxor",
                    Street = "Corniche El Nile, Luxor",
                    Latitude = 25.6872m,
                    Longitude = 32.6396m
                },

                new
                {
                    City = "Aswan",
                    Street = "Elephantine Island North Village",
                    Latitude = 24.0910m,
                    Longitude = 32.8985m
                },

                new
                {
                    City = "Aswan",
                    Street = "First Cataract Riverside, Aswan",
                    Latitude = 24.0750m,
                    Longitude = 32.8860m
                }
            };

            foreach (var item in addresses)
            {
                bool exists = await context.Addresses.AnyAsync(a =>
                    a.City == item.City &&
                    a.Street == item.Street);

                if (exists)
                    continue;

                context.Addresses.Add(new Address
                {
                    City = item.City,
                    Country = "Egypt",
                    Street = item.Street,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude
                });
            }

            await context.SaveChangesAsync();
        }

        // ================================================================
        // USERS
        // ================================================================

        private static async Task SeedUsersAsync(
            UserManager<User> userManager)
        {
            var users = new[]
            {
                new
                {
                    Email = "guest@test.com",
                    Name = "Test Guest",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "host@test.com",
                    Name = "Test Host",
                    Role = UserRoles.Host
                },

                new
                {
                    Email = "admin@test.com",
                    Name = "Test Admin",
                    Role = UserRoles.Admin
                },

                new
                {
                    Email = "elena.marinos@havenly.co",
                    Name = "Elena Marinos",
                    Role = UserRoles.Host
                },

                new
                {
                    Email = "nadia.rahman@mail.com",
                    Name = "Nadia Rahman",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "giulia@casafiora.it",
                    Name = "Giulia Ferrari",
                    Role = UserRoles.Host
                },

                new
                {
                    Email = "t.bergman@mail.com",
                    Name = "Tom Bergman",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "rui.almeida@mail.com",
                    Name = "Rui Almeida",
                    Role = UserRoles.Host
                },

                new
                {
                    Email = "yara.fahmy@mail.com",
                    Name = "Yara Fahmy",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "mikkel@northloft.dk",
                    Name = "Mikkel Sørensen",
                    Role = UserRoles.Host
                },

                new
                {
                    Email = "chloe.d@mail.com",
                    Name = "Chloe Deveraux",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "omar.khalil@havenly.co",
                    Name = "Omar Khalil",
                    Role = UserRoles.Admin
                },

                new
                {
                    Email = "karim.hassan@havenly.co",
                    Name = "Karim Hassan",
                    Role = UserRoles.Host
                },

                new
                {
                    Email = "salma.nassar@havenly.co",
                    Name = "Salma Nassar",
                    Role = UserRoles.Host
                },

                new
                {
                    Email = "adam.elmasry@havenly.co",
                    Name = "Adam El Masry",
                    Role = UserRoles.Host
                },

                new
                {
                    Email = "farah.adel@havenly.co",
                    Name = "Farah Adel",
                    Role = UserRoles.Host
                },

                new
                {
                    Email = "mariam.saleh@mail.com",
                    Name = "Mariam Saleh",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "ahmed.tarek@mail.com",
                    Name = "Ahmed Tarek",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "laila.mostafa@mail.com",
                    Name = "Laila Mostafa",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "daniel.george@mail.com",
                    Name = "Daniel George",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "hassan.elwakeel@mail.com",
                    Name = "Hassan El Wakeel",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "reem.fawzy@mail.com",
                    Name = "Reem Fawzy",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "mona.zaki@mail.com",
                    Name = "Mona Zaki",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "sherif.samir@mail.com",
                    Name = "Sherif Samir",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "hany.adel@mail.com",
                    Name = "Hany Adel",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "nouran.essam@mail.com",
                    Name = "Nouran Essam",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "kareem.sobhy@mail.com",
                    Name = "Kareem Sobhy",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "zeina.moussa@mail.com",
                    Name = "Zeina Moussa",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "tarek.allam@mail.com",
                    Name = "Tarek Allam",
                    Role = UserRoles.Guest
                },

                new
                {
                    Email = "salma.helmy@mail.com",
                    Name = "Salma Helmy",
                    Role = UserRoles.Guest
                }
            };

            int index = 0;

            foreach (var item in users)
            {
                var existingUser =
                    await userManager.FindByEmailAsync(item.Email);

                if (existingUser != null)
                {
                    // Distribute existing user's JoinedDate across the 12 months for rich analytics
                    if (existingUser.JoinedDate >= DateTime.UtcNow.AddDays(-2))
                    {
                        existingUser.JoinedDate = DateTime.UtcNow
                            .AddMonths(-11 + (index % 12))
                            .AddDays(2 + ((index * 3) % 25));
                        await userManager.UpdateAsync(existingUser);
                    }
                    index++;
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
                    AccessFailedCount = 0,
                    JoinedDate = DateTime.UtcNow
                        .AddMonths(-11 + (index % 12))
                        .AddDays(3 + ((index * 2) % 24))
                };

                var result = await userManager.CreateAsync(
                    user,
                    "P@ssword123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        user,
                        item.Role);
                }

                index++;
            }
        }

        // ================================================================
        // PROPERTIES - 30 EGYPTIAN PROPERTIES
        // ================================================================

        private static async Task SeedProperties(
            HavenlyDbContext context)
        {
            var addresses = await context.Addresses
                .ToListAsync();

            var addressMap = addresses
                .GroupBy(a => a.Street)
                .ToDictionary(g => g.Key, g => g.First().AddressID);

            var hosts = await context.Users
                .Where(u => u.Role == UserRoles.Host)
                .ToListAsync();

            if (!hosts.Any())
                return;

            // ------------------------------------------------------------
            // 1
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Villa Turquoise — Lagoon Beachfront Villa with Private Jetty",

                    Description =
                        "Nestled along the crystal-clear lagoons of El Gouna, this open-concept architectural villa features a private heated infinity pool, private lagoon beach access, and spacious sun decks.",

                    Category = "Beachfront",
                    NumberOfGuests = 6,
                    Capacity = 6,
                    BathroomCount = 3,
                    Rating = 4.97,
                    NumberOfReviews = 54
                },
                "Tawila Island Lagoon, El Gouna",
                0);

            // ------------------------------------------------------------
            // 2
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Blue Coral Sanctuary — Boho Beach House by the Reef",

                    Description =
                        "Right on the shores of the Gulf of Aqaba in Dahab, enjoy bohemian interiors with natural palm-leaf ceilings, direct reef snorkeling steps from your patio, and Sinai mountain sunset views.",

                    Category = "Islands",
                    NumberOfGuests = 4,
                    Capacity = 4,
                    BathroomCount = 2,
                    Rating = 4.88,
                    NumberOfReviews = 42
                },
                "Lighthouse Reef Bay, Dahab",
                1);

            // ------------------------------------------------------------
            // 3
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "The Nile View Loft — Modern Art Deco Penthouse in Zamalek",

                    Description =
                        "Perched high above leafy Zamalek with sweeping panoramic vistas of the River Nile and Cairo skyline. Designed with heritage parquet floors, contemporary art, and a wrap-around sunset balcony.",

                    Category = "City lofts",
                    NumberOfGuests = 3,
                    Capacity = 3,
                    BathroomCount = 2,
                    Rating = 4.92,
                    NumberOfReviews = 68
                },
                "Abou El Feda St, Zamalek",
                2);

            // ------------------------------------------------------------
            // 4
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Salt Lake Eco-Lodge — Kershef Adobe Chalet in Date Palms",

                    Description =
                        "Constructed entirely from authentic Siwan Kershef (salt rock and clay) amidst tranquil olive and date palm groves. Features private natural spring plunge pool and starry desert night skies.",

                    Category = "Cabins",
                    NumberOfGuests = 4,
                    Capacity = 4,
                    BathroomCount = 2,
                    Rating = 4.95,
                    NumberOfReviews = 31
                },
                "Aghurmi Salt Lake Valley, Siwa",
                3);

            // ------------------------------------------------------------
            // 5
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "The White Sands Manor — Mediterranean Beachfront Haven",

                    Description =
                        "Overlooking the crystal-clear turquoise waters and white sandy beaches of the North Coast (Sahel). Minimalist Mediterranean lines, expansive outdoor terrace, and private beach cabana.",

                    Category = "Beachfront",
                    NumberOfGuests = 8,
                    Capacity = 8,
                    BathroomCount = 4,
                    Rating = 4.98,
                    NumberOfReviews = 43
                },
                "Sidi Abdel Rahman Bay, Sahel",
                4);

            // ------------------------------------------------------------
            // 6
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Dar El Qamar — Artistic Country Farmhouse near Lake Qarun",

                    Description =
                        "Located in the famous potters' village of Tunis in Fayoum. Built with dome architecture, limestone walls, lush private gardens, and rooftop views overlooking Lake Qarun and desert dunes.",

                    Category = "Countryside",
                    NumberOfGuests = 4,
                    Capacity = 4,
                    BathroomCount = 2,
                    Rating = 4.75,
                    NumberOfReviews = 29
                },
                "Tunis Pottery Village, Lake Qarun",
                5);

            // ------------------------------------------------------------
            // 7
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Al-Qurna Heritage Palace — West Bank Villa with Ancient Views",

                    Description =
                        "Nestled on the tranquil West Bank of Luxor facing the Theban hills and Valley of the Nobles. Traditional Egyptian courtyard, hand-carved stone arches, and rooftop stargazing over the Nile.",

                    Category = "Design homes",
                    NumberOfGuests = 4,
                    Capacity = 4,
                    BathroomCount = 2,
                    Rating = 4.91,
                    NumberOfReviews = 36
                },
                "Al-Gharbi Nile West Bank, Luxor",
                6);

            // ------------------------------------------------------------
            // 8
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Nubian Sun Villa — Colorful Island Retreat on the Nile",

                    Description =
                        "Immerse yourself in authentic Nubian warmth and vibrant color palettes on the banks of the First Cataract in Aswan. Traditional vaulted ceilings, breezy terrace, and private felucca landing.",

                    Category = "Islands",
                    NumberOfGuests = 5,
                    Capacity = 5,
                    BathroomCount = 2,
                    Rating = 4.89,
                    NumberOfReviews = 48
                },
                "Elephantine Island, Aswan",
                7);

            // ------------------------------------------------------------
            // 9
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Coral Breeze Retreat — Modern Villa on El Gouna Lagoon",

                    Description =
                        "A bright contemporary villa overlooking one of El Gouna's peaceful lagoons, with a private pool, shaded terrace, modern kitchen, and easy access to the town's restaurants and beaches.",

                    Category = "Beachfront",
                    NumberOfGuests = 6,
                    Capacity = 6,
                    BathroomCount = 3,
                    Rating = 4.86,
                    NumberOfReviews = 37
                },
                "Mangroovy Lagoon, El Gouna",
                8);

            // ------------------------------------------------------------
            // 10
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Moonlit Bay House — Private Beach Escape in Soma Bay",

                    Description =
                        "A spacious coastal retreat in Soma Bay with panoramic sea views, private outdoor seating, contemporary interiors, and direct access to a quiet stretch of Red Sea beach.",

                    Category = "Beachfront",
                    NumberOfGuests = 6,
                    Capacity = 6,
                    BathroomCount = 3,
                    Rating = 4.90,
                    NumberOfReviews = 45
                },
                "Kite Beach Road, Soma Bay",
                9);

            // ------------------------------------------------------------
            // 11
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Sinai Horizon Lodge — Mountain & Sea Hideaway",

                    Description =
                        "A peaceful Dahab escape combining warm natural interiors with sweeping views of Sinai's mountains and the Red Sea, perfect for sunrise hikes and quiet evenings.",

                    Category = "Cabins",
                    NumberOfGuests = 4,
                    Capacity = 4,
                    BathroomCount = 2,
                    Rating = 4.84,
                    NumberOfReviews = 33
                },
                "Blue Hole Road, Dahab",
                10);

            // ------------------------------------------------------------
            // 12
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Azure Pearl Villa — Contemporary Escape in Ain Sokhna",

                    Description =
                        "A modern coastal villa in Ain Sokhna featuring floor-to-ceiling windows, a private garden, refreshing pool, and comfortable outdoor dining area overlooking the Red Sea.",

                    Category = "Beachfront",
                    NumberOfGuests = 7,
                    Capacity = 7,
                    BathroomCount = 3,
                    Rating = 4.87,
                    NumberOfReviews = 41
                },
                "Azha Coastal Road, Ain Sokhna",
                11);

            // ------------------------------------------------------------
            // 13
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Ras Mohammed Hideaway — Desert Villa Near the Red Sea",

                    Description =
                        "A secluded Sharm El Sheikh retreat surrounded by desert landscapes, with minimalist interiors, a private courtyard, and quick access to the spectacular Red Sea coastline.",

                    Category = "Desert stays",
                    NumberOfGuests = 5,
                    Capacity = 5,
                    BathroomCount = 2,
                    Rating = 4.82,
                    NumberOfReviews = 28
                },
                "Nabq Desert Edge, Sharm El Sheikh",
                12);

            // ------------------------------------------------------------
            // 14
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Makadi Moon Villa — Palm Garden Retreat by the Sea",

                    Description =
                        "A relaxed Makadi Bay villa surrounded by palms and flowering gardens, offering a private pool, outdoor lounge, fully equipped kitchen, and easy beach access.",

                    Category = "Beachfront",
                    NumberOfGuests = 6,
                    Capacity = 6,
                    BathroomCount = 3,
                    Rating = 4.85,
                    NumberOfReviews = 35
                },
                "Makadi Bay Palm District",
                13);

            // ------------------------------------------------------------
            // 15
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Garden City Residence — Elegant Nile-Side Townhouse",

                    Description =
                        "An elegant Cairo residence in Garden City with classic architectural details, high ceilings, refined interiors, and a peaceful balcony close to the Nile Corniche.",

                    Category = "City homes",
                    NumberOfGuests = 5,
                    Capacity = 5,
                    BathroomCount = 2,
                    Rating = 4.79,
                    NumberOfReviews = 39
                },
                "Corniche El Nil, Garden City",
                14);

            // ------------------------------------------------------------
            // 16
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Zamalek Art House — Contemporary Apartment Overlooking Cairo",

                    Description =
                        "A stylish Zamalek apartment filled with Egyptian artwork, natural light, comfortable living spaces, and a balcony overlooking one of Cairo's most vibrant neighborhoods.",

                    Category = "City lofts",
                    NumberOfGuests = 4,
                    Capacity = 4,
                    BathroomCount = 2,
                    Rating = 4.88,
                    NumberOfReviews = 52
                },
                "26th of July Corridor, Zamalek",
                15);

            // ------------------------------------------------------------
            // 17
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Nile Pearl Penthouse — Panoramic Cairo Skyline Retreat",

                    Description =
                        "A premium penthouse with panoramic Cairo and Nile views, generous living spaces, modern furnishings, a rooftop terrace, and a dedicated workspace.",

                    Category = "City lofts",
                    NumberOfGuests = 6,
                    Capacity = 6,
                    BathroomCount = 3,
                    Rating = 4.93,
                    NumberOfReviews = 47
                },
                "Road 9, Maadi",
                16);

            // ------------------------------------------------------------
            // 18
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Pyramids Sunset Villa — Private Garden Facing the Giza Plateau",

                    Description =
                        "A spacious Giza villa with a landscaped private garden and unforgettable sunset views toward the pyramids, combining modern comfort with Egyptian-inspired design.",

                    Category = "Design homes",
                    NumberOfGuests = 7,
                    Capacity = 7,
                    BathroomCount = 3,
                    Rating = 4.91,
                    NumberOfReviews = 44
                },
                "Al Haram Gardens, Giza",
                17);

            // ------------------------------------------------------------
            // 19
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Al-Muizz Heritage House — Traditional Home in Historic Cairo",

                    Description =
                        "A beautifully restored traditional Cairene home near Al-Muizz Street, featuring handcrafted details, an intimate courtyard, and easy access to historic Islamic Cairo.",

                    Category = "Heritage homes",
                    NumberOfGuests = 4,
                    Capacity = 4,
                    BathroomCount = 2,
                    Rating = 4.89,
                    NumberOfReviews = 32
                },
                "Al-Muizz Street, Gamaleya",
                18);

            // ------------------------------------------------------------
            // 20
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Palm Shadow Oasis — Traditional Siwan Eco Villa",

                    Description =
                        "A tranquil eco villa surrounded by palms and olive trees, built with natural local materials and designed for slow mornings, cool evenings, and authentic Siwan experiences.",

                    Category = "Desert stays",
                    NumberOfGuests = 5,
                    Capacity = 5,
                    BathroomCount = 2,
                    Rating = 4.94,
                    NumberOfReviews = 38
                },
                "Shali Oasis Road, Siwa",
                19);

            // ------------------------------------------------------------
            // 21
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Desert Rose Retreat — Kershef Villa Beneath the Stars",

                    Description =
                        "A handcrafted Kershef retreat near Siwa's desert lakes, with clay walls, cozy interiors, a private courtyard, and spectacular night skies away from city lights.",

                    Category = "Cabins",
                    NumberOfGuests = 4,
                    Capacity = 4,
                    BathroomCount = 2,
                    Rating = 4.92,
                    NumberOfReviews = 27
                },
                "Fatnas Lake Road, Siwa",
                20);

            // ------------------------------------------------------------
            // 22
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Amun's Garden House — Peaceful Olive Grove Escape",

                    Description =
                        "A peaceful Siwan home hidden among olive groves, offering a private garden, traditional architecture, outdoor breakfast space, and a relaxing connection to nature.",

                    Category = "Countryside",
                    NumberOfGuests = 4,
                    Capacity = 4,
                    BathroomCount = 2,
                    Rating = 4.90,
                    NumberOfReviews = 25
                },
                "Ain Safi Road, Siwa",
                21);

            // ------------------------------------------------------------
            // 23
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Lake Qarun House — Sunset Villa Above the Water",

                    Description =
                        "A peaceful Fayoum getaway overlooking Lake Qarun, with wide terraces, sunset dining, a private pool, and comfortable spaces for families and small groups.",

                    Category = "Lakefront",
                    NumberOfGuests = 6,
                    Capacity = 6,
                    BathroomCount = 3,
                    Rating = 4.83,
                    NumberOfReviews = 34
                },
                "Lake Qarun Shore, Fayoum",
                22);

            // ------------------------------------------------------------
            // 24
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Tunis Village Studio — Artistic Adobe Retreat",

                    Description =
                        "A charming adobe studio in Fayoum's artistic Tunis Village, surrounded by pottery workshops, local crafts, gardens, and beautiful countryside landscapes.",

                    Category = "Countryside",
                    NumberOfGuests = 2,
                    Capacity = 2,
                    BathroomCount = 1,
                    Rating = 4.81,
                    NumberOfReviews = 21
                },
                "Tunis Village Arts District",
                23);

            // ------------------------------------------------------------
            // 25
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Wadi El Hitan Lodge — Desert Gateway Eco Retreat",

                    Description =
                        "An eco-conscious desert lodge providing a comfortable base for exploring Wadi El Hitan, with simple natural interiors, outdoor seating, and wide desert views.",

                    Category = "Cabins",
                    NumberOfGuests = 4,
                    Capacity = 4,
                    BathroomCount = 2,
                    Rating = 4.78,
                    NumberOfReviews = 19
                },
                "Wadi El Hitan Desert Road",
                24);

            // ------------------------------------------------------------
            // 26
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Nile West Bank Villa — Rooftop Views of Luxor Temple",

                    Description =
                        "A peaceful West Bank villa with a spacious rooftop terrace, Nile views, traditional Egyptian details, and easy access to Luxor's ancient temples and monuments.",

                    Category = "Design homes",
                    NumberOfGuests = 6,
                    Capacity = 6,
                    BathroomCount = 3,
                    Rating = 4.90,
                    NumberOfReviews = 40
                },
                "Nile Corniche, West Bank Luxor",
                25);

            // ------------------------------------------------------------
            // 27
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Theban Hills House — Traditional Villa Among Ancient Tombs",

                    Description =
                        "A character-filled Luxor home nestled beneath the Theban hills, combining traditional architecture, peaceful outdoor spaces, and memorable views of the surrounding archaeological landscape.",

                    Category = "Heritage homes",
                    NumberOfGuests = 4,
                    Capacity = 4,
                    BathroomCount = 2,
                    Rating = 4.87,
                    NumberOfReviews = 30
                },
                "Theban Foothills, Sheikh Abd el-Qurna",
                26);

            // ------------------------------------------------------------
            // 28
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Pharaoh's Garden Retreat — Peaceful Villa on the Nile",

                    Description =
                        "A spacious riverside retreat in Luxor with lush gardens, a shaded terrace, traditional Egyptian-inspired interiors, and relaxing views across the Nile.",

                    Category = "Riverside",
                    NumberOfGuests = 6,
                    Capacity = 6,
                    BathroomCount = 3,
                    Rating = 4.88,
                    NumberOfReviews = 35
                },
                "Corniche El Nile, Luxor",
                27);

            // ------------------------------------------------------------
            // 29
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "Nubian Blue House — Colorful Villa on Elephantine Island",

                    Description =
                        "A vibrant Nubian-inspired home on Elephantine Island featuring colorful handcrafted interiors, a relaxing riverside terrace, and views of the Nile and surrounding desert hills.",

                    Category = "Islands",
                    NumberOfGuests = 5,
                    Capacity = 5,
                    BathroomCount = 2,
                    Rating = 4.93,
                    NumberOfReviews = 43
                },
                "Elephantine Island North Village",
                28);

            // ------------------------------------------------------------
            // 30
            // ------------------------------------------------------------
            await AddPropertyIfMissing(
                context,
                addressMap,
                hosts,
                new Property
                {
                    PropertyName =
                        "First Cataract Retreat — Nilefront Villa with Felucca Dock",

                    Description =
                        "A beautiful Aswan retreat overlooking the First Cataract, with a riverside terrace, private felucca landing, comfortable bedrooms, and peaceful views of the Nile.",

                    Category = "Riverside",
                    NumberOfGuests = 6,
                    Capacity = 6,
                    BathroomCount = 3,
                    Rating = 4.91,
                    NumberOfReviews = 37
                },
                "First Cataract Riverside, Aswan",
                29);

            await context.SaveChangesAsync();
        }

        private static async Task AddPropertyIfMissing(
            HavenlyDbContext context,
            Dictionary<string, long> addressMap,
            List<User> hosts,
            Property property,
            string street,
            int hostIndex)
        {
            bool exists = await context.Properties
                .AnyAsync(p =>
                    p.PropertyName == property.PropertyName);

            if (exists)
                return;

            if (!addressMap.TryGetValue(
                    street,
                    out var addressId))
            {
                return;
            }

            property.AddressID = addressId;
            property.OwnerUserID =
                hosts[hostIndex % hosts.Count].Id;

            context.Properties.Add(property);
        }

        // ================================================================
        // BEDROOMS
        // ================================================================

        private static async Task SeedBedrooms(
            HavenlyDbContext context)
        {
            var properties = await context.Properties
                .OrderBy(p => p.PropertyID)
                .ToListAsync();

            foreach (var property in properties)
            {
                bool exists = await context.Bedrooms
                    .AnyAsync(b =>
                        b.PropertyID == property.PropertyID);

                if (exists)
                    continue;

                int roomCount;

                if (property.PropertyName ==
                    "Tunis Village Studio — Artistic Adobe Retreat")
                {
                    roomCount = 1;
                }
                else if (property.NumberOfGuests >= 7)
                {
                    roomCount = 3;
                }
                else if (property.NumberOfGuests >= 5)
                {
                    roomCount = 3;
                }
                else
                {
                    roomCount = 2;
                }

                for (int room = 1; room <= roomCount; room++)
                {
                    int beds = 1;

                    if (room == roomCount &&
                        property.NumberOfGuests >= 5)
                    {
                        beds = 2;
                    }

                    string roomName;

                    if (room == 1)
                        roomName = "Primary Bedroom";
                    else if (room == 2)
                        roomName = "Guest Bedroom";
                    else
                        roomName = "Family Bedroom";

                    context.Bedrooms.Add(new Bedroom
                    {
                        PropertyID = property.PropertyID,
                        RoomNumber = room,
                        BedCount = beds,
                        RoomName = roomName
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        // ================================================================
        // PROPERTY IMAGES
        // ================================================================

        private static async Task SeedPropertyImages(
            HavenlyDbContext context)
        {
            var properties = await context.Properties
                .OrderBy(p => p.PropertyID)
                .ToListAsync();

            int nextImageNumber =
                await context.PropertyImages.CountAsync() + 1;

            foreach (var property in properties)
            {
                bool exists = await context.PropertyImages
                    .AnyAsync(i =>
                        i.PropertyID == property.PropertyID);

                if (exists)
                    continue;

                int primaryImgNum = ((nextImageNumber - 1) % 7) + 1;
                context.PropertyImages.Add(
                    new PropertyImage
                    {
                        PropertyID = property.PropertyID,
                        ImagePath =
                            $"/images/p{primaryImgNum}.jpg",
                        IsPrimary = true
                    });

                nextImageNumber++;

                int secondaryImgNum = ((nextImageNumber - 1) % 7) + 1;
                context.PropertyImages.Add(
                    new PropertyImage
                    {
                        PropertyID = property.PropertyID,
                        ImagePath =
                            $"/images/p{secondaryImgNum}.jpg",
                        IsPrimary = false
                    });

                nextImageNumber++;
            }

            await context.SaveChangesAsync();
        }

        // ================================================================
        // LISTINGS
        // ================================================================

        private static async Task SeedListings(
            HavenlyDbContext context)
        {
            var properties = await context.Properties
                .OrderBy(p => p.PropertyID)
                .ToListAsync();

            var listingData = new Dictionary<string, (string Description, decimal Price)>
            {
                {
                    "Villa Turquoise — Lagoon Beachfront Villa with Private Jetty",
                    ("Lagoon beachfront villa with private jetty", 340)
                },

                {
                    "Blue Coral Sanctuary — Boho Beach House by the Reef",
                    ("Boho beach house beside Dahab's coral reef", 275)
                },

                {
                    "The Nile View Loft — Modern Art Deco Penthouse in Zamalek",
                    ("Modern Art Deco penthouse with Nile views", 310)
                },

                {
                    "Salt Lake Eco-Lodge — Kershef Adobe Chalet in Date Palms",
                    ("Siwan Kershef eco-lodge surrounded by date palms", 220)
                },

                {
                    "The White Sands Manor — Mediterranean Beachfront Haven",
                    ("Mediterranean beachfront manor on the North Coast", 420)
                },

                {
                    "Dar El Qamar — Artistic Country Farmhouse near Lake Qarun",
                    ("Artistic countryside farmhouse near Lake Qarun", 190)
                },

                {
                    "Al-Qurna Heritage Palace — West Bank Villa with Ancient Views",
                    ("Heritage villa overlooking Luxor's ancient hills", 240)
                },

                {
                    "Nubian Sun Villa — Colorful Island Retreat on the Nile",
                    ("Colorful Nubian island villa on the Nile", 210)
                },

                {
                    "Coral Breeze Retreat — Modern Villa on El Gouna Lagoon",
                    ("Modern El Gouna villa overlooking a peaceful lagoon", 295)
                },

                {
                    "Moonlit Bay House — Private Beach Escape in Soma Bay",
                    ("Private Soma Bay beach escape with sea views", 360)
                },

                {
                    "Sinai Horizon Lodge — Mountain & Sea Hideaway",
                    ("Dahab mountain and sea hideaway", 205)
                },

                {
                    "Azure Pearl Villa — Contemporary Escape in Ain Sokhna",
                    ("Contemporary Ain Sokhna coastal villa", 280)
                },

                {
                    "Ras Mohammed Hideaway — Desert Villa Near the Red Sea",
                    ("Desert retreat near Sharm El Sheikh and the Red Sea", 230)
                },

                {
                    "Makadi Moon Villa — Palm Garden Retreat by the Sea",
                    ("Palm garden villa near Makadi Bay beach", 265)
                },

                {
                    "Garden City Residence — Elegant Nile-Side Townhouse",
                    ("Elegant Garden City residence near the Nile", 250)
                },

                {
                    "Zamalek Art House — Contemporary Apartment Overlooking Cairo",
                    ("Contemporary Zamalek apartment with artistic interiors", 225)
                },

                {
                    "Nile Pearl Penthouse — Panoramic Cairo Skyline Retreat",
                    ("Luxury Cairo penthouse with panoramic skyline views", 330)
                },

                {
                    "Pyramids Sunset Villa — Private Garden Facing the Giza Plateau",
                    ("Giza villa with private garden and pyramid views", 315)
                },

                {
                    "Al-Muizz Heritage House — Traditional Home in Historic Cairo",
                    ("Traditional home in historic Islamic Cairo", 185)
                },

                {
                    "Palm Shadow Oasis — Traditional Siwan Eco Villa",
                    ("Traditional Siwan eco villa among palms", 200)
                },

                {
                    "Desert Rose Retreat — Kershef Villa Beneath the Stars",
                    ("Kershef desert retreat beneath Siwa's night sky", 195)
                },

                {
                    "Amun's Garden House — Peaceful Olive Grove Escape",
                    ("Peaceful Siwan olive grove house", 180)
                },

                {
                    "Lake Qarun House — Sunset Villa Above the Water",
                    ("Fayoum villa overlooking Lake Qarun at sunset", 240)
                },

                {
                    "Tunis Village Studio — Artistic Adobe Retreat",
                    ("Artistic adobe studio in Tunis Village", 145)
                },

                {
                    "Wadi El Hitan Lodge — Desert Gateway Eco Retreat",
                    ("Eco lodge near Wadi El Hitan", 170)
                },

                {
                    "Nile West Bank Villa — Rooftop Views of Luxor Temple",
                    ("Luxor West Bank villa with rooftop Nile views", 225)
                },

                {
                    "Theban Hills House — Traditional Villa Among Ancient Tombs",
                    ("Traditional Luxor villa beneath the Theban hills", 200)
                },

                {
                    "Pharaoh's Garden Retreat — Peaceful Villa on the Nile",
                    ("Peaceful riverside villa in Luxor", 235)
                },

                {
                    "Nubian Blue House — Colorful Villa on Elephantine Island",
                    ("Colorful Nubian home on Elephantine Island", 215)
                },

                {
                    "First Cataract Retreat — Nilefront Villa with Felucca Dock",
                    ("Nilefront Aswan villa with private felucca landing", 255)
                }
            };

            foreach (var property in properties)
            {
                bool exists = await context.Listings
                    .AnyAsync(l =>
                        l.PropertyID == property.PropertyID);

                if (exists)
                    continue;

                if (!listingData.TryGetValue(
                        property.PropertyName,
                        out var data))
                {
                    continue;
                }

                context.Listings.Add(new Listing
                {
                    PropertyID = property.PropertyID,
                    Description = data.Description,
                    Price = data.Price,
                    IsValid = true,
                    ListingStatus = ListingStatus.Approved
                });
            }

            await context.SaveChangesAsync();
        }

        // ================================================================
        // AMENITIES
        // ================================================================

        private static async Task SeedAmenities(
            HavenlyDbContext context)
        {
            string[] amenities =
            {
                "Wi-Fi",
                "Parking",
                "Kitchen",
                "Air conditioning",
                "Pool",
                "Hot tub",
                "Patio or balcony",
                "Dedicated workspace",
                "EV charger",
                "Pet friendly"
            };

            foreach (var name in amenities)
            {
                bool exists = await context.Amenities
                    .AnyAsync(a => a.Name == name);

                if (exists)
                    continue;

                var amenity = new Amenity();

                amenity.Create(
                    0,
                    name);

                context.Amenities.Add(amenity);
            }

            await context.SaveChangesAsync();
        }

        // ================================================================
        // PROPERTY AMENITIES
        // ================================================================

        private static async Task SeedPropertyAmenities(
            HavenlyDbContext context)
        {
            var properties = await context.Properties
                .ToListAsync();

            var amenities = await context.Amenities
                .ToListAsync();

            var amenityMap = amenities
                .GroupBy(a => a.Name)
                .ToDictionary(g => g.Key, g => g.First().AmenitiesID);

            foreach (var property in properties)
            {
                var names = GetAmenitiesForProperty(
                    property.PropertyName);

                foreach (var name in names)
                {
                    if (!amenityMap.TryGetValue(
                            name,
                            out var amenityId))
                    {
                        continue;
                    }

                    bool exists = await context.PropertyAmenities
                        .AnyAsync(pa =>
                            pa.PropertyID == property.PropertyID &&
                            pa.AmenitiesID == amenityId);

                    if (exists)
                        continue;

                    var propertyAmenity =
                        new PropertyAmenity();

                    propertyAmenity.Create(
                        property.PropertyID,
                        amenityId);

                    context.PropertyAmenities.Add(
                        propertyAmenity);
                }
            }

            await context.SaveChangesAsync();
        }

        private static string[] GetAmenitiesForProperty(
            string propertyName)
        {
            if (propertyName.Contains("Villa Turquoise"))
            {
                return new[]
                {
                    "Wi-Fi",
                    "Parking",
                    "Kitchen",
                    "Air conditioning",
                    "Pool",
                    "Patio or balcony",
                    "Dedicated workspace"
                };
            }

            if (propertyName.Contains("Blue Coral"))
            {
                return new[]
                {
                    "Wi-Fi",
                    "Kitchen",
                    "Air conditioning",
                    "Patio or balcony"
                };
            }

            if (propertyName.Contains("Nile View Loft"))
            {
                return new[]
                {
                    "Wi-Fi",
                    "Kitchen",
                    "Air conditioning",
                    "Patio or balcony",
                    "Dedicated workspace"
                };
            }

            if (propertyName.Contains("Salt Lake"))
            {
                return new[]
                {
                    "Wi-Fi",
                    "Parking",
                    "Kitchen",
                    "Pool",
                    "Patio or balcony"
                };
            }

            if (propertyName.Contains("White Sands"))
            {
                return new[]
                {
                    "Wi-Fi",
                    "Parking",
                    "Kitchen",
                    "Air conditioning",
                    "Pool",
                    "Patio or balcony"
                };
            }

            if (propertyName.Contains("Dar El Qamar"))
            {
                return new[]
                {
                    "Wi-Fi",
                    "Parking",
                    "Kitchen",
                    "Pool",
                    "Patio or balcony",
                    "Pet friendly"
                };
            }

            if (propertyName.Contains("Al-Qurna"))
            {
                return new[]
                {
                    "Wi-Fi",
                    "Kitchen",
                    "Air conditioning",
                    "Patio or balcony",
                    "Dedicated workspace"
                };
            }

            if (propertyName.Contains("Nubian Sun"))
            {
                return new[]
                {
                    "Wi-Fi",
                    "Kitchen",
                    "Air conditioning",
                    "Patio or balcony"
                };
            }

            // Default amenities for the additional properties.
            return new[]
            {
                "Wi-Fi",
                "Kitchen",
                "Air conditioning",
                "Patio or balcony"
            };
        }

        // ================================================================
        // RATINGS / CATEGORIES
        // ================================================================

        private static async Task UpdatePropertyRatingsAndCategories(
            HavenlyDbContext context)
        {
            var properties = await context.Properties
                .ToListAsync();

            foreach (var property in properties)
            {
                if (string.IsNullOrWhiteSpace(property.Category))
                {
                    property.Category = "Design homes";
                }
            }

            await context.SaveChangesAsync();
        }

        // ================================================================
        // BOOKINGS
        // ================================================================

        private static async Task SeedBookings(
            HavenlyDbContext context)
        {
            // Seed a full 12-month dataset if the database has fewer than 60 bookings
            if (await context.Bookings.CountAsync() >= 60)
                return;

            var listings = await context.Listings
                .OrderBy(l => l.ListingID)
                .ToListAsync();

            var guests = await context.Users
                .Where(u => u.Role == UserRoles.Guest)
                .OrderBy(u => u.Id)
                .ToListAsync();

            if (!listings.Any() || !guests.Any())
                return;

            var today = DateTime.UtcNow.Date;

            int bookingCounter = 0;

            // Previous 11 months + current month (full 12 months for chart)
            for (int monthIndex = 0;
                 monthIndex < 12;
                 monthIndex++)
            {
                var monthStart =
                    new DateTime(
                        today.Year,
                        today.Month,
                        1)
                    .AddMonths(-11 + monthIndex);

                // High season in Egypt (Winter & Summer): 5-8 bookings, low season: 3-5 bookings
                int bookingsThisMonth = (monthIndex == 11) ? 4 : (4 + (monthIndex % 4));

                for (int i = 0;
                     i < bookingsThisMonth;
                     i++)
                {
                    var listing =
                        listings[
                            bookingCounter % listings.Count];

                    var guest =
                        guests[
                            bookingCounter % guests.Count];

                    DateTime createdDate;

                    if (monthIndex == 11)
                    {
                        createdDate =
                            today.AddDays(-(i * 2 + 1));
                    }
                    else
                    {
                        createdDate =
                            monthStart.AddDays(
                                2 + (i * 4));
                    }

                    int nights = 3 + (i % 4);

                    var checkIn =
                        createdDate.Date.AddDays(7 + i);

                    var checkOut =
                        checkIn.AddDays(nights);

                    BookingStatus status;

                    if (monthIndex < 10)
                    {
                        status = (i % 7 == 0) ? BookingStatus.Cancelled : BookingStatus.Completed;
                    }
                    else if (i % 3 == 0)
                    {
                        status = BookingStatus.Completed;
                    }
                    else if (i % 2 == 0)
                    {
                        status = BookingStatus.Approved;
                    }
                    else
                    {
                        status = BookingStatus.Pending;
                    }

                    decimal totalPrice =
                        listing.Price * nights;

                    context.Bookings.Add(
                        new Booking
                        {
                            GuestUserID = guest.Id,
                            ListingID = listing.ListingID,
                            CreatedDate = createdDate,
                            CheckIn = checkIn,
                            CheckOut = checkOut,
                            TotalPrice = totalPrice,
                            Status = status
                        });

                    bookingCounter++;
                }
            }

            await context.SaveChangesAsync();
        }

        // ================================================================
        // PAYMENTS
        // ================================================================

        private static async Task SeedPayments(
            HavenlyDbContext context)
        {
            var bookingsWithoutPayment = await context.Bookings
                .Where(b => !context.Payments.Any(p => p.BookingID == b.BookingID))
                .ToListAsync();

            if (!bookingsWithoutPayment.Any())
                return;

            foreach (var booking in bookingsWithoutPayment)
            {
                var payment = new Payment();

                bool completed =
                    booking.Status ==
                    BookingStatus.Completed;

                payment.Create(
                    paymentId: 0,
                    bookingId: booking.BookingID,
                    gateway: "Paymob Card",
                    amount: booking.TotalPrice,
                    transactionId:
                        $"PM-{booking.CreatedDate:yyyyMMdd}-{booking.BookingID * 1000 + 421}",
                    platformFee:
                        Math.Round(
                            booking.TotalPrice * 0.10m,
                            2),
                    hostPayoutAmount:
                        Math.Round(
                            booking.TotalPrice * 0.90m,
                            2),
                    status:
                        completed
                            ? PaymentStatus.Completed
                            : (booking.Status == BookingStatus.Approved ? PaymentStatus.Completed : PaymentStatus.Pending));

                if (completed)
                {
                    payment.MarkCompleted(
                        $"PM-TXN-{booking.BookingID * 1000 + 421}",
                        "Paymob Gateway");

                    payment.MarkPayoutToHost();
                }

                context.Payments.Add(payment);
            }

            await context.SaveChangesAsync();
        }

        // ================================================================
        // REVIEWS
        // ================================================================

        private static async Task SeedReviews(
            HavenlyDbContext context)
        {
            var properties = await context.Properties
                .OrderBy(p => p.PropertyID)
                .ToListAsync();

            var guests = await context.Users
                .Where(u => u.Role == UserRoles.Guest)
                .OrderBy(u => u.Id)
                .ToListAsync();

            if (!guests.Any())
                return;

            string[] comments =
            {
                "Beautiful property and excellent location. Everything was clean and comfortable.",

                "Amazing stay. The views were even better than expected and the host was very helpful.",

                "A wonderful Egyptian getaway. The property was exactly as described.",

                "Very comfortable, peaceful and well maintained. I would definitely stay again.",

                "Excellent experience from check-in to check-out. Highly recommended.",

                "The location was perfect and the property had everything we needed.",

                "Beautiful design, great atmosphere and very friendly hosting.",

                "One of the best stays we have had in Egypt."
            };

            int index = 0;

            foreach (var property in properties)
            {
                bool exists = await context.Reviews
                    .AnyAsync(r =>
                        r.PropertyID == property.PropertyID);

                if (exists)
                {
                    index++;
                    continue;
                }

                var guest =
                    guests[index % guests.Count];

                int rating =
                    property.Rating >= 4.80
                        ? 5
                        : 4;

                context.Reviews.Add(
                    new Review
                    {
                        UserID = guest.Id,
                        PropertyID = property.PropertyID,
                        Rating = rating,
                        Comment =
                            comments[
                                index % comments.Length]
                    });

                index++;
            }

            await context.SaveChangesAsync();
        }

        // ================================================================
        // FAVORITES
        // ================================================================

        private static async Task SeedFavorites(
            HavenlyDbContext context)
        {
            var guests = await context.Users
                .Where(u => u.Role == UserRoles.Guest)
                .OrderBy(u => u.Id)
                .ToListAsync();

            var listings = await context.Listings
                .OrderBy(l => l.ListingID)
                .ToListAsync();

            if (!guests.Any() || !listings.Any())
                return;

            // Create a good number of favorites without duplicates.
            for (int guestIndex = 0;
                 guestIndex < guests.Count;
                 guestIndex++)
            {
                for (int i = 0; i < 4; i++)
                {
                    int listingIndex =
                        (guestIndex * 4 + i * 3)
                        % listings.Count;

                    var guest =
                        guests[guestIndex];

                    var listing =
                        listings[listingIndex];

                    bool exists =
                        await context.Favorites.AnyAsync(f =>
                            f.UserID == guest.Id &&
                            f.ListingID == listing.ListingID);

                    if (exists)
                        continue;

                    context.Favorites.Add(
                        new Favorite
                        {
                            UserID = guest.Id,
                            ListingID = listing.ListingID
                        });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}