
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.EntityFrameworkCore;

using Havenly.DAL.Enums;




namespace Havenly.DAL.Repos.Implementations
{
    public class BookingRepository : IBookingRepository
    {

        private readonly HavenlyDbContext _context;
        public BookingRepository(HavenlyDbContext context) { this._context = context; }

        public async Task Add(Booking entity)
        {
            try { await _context.Bookings.AddAsync(entity); await _context.SaveChangesAsync(); Console.WriteLine("info:Booking added successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public void Delete(Booking entity)
        {
            try { _context.Bookings.Remove(entity); _context.SaveChanges(); Console.WriteLine("info:Booking deleted successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

       

        public Task<Booking?> Get(Func<Booking, bool> predicate)
        {
            try { var e = _context.Bookings.AsEnumerable().FirstOrDefault(predicate); return Task.FromResult(e); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Task.FromResult<Booking?>(null); }
        }

        //public async Task<IEnumerable<Booking>> GetAll()
        //{
        //    try { var list = await _context.Bookings.ToListAsync(); Console.WriteLine("info:Bookings fetched successfully"); return list; }
        //    catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Booking>(); }
        //}

        public async Task<Booking?> GetById(long id)
        {
            try { var e = await _context.Bookings.FindAsync(id); Console.WriteLine("info:Booking fetched successfully"); return e; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return null; }
        }

        public void Update(Booking entity)
        {
            try { _context.Bookings.Update(entity); _context.SaveChanges(); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<int> SaveChanges()
        {
            try { return await _context.SaveChangesAsync(); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return 0; }


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
