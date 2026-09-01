using Havenly.BLL.ModelVMs;
using Havenly.BLL.ModelVMs.Admin;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;

namespace Havenly.BLL.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repository;

        public AdminService(IAdminRepository repository)
        {
            _repository = repository;
        }

        public async Task<AdminDashboardVM> GetDashboardDataAsync()
        {
            var totalMembersTask = _repository.GetMembersCountAsync(null, null);
            var totalListingsTask = _repository.GetListingsCountAsync(null, null);
            var pendingListingsTask = _repository.GetListingsCountAsync(ListingStatus.Pending, null);
            var (bookings, _) = await _repository.GetBookingsAsync(null, null, null, null, 1, int.MaxValue);

            var totalMembers = await totalMembersTask;
            var totalListings = await totalListingsTask;
            var pendingListings = await pendingListingsTask;

            var grossBookings = bookings
                .Where(b => b.Status != BookingStatus.Cancelled)
                .Sum(b => b.TotalPrice);

            // Get pending listings for display
            var (pendingEntities, _) = await _repository.GetListingsAsync(
                status: ListingStatus.Pending,
                page: 1,
                pageSize: 10);

            var pendingList = pendingEntities.Select(l => new PendingListing
            {
                ListingId = l.ListingID,
                PropertyName = l.Property?.PropertyName ?? "Unknown",
                HostName = l.Property?.Owner?.Name ?? "Unknown",
                HostId = l.Property?.OwnerUserID ?? string.Empty,
                Location = l.Property?.Address != null
                    ? $"{l.Property.Address.City}, {l.Property.Address.Country}"
                    : "N/A",
                ImageUrl = l.Property?.Images ?? new List<PropertyImage>(),
                Price = l.Price
            }).ToList();

            return new AdminDashboardVM
            {
                Stats = new PlatformStatsVM
                {
                    MemberCount = totalMembers,
                    ListingCount = totalListings,
                    GrossBookings = grossBookings,
                    PendingReviewCount = pendingListings,
                    TotalBookings = bookings.Count
                },
                PendingListings = pendingList
            };
        }

        public async Task<AdminListingsVM> GetListingsDataAsync(
            ListingStatus? status = null,
            string? search = null,
            string? sortBy = null,
            int page = 1,
            int pageSize = 20)
        {
            var (entities, totalCount) = await _repository.GetListingsAsync(status, search, sortBy, page, pageSize);

            var items = entities.Select(l => new ListingItem
            {
                Id = l.ListingID,
                Title = l.Property?.PropertyName ?? "Unknown",
                HostName = l.Property?.Owner?.Name ?? "Unknown",
                HostId = l.Property?.OwnerUserID ?? string.Empty,
                Status = l.ListingStatus,
                Price = l.Price,
                Location = l.Property?.Address != null
                    ? $"{l.Property.Address.City}, {l.Property.Address.Country}"
                    : "N/A",
                ImageUrl = l.Property?.Images?.FirstOrDefault()?.ImagePath,
                ReviewsCount = l.Bookings?.SelectMany(b => b.Reviews).Count() ?? 0,
                AverageRating = l.Bookings?
                    .SelectMany(b => b.Reviews)
                    .Average(r => (double?)r.Rating) ?? 0
            }).ToList();

            // Get accurate stats
            var total = await _repository.GetListingsCountAsync(null, null);
            var pending = await _repository.GetListingsCountAsync(ListingStatus.Pending, null);
            var approved = await _repository.GetListingsCountAsync(ListingStatus.Approved, null);
            var declined = await _repository.GetListingsCountAsync(ListingStatus.Declined, null);

            return new AdminListingsVM
            {
                Listings = items,
                Pagination = new PaginationInfo
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = totalCount
                },
                Filters = new ListingFilters
                {
                    Status = status?.ToString()?.ToLower() ?? "all",
                    SearchTerm = search ?? string.Empty,
                    SortBy = sortBy ?? "newest"
                },
                Stats = new ListingStats
                {
                    Total = total,
                    Pending = pending,
                    Approved = approved,
                    Declined = declined
                }
            };
        }

        public async Task<bool> ApproveListingAsync(int listingId, int adminUserId)
        {
            return await _repository.ApproveListingAsync(listingId, adminUserId.ToString());
        }

        public async Task<bool> RejectListingAsync(int listingId, int adminUserId, string? reason = null)
        {
            return await _repository.RejectListingAsync(listingId, adminUserId.ToString(), reason);
        }

        public async Task<AdminMembersVM> GetMembersDataAsync(
            UserStatus? status = null,
            string? search = null,
            int page = 1,
            int pageSize = 20)
        {
            var (entities, totalCount) = await _repository.GetMembersAsync(status, search, page, pageSize);

            var items = entities.Select(u => new MemberItem
            {
                Id = u.Id,
                FullName = u.Name,
                Email = u.Email ?? string.Empty,
                Role = u.Role ?? "Guest",
                Status = u.Status,
                BookingsCount = u.Bookings?.Count ?? 0,
                TotalSpent = u.Bookings?
                    .Where(b => b.Status != BookingStatus.Cancelled)
                    .Sum(b => b.TotalPrice) ?? 0,
                Phone = u.PhoneNumber ?? string.Empty,
                IsVerified = u.Status == UserStatus.Active
            }).ToList();

            // Get accurate member counts
            var allUsers = await _repository.GetMembersAsync(null, null, 1, int.MaxValue);
            var allUsersList = allUsers.Items;

            return new AdminMembersVM
            {
                Members = items,
                Pagination = new PaginationInfo
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = totalCount
                },
                Filters = new MemberFilters
                {
                    Status = status?.ToString()?.ToLower() ?? "all",
                    SearchTerm = search ?? string.Empty
                },
                Stats = new MemberStats
                {
                    Total = allUsersList.Count,
                    Active = allUsersList.Count(u => u.Status == UserStatus.Active),
                    Suspended = allUsersList.Count(u => u.Status == UserStatus.Suspended),
                    Guests = allUsersList.Count(u => string.Equals(u.Role, "Guest", StringComparison.OrdinalIgnoreCase)),
                    Hosts = allUsersList.Count(u => string.Equals(u.Role, "Host", StringComparison.OrdinalIgnoreCase)),
                    Admins = allUsersList.Count(u => string.Equals(u.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                }
            };
        }

        public async Task<bool> ToggleUserStatusAsync(string userId, bool isActive)
        {
            return await _repository.ToggleUserStatusAsync(userId, isActive);
        }

        public async Task<AdminBookingsVM> GetBookingsDataAsync(
            BookingStatus? status = null,
            string? search = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 20)
        {
            var (entities, totalCount) = await _repository.GetBookingsAsync(status, search, fromDate, toDate, page, pageSize);

            var items = entities.Select(b => new BookingDetailsVM
            {
                BookingId = b.BookingID,
                propertyName = b.Listing?.Property?.PropertyName ?? "Unknown",
                PropertyId = b.Listing?.PropertyID,
                Guest = b.Guest?.Name ?? "Unknown",
                GuestID = b.Guest?.Id ?? string.Empty,
                HostName = b.Listing?.Property?.Owner?.Name ?? "Unknown",
                HostId = b.Listing?.Property?.OwnerUserID ?? string.Empty,
                CheckIn = b.CheckIn,
                CheckOut = b.CheckOut,
                Nights = (b.CheckOut - b.CheckIn).Days,
                Total = b.TotalPrice,
                Status = b.Status.ToString(),
                ImageUrl = b.Listing?.Property?.Images?.FirstOrDefault()?.ImagePath ?? "/images/p1.jpg"
            }).ToList();

            // Get stats across all bookings
            var allBookings = await _repository.GetBookingsAsync(null, null, null, null, 1, int.MaxValue);
            var allBookingsList = allBookings.Items;

            return new AdminBookingsVM
            {
                Bookings = items,
                Pagination = new PaginationInfo
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = totalCount
                },
                Stats = new BookingStats
                {
                    Total = allBookingsList.Count,
                    Confirmed = allBookingsList.Count(b => b.Status == BookingStatus.Approved),
                    Pending = allBookingsList.Count(b => b.Status == BookingStatus.Pending),
                    Cancelled = allBookingsList.Count(b => b.Status == BookingStatus.Cancelled),
                    Completed = allBookingsList.Count(b => b.Status == BookingStatus.Completed),
                    TotalRevenue = allBookingsList
                        .Where(b => b.Status != BookingStatus.Cancelled)
                        .Sum(b => b.TotalPrice),
                    AverageBookingValue = allBookingsList
                        .Where(b => b.Status != BookingStatus.Cancelled)
                        .Select(b => b.TotalPrice)
                        .DefaultIfEmpty(0)
                        .Average()
                },
                Filters = new BookingFilters
                {
                    Status = status?.ToString()?.ToLower() ?? "all",
                    SearchTerm = search ?? string.Empty,
                    FromDate = fromDate,
                    ToDate = toDate
                }
            };
        }
    }
}