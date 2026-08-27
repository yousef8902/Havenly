using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace Havenly.DAL.Repos.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly HavenlyDbContext _context;

        public BookingRepository(HavenlyDbContext context)
        {
            _context = context;
        }

        // Fetches only Pending and Approved bookings that overlap with the requested dates
        public async Task<IEnumerable<Booking>> GetActiveBookingsForListingAsync(long listingId, DateTime checkIn, DateTime checkOut)
        {
            var activeStatuses = new[] { BookingStatus.Pending, BookingStatus.Approved };

            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.ListingID == listingId
                         && activeStatuses.Contains(b.Status)
                         && b.CheckIn < checkOut
                         && b.CheckOut > checkIn)
                .ToListAsync();
        }

        public async Task AddBooking(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        public IQueryable<Booking> GetAll()
        {
            return _context.Bookings.AsQueryable();
        }

        public async Task<IEnumerable<Booking>> Find(Expression<Func<Booking, bool>> predicate)
        {
            try
            {
                return await _context.Bookings
                    .Where(predicate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Enumerable.Empty<Booking>();
            }
        }
    }
}
