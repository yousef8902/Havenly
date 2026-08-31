using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace Havenly.BLL.ModelVMs
{
    public class PropertyCardVM
    {
        public long Id { get; set; } 
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

    public class HostVM
    {
        public string Name { get; set; } = "";
        public string Since { get; set; } = "";
        public bool Superhost { get; set; }
        public int ResponseRate { get; set; }
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

    public class AdminMembersVM
    {
        public List<MemberItem> Members { get; set; } = new();
        public PaginationInfo Pagination { get; set; } = new();
        public MemberFilters Filters { get; set; } = new();
        public MemberStats Stats { get; set; } = new();
    }

    public class MemberItem
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        //public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime? JoinedDate { get; set; }
        public UserStatus Status { get; set; }
        public int BookingsCount { get; set; }
        public decimal TotalSpent { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime? LastActive { get; set; }
        public bool IsVerified { get; set; }
        public string StatusDisplay => Status.ToString();
        public bool IsActive => Status == UserStatus.Active;
    }

    public class MemberFilters
    {
        public string Status { get; set; } = "all";
        public string SearchTerm { get; set; } = string.Empty;
        public string Role { get; set; } = "all";
    }

    public class MemberStats
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int Suspended { get; set; }
        public int Guests { get; set; }
        public int Hosts { get; set; }
        public int Admins { get; set; }
    }
    public class AdminListingsVM
    {
        public List<ListingItem> Listings { get; set; } = new();
        public PaginationInfo Pagination { get; set; } = new();
        public ListingFilters Filters { get; set; } = new();
        public ListingStats Stats { get; set; } = new();
    }

    public class ListingItem
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string HostName { get; set; } = string.Empty;
        public string HostId { get; set; } = string.Empty;
        public ListingStatus Status { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public String? ImageUrl { get; set; } = null;
        public int ReviewsCount { get; set; }
        public double AverageRating { get; set; }
        public string StatusDisplay => Status.ToString();
    }

    public class ListingFilters
    {
        public string Status { get; set; } = "all";
        public string SearchTerm { get; set; } = string.Empty;
        public string SortBy { get; set; } = "newest";
    }

    public class ListingStats
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Declined { get; set; }
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
    public class PendingListing
    {
        public long ListingId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string HostName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public String HostId { get; set; }
         public DateTime? SubmittedDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public ICollection<PropertyImage> ImageUrl { get; set; }
        public decimal Price { get; set; }
    }
    public class AdminActivityVM
    {
        public List<ActivityItem> Activities { get; set; } = new();
        public ActivityFilter Filters { get; set; } = new();
    }

    public class ActivityItem
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserAvatar { get; set; } = string.Empty;
    }

    public class ActivityFilter
    {
        public string Type { get; set; } = "all";
    }

}