using Havenly.BLL.ModelVMs;
using Havenly.DAL.Enums;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IAdminService
    {
        
        Task<AdminDashboardVM> GetDashboardDataAsync();

        // Listings
        Task<AdminListingsVM> GetListingsDataAsync(
            ListingStatus? status = null,
            string? search = null,
            string? sortBy = null,
            int page = 1,
            int pageSize = 20);
        Task<bool> ApproveListingAsync(int listingId, int adminUserId);
        Task<bool> RejectListingAsync(int listingId, int adminUserId, string? reason = null);

        // Members
        Task<AdminMembersVM> GetMembersDataAsync(
            UserStatus? status = null,
            string? search = null,
            int page = 1,
            int pageSize = 20);
        Task<bool> ToggleUserStatusAsync(string userId, bool isActive);

        // Bookings
        Task<AdminBookingsVM> GetBookingsDataAsync(
            BookingStatus? status = null,
            string? search = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 20);

        // Activity
       
    }
}
