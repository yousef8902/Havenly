using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
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
                .ThenInclude(l=>l.Owner)
                .Include(l => l.Bookings)
                .ThenInclude(l=>l.Reviews)
                .AsQueryable();

            
                query = query.Where(l => l.ListingStatus == status);
            

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(l =>
                    l.Property.PropertyName.Contains(search) ||
                    l.Description.Contains(search) ||
                    l.Property.Owner.Name.Contains(search) 
                    
                    
                );
            }

            
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = sortBy switch
            {
                
                "price_high" => query.OrderByDescending(l => l.Price),
                "price_low" => query.OrderBy(l => l.Price),
                
                _ => query.OrderBy(l => l.Price), // newest
            };

            // Apply pagination
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Listing?> GetListingByIdAsync(int id)
        {
            return await _context.Listings
                 .Include(l => l.Property)
                .ThenInclude(l => l.Owner)
                .Include(l => l.Bookings)
                .ThenInclude(l => l.Reviews)
                .FirstOrDefaultAsync(l => l.ListingID == id);
        }

        public async Task<bool> ApproveListingAsync(int listingId, int adminUserId)
        {
            var listing = await _context.Listings.FindAsync(listingId);
            if (listing == null || listing.ListingStatus != ListingStatus.Pending)
                return false;

            listing.ListingStatus = ListingStatus.Approved;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectListingAsync(int listingId, int adminUserId, string? reason = null)
        {
            var listing = await _context.Listings.FindAsync(listingId);
            if (listing == null || listing.ListingStatus != ListingStatus.Pending)
                return false;

            listing.ListingStatus = ListingStatus.Declined;
          

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(List<User> Items, int TotalCount)> GetMembersAsync(
            
            UserStatus? status = UserStatus.Active,
            string? search = null,
            int page = 1,
            int pageSize = 20)
        {
            var query = _context.Users
                .Include(u => u.Bookings)
                .AsQueryable();

          

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    u.Name.Contains(search) 
    
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
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

        public async Task<bool> ToggleUserStatusAsync(int userId, bool isActive)
        {
            throw new NotImplementedException();
        }

        public async Task<(List<Booking> Items, int TotalCount)> GetBookingsAsync(
            BookingStatus? status = BookingStatus.Pending,
            string? search = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 20)
        {
            var query = _context.Bookings
                .Include(b => b.Listing)
                .ThenInclude(b => b.Property)
                .Include(b => b.Guest)
                .AsQueryable();

            
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
                query = query.Where(b =>
                    b.Listing.Description.Contains(search) ||
                    b.Guest.Name.Contains(search) 
                  
                  
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }



        public async Task<int> GetListingsCountAsync(ListingStatus? status = ListingStatus.Approved, string? search = null)
        {
            var query = _context.Listings
                .Include(l => l.Property)
                .ThenInclude(l => l.Owner)
                 .Include(l => l.Property)
                .ThenInclude(l => l.Address)
                .AsQueryable();

            

                         

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(l =>
                    
                    l.Description.Contains(search) ||
                    l.Property.Owner.Name.Contains(search) ||
                   
                    l.Property.Address.City.Contains(search) ||
                    l.Property.Address.Country.Contains(search)
                );
            }

            return await query.CountAsync();
        }

        public async Task<int> GetMembersCountAsync(UserStatus? status = UserStatus.Active, string? search = null)
        {
            var query = _context.Users.AsQueryable();

          

          

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    u.Name.Contains(search)
                 
               
                );
            }

            return await query.CountAsync();
        }

        public async Task<int> GetBookingsCountAsync(BookingStatus? status = BookingStatus.Pending, string? search = null)
        {
            var query = _context.Bookings.AsQueryable();


            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b =>
                    b.Listing.Property.PropertyName.Contains(search) ||
                    b.Guest.Name.Contains(search) 
                );
            }

            return await query.CountAsync();
        }
    }
}
