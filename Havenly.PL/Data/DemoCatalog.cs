using Havenly.BLL.ModelVMs;

namespace Havenly.PL.Data;

/// <summary>
/// Prototype catalog matching the Lovable UI seed data. Replace with BLL services.
/// </summary>
public static class DemoCatalog
{
    public static readonly string[] Categories =
    [
        "Beachfront", "Countryside", "Cabins", "City lofts", "Islands", "Design homes"
    ];

    public static readonly string[] Amenities =
    [
        "Wi-Fi", "Parking", "Kitchen", "Air conditioning", "TV", "Pool",
        "Washer", "Workspace", "Fireplace", "Pets allowed", "Breakfast", "Hot tub"
    ];

    public static readonly string[] BaseRules =
    [
        "Check-in after 3:00 PM",
        "Check-out before 11:00 AM",
        "No parties or events",
        "No smoking indoors"
    ];

    public static List<PropertyDetailsVM> Properties { get; } = BuildProperties();

    public static List<DestinationVM> Destinations { get; } =
    [
        new() { City = "Paros", Country = "Greece", ImageUrl = "/images/hero.jpg" },
        new() { City = "Val d'Orcia", Country = "Italy", ImageUrl = "/images/p2.jpg" },
        new() { City = "Copenhagen", Country = "Denmark", ImageUrl = "/images/p1.jpg" },
        new() { City = "Comporta", Country = "Portugal", ImageUrl = "/images/p4.jpg" }
    ];

    public static List<BookingRowVM> Bookings { get; } =
    [
        Row("HV-4821", "olive-ridge", "Nadia Rahman", "nadia.rahman@mail.com", "2026-09-04", "2026-09-10", 6, 2196, "approved"),
        Row("HV-4832", "pine-hollow", "Tom Bergman", "t.bergman@mail.com", "2026-08-20", "2026-08-23", 4, 897, "pending"),
        Row("HV-4790", "north-loft", "Amira Haddad", "amira.h@mail.com", "2026-06-11", "2026-06-15", 2, 712, "completed"),
        Row("HV-4755", "casa-fiora", "Lucas Meyer", "lucas.meyer@mail.com", "2026-05-02", "2026-05-07", 5, 1188, "completed"),
        Row("HV-4844", "salt-house", "Yara Fahmy", "yara.fahmy@mail.com", "2026-09-01", "2026-09-03", 4, 654, "cancelled"),
        Row("HV-4851", "barn-eleven", "Chloe Deveraux", "chloe.d@mail.com", "2026-08-27", "2026-08-31", 6, 836, "pending"),
        Row("HV-4712", "olive-ridge", "Peter Novak", "p.novak@mail.com", "2026-04-09", "2026-04-13", 8, 1462, "rejected"),
    ];

    public static List<ReviewItemVM> Reviews { get; } =
    [
        new() { Id = "r1", PropertyId = "olive-ridge", Author = "Amira Haddad", Date = "July 2026", Rating = 5, Body = "The photos undersell the view. Elena left a bottle of local wine and a hand-drawn map of the swimming coves. The pool is genuinely as good as it looks.", HostResponse = "Thank you Amira — you left the house spotless. Come back in spring when the olives are flowering." },
        new() { Id = "r2", PropertyId = "olive-ridge", Author = "Lucas Meyer", Date = "June 2026", Rating = 5, Body = "Four adults and four kids and nobody felt crowded. The kitchen is well equipped and the drive down to Naoussa is only ten minutes." },
        new() { Id = "r3", PropertyId = "olive-ridge", Author = "Sara Bensalem", Date = "May 2026", Rating = 4, Body = "Beautiful house and a very responsive host. The road up is steep — take a proper car, not a scooter." },
        new() { Id = "r4", PropertyId = "north-loft", Author = "Tom Bergman", Date = "June 2026", Rating = 5, Body = "Perfect base for four days in the city. Quiet street, excellent bed, and the desk made a work morning painless." },
        new() { Id = "r5", PropertyId = "casa-fiora", Author = "Nadia Rahman", Date = "May 2026", Rating = 5, Body = "Giulia's breakfast baskets alone are worth the booking. We spent every evening on the terrace watching the light go over the valley.", HostResponse = "It was a pleasure hosting you. The terrace misses you already!" },
    ];

    public static List<MemberRowVM> Users { get; } =
    [
        new() { Id = "u1", Name = "Elena Marinos", Email = "elena.marinos@havenly.co", Role = "Host", Status = "Active", Joined = "2019-04-12", Bookings = 214 },
        new() { Id = "u2", Name = "Nadia Rahman", Email = "nadia.rahman@mail.com", Role = "Guest", Status = "Active", Joined = "2024-02-03", Bookings = 9 },
        new() { Id = "u3", Name = "Giulia Ferrari", Email = "giulia@casafiora.it", Role = "Host", Status = "Active", Joined = "2018-09-21", Bookings = 168 },
        new() { Id = "u4", Name = "Tom Bergman", Email = "t.bergman@mail.com", Role = "Guest", Status = "Active", Joined = "2025-06-18", Bookings = 3 },
        new() { Id = "u5", Name = "Rui Almeida", Email = "rui.almeida@mail.com", Role = "Host", Status = "Suspended", Joined = "2022-11-05", Bookings = 47 },
        new() { Id = "u6", Name = "Yara Fahmy", Email = "yara.fahmy@mail.com", Role = "Guest", Status = "Active", Joined = "2026-01-09", Bookings = 2 },
        new() { Id = "u7", Name = "Mikkel Sørensen", Email = "mikkel@northloft.dk", Role = "Host", Status = "Active", Joined = "2021-03-30", Bookings = 121 },
        new() { Id = "u8", Name = "Chloe Deveraux", Email = "chloe.d@mail.com", Role = "Guest", Status = "Pending", Joined = "2026-08-10", Bookings = 0 },
        new() { Id = "u9", Name = "Omar Khalil", Email = "omar.khalil@havenly.co", Role = "Admin", Status = "Active", Joined = "2023-05-02", Bookings = 0 },
    ];

    public static List<ActivityItemVM> Activity { get; } =
    [
        new() { Id = "a1", Type = "listing", Text = "Sofia Costa submitted “Skyline Nine” for review", Time = "12 minutes ago" },
        new() { Id = "a2", Type = "booking", Text = "Chloe Deveraux requested 4 nights at “Barn Eleven”", Time = "48 minutes ago" },
        new() { Id = "a3", Type = "user", Text = "Chloe Deveraux created a guest account", Time = "1 hour ago" },
        new() { Id = "a4", Type = "review", Text = "Amira Haddad left a 5-star review on “Olive Ridge”", Time = "3 hours ago" },
        new() { Id = "a5", Type = "approved", Text = "“Pine Hollow” was approved and published", Time = "Yesterday" },
        new() { Id = "a6", Type = "cancelled", Text = "Booking HV-4844 was cancelled by the guest", Time = "Yesterday" },
        new() { Id = "a7", Type = "booking", Text = "Tom Bergman requested 3 nights at “Pine Hollow”", Time = "2 days ago" },
        new() { Id = "a8", Type = "user", Text = "Rui Almeida was suspended after 3 policy reports", Time = "3 days ago" },
    ];

    public static readonly (string Month, int Revenue)[] RevenueSeries =
    [
        ("Mar", 3120), ("Apr", 4480), ("May", 5260), ("Jun", 4890), ("Jul", 7320), ("Aug", 8140)
    ];

    public static readonly (string Month, int Bookings, int Signups)[] PlatformSeries =
    [
        ("Mar", 214, 96), ("Apr", 268, 121), ("May", 341, 148), ("Jun", 392, 133), ("Jul", 486, 187), ("Aug", 528, 204)
    ];

    public static readonly HashSet<string> DefaultFavorites = ["casa-fiora", "pine-hollow"];

    public static PropertyDetailsVM? Get(string id) =>
        Properties.FirstOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

    public static PropertyCardVM ToCard(PropertyDetailsVM p, string? badge = null, bool showStatus = false, bool? favorite = null) =>
        new()
        {
            Id = p.Id,
            Title = p.Title,
            City = p.City,
            Country = p.Country,
            Price = p.Price,
            Rating = p.Rating,
            Reviews = p.Reviews,
            Guests = p.Guests,
            Bedrooms = p.Bedrooms,
            ImageUrl = p.Images.FirstOrDefault() ?? "/images/p1.jpg",
            Badge = badge,
            Status = p.Status,
            ShowStatus = showStatus,
            IsFavorite = favorite ?? DefaultFavorites.Contains(p.Id)
        };

    public static string Money(decimal n) => $"${n:N0}";

    public static string Date(DateTime d) => d.ToString("MMM d, yyyy");

    public static int Nights(DateTime a, DateTime b)
    {
        var diff = (b.Date - a.Date).TotalDays;
        return diff > 0 ? (int)Math.Round(diff) : 0;
    }

    private static BookingRowVM Row(string id, string propertyId, string guest, string email, string cin, string cout, int guests, decimal total, string status)
    {
        var p = Get(propertyId);
        return new BookingRowVM
        {
            Id = id,
            PropertyId = propertyId,
            Title = p?.Title ?? propertyId,
            City = p?.City ?? "",
            Country = p?.Country ?? "",
            ImageUrl = p?.Images.FirstOrDefault() ?? "/images/p1.jpg",
            Guest = guest,
            GuestEmail = email,
            CheckIn = DateTime.Parse(cin),
            CheckOut = DateTime.Parse(cout),
            Guests = guests,
            Total = total,
            Status = status
        };
    }

    private static List<string> Days(string start, int count)
    {
        var outList = new List<string>();
        var d = DateTime.Parse(start);
        for (var i = 0; i < count; i++)
            outList.Add(d.AddDays(i).ToString("yyyy-MM-dd"));
        return outList;
    }

    private static List<PropertyDetailsVM> BuildProperties() =>
    [
        P("olive-ridge", "Olive Ridge — Cliffside Villa with Infinity Pool", "Paros", "Greece", "Naoussa Bay", 340, 4.94, 128, 8, 4, 5, 3, "Islands",
            ["Wi-Fi", "Pool", "Kitchen", "Air conditioning", "Parking", "TV"],
            "Perched above Naoussa Bay, Olive Ridge pairs warm timber interiors with a 14-metre infinity pool that meets the horizon at sunset. Mornings start with coffee under the olive tree; evenings end on the long teak table with the whole family.",
            ["/images/hero.jpg", "/images/p6.jpg", "/images/p4.jpg", "/images/p1.jpg"],
            "Elena Marinos", "2019", true, 98, BaseRules, "approved", "2024-03-11", Days("2026-09-04", 6)),
        P("north-loft", "North Loft — Bright Oak Apartment in the Old Town", "Copenhagen", "Denmark", "Nyhavn", 165, 4.87, 214, 4, 2, 2, 1, "City lofts",
            ["Wi-Fi", "Kitchen", "Washer", "Workspace", "TV"],
            "A calm two-bedroom loft two streets from the harbour. Herringbone oak floors, tall windows and a proper desk for anyone mixing a few work days into the trip.",
            ["/images/p1.jpg", "/images/p7.jpg", "/images/p5.jpg"],
            "Mikkel Sørensen", "2021", false, 92, BaseRules, "approved", "2024-06-02", Days("2026-08-24", 4)),
        P("casa-fiora", "Casa Fiora — Restored Stone Farmhouse", "Val d'Orcia", "Italy", "Pienza", 220, 4.91, 96, 6, 3, 4, 2, "Countryside",
            ["Wi-Fi", "Pool", "Kitchen", "Parking", "Fireplace", "Breakfast"],
            "Seventeenth-century stone, cypress avenue, and a kitchen built for long lunches. Ten minutes from Pienza, an hour from Siena, and quiet enough to hear the wind in the wheat.",
            ["/images/p2.jpg", "/images/p7.jpg", "/images/p4.jpg"],
            "Giulia Ferrari", "2018", true, 100, [..BaseRules, "Pets welcome with prior notice"], "approved", "2024-01-19", Days("2026-09-15", 5)),
        P("pine-hollow", "Pine Hollow — Glass Cabin in the Forest", "Åre", "Sweden", "Björnänge", 275, 4.96, 74, 5, 2, 3, 2, "Cabins",
            ["Wi-Fi", "Hot tub", "Kitchen", "Fireplace", "Parking", "Pets allowed"],
            "Floor-to-ceiling glass facing a wall of pines, a wood stove that heats the whole cabin, and a cedar hot tub under the northern sky.",
            ["/images/p3.jpg", "/images/p7.jpg", "/images/p1.jpg"],
            "Anders Lind", "2020", true, 95, BaseRules, "approved", "2024-08-07", Days("2026-08-20", 3)),
        P("salt-house", "Salt House — Beachfront Home with Open Terrace", "Comporta", "Portugal", "Carvalhal", 295, 4.82, 151, 7, 3, 4, 3, "Beachfront",
            ["Wi-Fi", "Kitchen", "Air conditioning", "Parking", "TV", "Washer"],
            "Two minutes of soft sand between the terrace and the Atlantic. Whitewashed walls, rattan everywhere, and outdoor showers for coming back from the beach.",
            ["/images/p4.jpg", "/images/p6.jpg", "/images/p2.jpg"],
            "Rui Almeida", "2022", false, 88, BaseRules, "approved", "2025-02-14", Days("2026-09-01", 2)),
        P("skyline-nine", "Skyline Nine — Penthouse Terrace above the River", "Lisbon", "Portugal", "Príncipe Real", 410, 4.78, 63, 4, 2, 2, 2, "Design homes",
            ["Wi-Fi", "Kitchen", "Air conditioning", "TV", "Workspace", "Washer"],
            "A ninth-floor apartment with a wraparound terrace, a fire bowl, and the whole city glittering below after dark.",
            ["/images/p5.jpg", "/images/p1.jpg", "/images/p4.jpg"],
            "Sofia Costa", "2023", false, 90, BaseRules, "pending", "2026-08-02", []),
        P("barn-eleven", "Barn Eleven — Converted Hay Barn with Beams", "Cotswolds", "United Kingdom", "Stow-on-the-Wold", 190, 4.89, 108, 6, 3, 3, 2, "Countryside",
            ["Wi-Fi", "Kitchen", "Fireplace", "Parking", "Pets allowed", "Washer"],
            "Original oak trusses, exposed brick, and wool blankets on every bed. Village pub is a six-minute walk across the field.",
            ["/images/p7.jpg", "/images/p2.jpg", "/images/p3.jpg"],
            "Harriet Doyle", "2017", true, 97, BaseRules, "approved", "2023-11-28", Days("2026-08-27", 4)),
        P("cala-blanca", "Cala Blanca — Village House with Blue Shutters", "Menorca", "Spain", "Binibeca", 145, 4.71, 87, 4, 2, 3, 1, "Islands",
            ["Wi-Fi", "Kitchen", "Air conditioning", "TV"],
            "A whitewashed fisherman's house on a quiet lane, bougainvillea over the door, and a cove you can swim in before breakfast.",
            ["/images/p6.jpg", "/images/p4.jpg", "/images/p2.jpg"],
            "Rui Almeida", "2022", false, 88, BaseRules, "draft", "2026-08-12", []),
    ];

    private static PropertyDetailsVM P(
        string id, string title, string city, string country, string neighbourhood,
        decimal price, double rating, int reviews, int guests, int bedrooms, int beds, int baths,
        string category, List<string> amenities, string description, List<string> images,
        string host, string since, bool superhost, int response, IReadOnlyList<string> rules,
        string status, string submitted, List<string> booked) => new()
    {
        Id = id,
        Title = title,
        City = city,
        Country = country,
        Neighbourhood = neighbourhood,
        Price = price,
        Rating = rating,
        Reviews = reviews,
        Guests = guests,
        Bedrooms = bedrooms,
        Beds = beds,
        Baths = baths,
        Category = category,
        Amenities = amenities,
        Description = description,
        Images = images,
        Host = new HostInfoVM { Name = host, Since = since, Superhost = superhost, ResponseRate = response },
        Rules = [.. rules],
        Status = status,
        Submitted = submitted,
        BookedDates = booked
    };
}
