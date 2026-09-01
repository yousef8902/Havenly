using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Repos.Implementations
{
    public class AdminRepository : IAdminRepository
    {
        private readonly HavenlyDbContext _context;

        public AdminRepository(HavenlyDbContext context)
        {
            _context = context;
        }

        // Returns Entities with Pagination
        public async Task<(List<Listing> Items, int TotalCount)> GetListingsAsync(
            ListingStatus? status = null,
            string? search = null,
            string? sortBy = null,
            int page = 1,
            int pageSize = 20)
        {
            var query = _context.Listings
                .Include(l => l.Property)
                    .ThenInclude(p => p.Owner)
                .Include(l => l.Property)
                    .ThenInclude(p => p.Address)
                .Include(l => l.Property)
                    .ThenInclude(p => p.Images)
                .Include(l => l.Bookings)
                    .ThenInclude(b => b.Reviews)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(l => l.ListingStatus == status.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(l =>
                    l.Property.PropertyName.ToLower().Contains(s) ||
                    l.Description.ToLower().Contains(s) ||
                    l.Property.Owner.Name.ToLower().Contains(s) ||
                    (l.Property.Address != null && (l.Property.Address.City.ToLower().Contains(s) || l.Property.Address.Country.ToLower().Contains(s)))
                );
            }

            var totalCount = await query.CountAsync();

            // Apply sorting
            query = sortBy switch
            {
                "price_high" => query.OrderByDescending(l => l.Price),
                "price_low" => query.OrderBy(l => l.Price),
                _ => query.OrderByDescending(l => l.ListingID), // newest
            };

            // Apply pagination
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Listing?> GetListingByIdAsync(long id)
        {
            return await _context.Listings
                .Include(l => l.Property)
                    .ThenInclude(p => p.Owner)
                .Include(l => l.Property)
                    .ThenInclude(p => p.Address)
                .Include(l => l.Property)
                    .ThenInclude(p => p.Images)
                .Include(l => l.Bookings)
                    .ThenInclude(b => b.Reviews)
                .FirstOrDefaultAsync(l => l.ListingID == id);
        }

        public async Task<bool> ApproveListingAsync(long listingId, string? adminUserId = null)
        {
            var listing = await _context.Listings.FindAsync(listingId);
            if (listing == null)
                return false;

            listing.ListingStatus = ListingStatus.Approved;
            listing.IsValid = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectListingAsync(long listingId, string? adminUserId = null, string? reason = null)
        {
            var listing = await _context.Listings.FindAsync(listingId);
            if (listing == null)
                return false;

            listing.ListingStatus = ListingStatus.Declined;
            listing.IsValid = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(List<User> Items, int TotalCount)> GetMembersAsync(
            UserStatus? status = null,
            string? search = null,
            int page = 1,
            int pageSize = 20)
        {
            var query = _context.Users
                .Include(u => u.Bookings)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(u =>
                    u.Name.ToLower().Contains(s) ||
                    (u.Email != null && u.Email.ToLower().Contains(s)) ||
                    (u.Role != null && u.Role.ToLower().Contains(s))
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _context.Users
                .Include(u => u.Bookings)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> ToggleUserStatusAsync(string userId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.Status = isActive ? UserStatus.Active : UserStatus.Suspended;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(List<Booking> Items, int TotalCount)> GetBookingsAsync(
            BookingStatus? status = null,
            string? search = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 20)
        {
            var query = _context.Bookings
                .Include(b => b.Listing)
                    .ThenInclude(l => l.Property)
                        .ThenInclude(p => p.Images)
                .Include(b => b.Guest)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(b => b.CheckIn >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(b => b.CheckOut <= toDate.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(b =>
                    (b.Listing != null && b.Listing.Description.ToLower().Contains(s)) ||
                    (b.Listing != null && b.Listing.Property != null && b.Listing.Property.PropertyName.ToLower().Contains(s)) ||
                    (b.Guest != null && b.Guest.Name.ToLower().Contains(s))
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(b => b.CheckIn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<int> GetListingsCountAsync(ListingStatus? status = null, string? search = null)
        {
            var query = _context.Listings.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(l => l.ListingStatus == status.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(l =>
                    l.Description.ToLower().Contains(s) ||
                    l.Property.PropertyName.ToLower().Contains(s) ||
                    l.Property.Owner.Name.ToLower().Contains(s) ||
                    (l.Property.Address != null && (l.Property.Address.City.ToLower().Contains(s) || l.Property.Address.Country.ToLower().Contains(s)))
                );
            }

            return await query.CountAsync();
        }

        public async Task<int> GetMembersCountAsync(UserStatus? status = null, string? search = null)
        {
            var query = _context.Users.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(u =>
                    u.Name.ToLower().Contains(s) ||
                    (u.Email != null && u.Email.ToLower().Contains(s))
                );
            }

            return await query.CountAsync();
        }

        public async Task<int> GetBookingsCountAsync(BookingStatus? status = null, string? search = null)
        {
            var query = _context.Bookings.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(b =>
                    (b.Listing != null && b.Listing.Property.PropertyName.ToLower().Contains(s)) ||
                    (b.Guest != null && b.Guest.Name.ToLower().Contains(s))
                );
            }

            return await query.CountAsync();
        }
    }
}
