namespace Havenly.BLL.ModelVMs
{
    public class PropertyDetailsVM
    {
        public string Id { get; set; } = "";
        public string OwnerId { get; set; } = "";
        public long ListingID { get; set; } 
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
        public HostVM Host { get; set; } = new();
        public List<string> Rules { get; set; } = [];
        public string Status { get; set; } = "approved";
        public bool IsValid { get; set; } = true;
        public string Submitted { get; set; } = "";
        public List<string> BookedDates { get; set; } = [];
        public bool IsFavorite { get; set; }
        public List<ReviewItemVM> PropertyReviews { get; set; } = [];
        public List<PropertyCardVM> Similar { get; set; } = [];
        public BookingRequestFormVM Booking { get; set; } = new();
    }
}