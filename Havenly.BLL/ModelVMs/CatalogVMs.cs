namespace Havenly.BLL.ModelVMs
{
    public class PropertyCardVM
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public int Reviews { get; set; }
        public int MaxGuests { get; set; }
        public int Bedrooms { get; set; }
        public string ImageUrl { get; set; } = "";
        public string? Badge { get; set; }
        public string Status { get; set; } = "approved";
        public bool ShowStatus { get; set; }
        public bool IsFavorite { get; set; }
    }

    public class DestinationVM
    {
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public string ImageUrl { get; set; } = "";
    }

    public class HostInfoVM
    {
        public string Name { get; set; } = "";
        public string Since { get; set; } = "";
        public bool Superhost { get; set; }
        public int ResponseRate { get; set; }
    }

    public class PropertyDetailsVM
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public string Neighbourhood { get; set; } = "";
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public int Reviews { get; set; }
        public int Guests { get; set; }
        public int Bedrooms { get; set; }
        public int Beds { get; set; }
        public int Baths { get; set; }
        public string Category { get; set; } = "";
        public List<string> Amenities { get; set; } = [];
        public string Description { get; set; } = "";
        public List<string> Images { get; set; } = [];
        public HostInfoVM Host { get; set; } = new();
        public List<string> Rules { get; set; } = [];
        public string Status { get; set; } = "approved";
        public string Submitted { get; set; } = "";
        public List<string> BookedDates { get; set; } = [];
        public bool IsFavorite { get; set; }
        public List<ReviewItemVM> PropertyReviews { get; set; } = [];
        public List<PropertyCardVM> Similar { get; set; } = [];
        public BookingRequestFormVM Booking { get; set; } = new();
    }

    public class ReviewItemVM
    {
        public string Id { get; set; } = "";
        public string PropertyId { get; set; } = "";
        public string PropertyTitle { get; set; } = "";
        public string Author { get; set; } = "";
        public string Date { get; set; } = "";
        public int Rating { get; set; }
        public string Body { get; set; } = "";
        public string? HostResponse { get; set; }
    }

    //public class BookingRowVM
    //{
    //    public string Id { get; set; } = "";
    //    public string PropertyId { get; set; } = "";
    //    public string Title { get; set; } = "";
    //    public string City { get; set; } = "";
    //    public string Country { get; set; } = "";
    //    public string ImageUrl { get; set; } = "";
    //    public string Guest { get; set; } = "";
    //    public string GuestEmail { get; set; } = "";
    //    public DateTime CheckIn { get; set; }
    //    public DateTime CheckOut { get; set; }
    //    public int Guests { get; set; }
    //    public decimal Total { get; set; }
    //    public string Status { get; set; } = "";
    //}

    public class HomeIndexVM
    {
        public SearchFilterVM Search { get; set; } = new();
        public List<DestinationVM> Destinations { get; set; } = [];
        public List<PropertyCardVM> Featured { get; set; } = [];
        public List<PropertyCardVM> Recommended { get; set; } = [];
    }

    public class SearchPageVM
    {
        public SearchFilterVM Filters { get; set; } = new();
        public List<PropertyCardVM> Results { get; set; } = [];
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 6;
        public int PageCount { get; set; } = 1;
        public IReadOnlyList<string> Categories { get; set; } = [];
        public IReadOnlyList<string> AmenityOptions { get; set; } = [];
    }

    public class BookingsPageVM
    {
        public List<BookingDetailsVM> Bookings { get; set; } = [];
    }

    public class BookingConfirmedVM
    {
        public string Id { get; set; } = "";
        public PropertyDetailsVM? Property { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Guests { get; set; }
        public decimal Total { get; set; }
        public int Nights { get; set; }
    }

    public class FavoritesPageVM
    {
        public List<PropertyCardVM> Saved { get; set; } = [];
    }

    public class ReviewsPageVM
    {
        public ReviewCreateVM Form { get; set; } = new();
        public List<BookingDetailsVM> PendingReviews { get; set; } = [];
        public List<ReviewItemVM> Recent { get; set; } = [];
    }

    public class HostDashboardVM
    {
        public string Earnings { get; set; } = "";
        public int Pending { get; set; }
        public int ActiveListings { get; set; }
        public string AverageRating { get; set; } = "";
        public List<(string Month, int Revenue)> Revenue { get; set; } = [];
        public List<PropertyCardVM> Listings { get; set; } = [];
        public List<BookingDetailsVM> LatestBookings { get; set; } = [];
    }

    public class HostBookingsPageVM
    {
        public List<BookingDetailsVM> Bookings { get; set; } = [];
    }

    public class AdminOverviewVM
    {
        public int MemberCount { get; set; }
        public int ListingCount { get; set; }
        public string GrossBookings { get; set; } = "";
        public int PendingReview { get; set; }
        public List<(string Month, int Bookings, int Signups)> Platform { get; set; } = [];
        public List<PropertyCardVM> Listings { get; set; } = [];
        public List<MemberRowVM> Members { get; set; } = [];
        public string? MemberQuery { get; set; }
    }

    public class MemberRowVM
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string Status { get; set; } = "";
        public string Joined { get; set; } = "";
        public int Bookings { get; set; }
    }

    public class ActivityItemVM
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = "";
        public string Text { get; set; } = "";
        public string Time { get; set; } = "";
    }

    public class AdminActivityVM
    {
        public string Filter { get; set; } = "all";
        public List<ActivityItemVM> Rows { get; set; } = [];
    }
}