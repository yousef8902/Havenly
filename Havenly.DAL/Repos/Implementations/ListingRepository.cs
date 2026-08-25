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
    public class ListingRepository : IListingRepository
    {
        private readonly HavenlyDbContext context;
        public ListingRepository(HavenlyDbContext context) { this.context = context; }

        public async Task Add(Listing entity)
        {
            try { await context.Listings.AddAsync(entity); await context.SaveChangesAsync(); Console.WriteLine("info:Listing added successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public void Delete(Listing entity)
        {
            try { context.Listings.Remove(entity); context.SaveChanges(); Console.WriteLine("info:Listing deleted successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<IEnumerable<Listing>> Find(Expression<Func<Listing, bool>> predicate)
        {
            try { var list = await context.Listings.Where(predicate).ToListAsync(); Console.WriteLine("info:Listings fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Listing>(); }
        }

        public Task<Listing?> Get(Func<Listing, bool> predicate)
        {
            try { var e = context.Listings.AsEnumerable().FirstOrDefault(predicate); return Task.FromResult(e); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Task.FromResult<Listing?>(null); }
        }

        public async Task<IEnumerable<Listing>> GetAll()
        {
            try { var list = await context.Listings.ToListAsync(); Console.WriteLine("info:Listings fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Listing>(); }
        }

        public async Task<Listing?> GetById(long id)
        {
            try { var e = await context.Listings.FindAsync(id); Console.WriteLine("info:Listing fetched successfully"); return e; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return null; }
        }

        public void Update(Listing entity)
        {
            try
            {
                // use provided entity directly
                entity.Update(entity.Description, entity.Price);
                context.SaveChanges();
            }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<int> SaveChanges()
        {
            try { return await context.SaveChangesAsync(); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return 0; }
        }
    }
}
