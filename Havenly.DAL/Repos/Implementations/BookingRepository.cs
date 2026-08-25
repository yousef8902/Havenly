using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Repos.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly HavenlyDbContext context;
        public BookingRepository(HavenlyDbContext context) { this.context = context; }

        public async Task Add(Booking entity)
        {
            try { await context.Bookings.AddAsync(entity); await context.SaveChangesAsync(); Console.WriteLine("info:Booking added successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public void Delete(Booking entity)
        {
            try { context.Bookings.Remove(entity); context.SaveChanges(); Console.WriteLine("info:Booking deleted successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<IEnumerable<Booking>> Find(Expression<Func<Booking, bool>> predicate)
        {
            try { var list = await context.Bookings.Where(predicate).ToListAsync(); Console.WriteLine("info:Bookings fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Booking>(); }
        }

        public Task<Booking?> Get(Func<Booking, bool> predicate)
        {
            try { var e = context.Bookings.AsEnumerable().FirstOrDefault(predicate); return Task.FromResult(e); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Task.FromResult<Booking?>(null); }
        }

        public async Task<IEnumerable<Booking>> GetAll()
        {
            try { var list = await context.Bookings.ToListAsync(); Console.WriteLine("info:Bookings fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Booking>(); }
        }

        public async Task<Booking?> GetById(long id)
        {
            try { var e = await context.Bookings.FindAsync(id); Console.WriteLine("info:Booking fetched successfully"); return e; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return null; }
        }

        public void Update(Booking entity)
        {
            try { context.Bookings.Update(entity); context.SaveChanges(); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<int> SaveChanges()
        {
            try { return await context.SaveChangesAsync(); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return 0; }
        }
    }
}
