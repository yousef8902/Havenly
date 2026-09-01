using Havenly.DAL.Entities;
using Havenly.DAL.Enums;

namespace Havenly.DAL.Repos.Abstractions
{
    public interface IAdminRepository
    {
        // Listings
        Task<(List<Listing> Items, int TotalCount)> GetListingsAsync(
            ListingStatus? status = null,
            string? search = null,
            string? sortBy = null,
            int page = 1,
            int pageSize = 20);

        Task<Listing?> GetListingByIdAsync(long id);
        Task<bool> ApproveListingAsync(long listingId, string? adminUserId = null);
        Task<bool> RejectListingAsync(long listingId, string? adminUserId = null, string? reason = null);

        // Users / Members
        Task<(List<User> Items, int TotalCount)> GetMembersAsync(
            UserStatus? status = null,
            string? search = null,
            int page = 1,
            int pageSize = 20);

        Task<User?> GetUserByIdAsync(string id);
        Task<bool> ToggleUserStatusAsync(string userId, bool isActive);

        // Bookings
        Task<(List<Booking> Items, int TotalCount)> GetBookingsAsync(
            BookingStatus? status = null,
            string? search = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 20);

        // Counts
        Task<int> GetListingsCountAsync(ListingStatus? status = null, string? search = null);
        Task<int> GetMembersCountAsync(UserStatus? status = null, string? search = null);
        Task<int> GetBookingsCountAsync(BookingStatus? status = null, string? search = null);
    }
}
