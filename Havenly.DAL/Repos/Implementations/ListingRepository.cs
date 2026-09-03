using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<int> SaveChanges()
        {
            try { return await context.SaveChangesAsync(); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return 0; }
        }

        public async Task<bool> ApproveListing(long id)
        {
            try
            {
                var listing = await context.Listings.FindAsync(id);
                if (listing is null)
                {
                    Console.WriteLine("Error: Listing not found");
                    return false;
                }
                listing.Approve();
                await context.SaveChangesAsync();
                Console.WriteLine("info:Listing approved successfully");
                return true;

            }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message);return false; }
        }

        public async Task<bool> DeclineListing(long id)
        {
            try
            {
                var listing = await context.Listings.FindAsync(id);
                if (listing is null)
                {
                    Console.WriteLine("Error: Listing not found");
                    return false;
                }
                listing.Decline();
                await context.SaveChangesAsync();
                Console.WriteLine("info:Listing declined successfully");
                return true;
            }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return false; }
        }

        public async Task<IEnumerable<Listing>> GetByStatus(ListingStatus status)
        {
            try
            {
                var list = await  context.Listings
        .Include(l => l.Property)
            .ThenInclude(p => p.Address)
        .Include(l => l.Property)
            .ThenInclude(p => p.Images)
        .Include(l => l.Property)
 .Where(l => l.ListingStatus == status && l.IsValid && (l.Property == null || !l.Property.IsDeleted))
        .ToListAsync();
                Console.WriteLine("info:Listings fetched successfully");
                return list;
            }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Listing>(); }
        }
    }
}
