using Havenly.DAL.Entities;
using Havenly.DAL.Enums;

public interface IAdminRepository
{

    // Listings - Returns Entities
    Task<(List<Listing> Items, int TotalCount)> GetListingsAsync(
         ListingStatus? status = null,
            string? search = null,
            string? sortBy = null,
            int page = 1,
            int pageSize = 20);
    Task<Listing?> GetListingByIdAsync(int id);
    Task<bool> ApproveListingAsync(int listingId, int adminUserId);
    Task<bool> RejectListingAsync(int listingId, int adminUserId, string? reason = null);

    // Users/Members - Returns Entities
    Task<(List<User> Items, int TotalCount)> GetMembersAsync(
         UserStatus? status = UserStatus.Active,
            string? search = null,
            int page = 1,
            int pageSize = 20);
   
    Task<bool> ToggleUserStatusAsync(int userId, bool isActive);

    // Bookings - Returns Entities
    Task<(List<Booking> Items, int TotalCount)> GetBookingsAsync(
            BookingStatus? status = BookingStatus.Pending,
            string? search = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 20);
    

  
    // Counts - Returns primitives
    Task<int> GetListingsCountAsync(ListingStatus? status = ListingStatus.Approved, string? search = null);
    Task<int> GetMembersCountAsync(UserStatus? status = UserStatus.Active, string? search = null);
    Task<int> GetBookingsCountAsync(BookingStatus? status = BookingStatus.Pending, string? search = null);
}
