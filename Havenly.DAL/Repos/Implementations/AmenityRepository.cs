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
    public class AmenityRepository : IAmenityRepository
    {
        private readonly HavenlyDbContext context;
        public AmenityRepository(HavenlyDbContext context) { this.context = context; }

        public async Task Add(Amenity entity)
        {
            try { await context.Amenities.AddAsync(entity); await context.SaveChangesAsync(); Console.WriteLine("info:Amenity added successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public void Delete(Amenity entity)
        {
            try { context.Amenities.Remove(entity); context.SaveChanges(); Console.WriteLine("info:Amenity deleted successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<IEnumerable<Amenity>> Find(Expression<Func<Amenity, bool>> predicate)
        {
            try { var list = await context.Amenities.Where(predicate).ToListAsync(); Console.WriteLine("info:Amenities fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Amenity>(); }
        }

        public Task<Amenity?> Get(Func<Amenity, bool> predicate)
        {
            try { var e = context.Amenities.AsEnumerable().FirstOrDefault(predicate); return Task.FromResult(e); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Task.FromResult<Amenity?>(null); }
        }

        public async Task<IEnumerable<Amenity>> GetAll()
        {
            try { var list = await context.Amenities.ToListAsync(); Console.WriteLine("info:Amenities fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Amenity>(); }
        }

        public async Task<Amenity?> GetById(long id)
        {
            try { var e = await context.Amenities.FindAsync(id); Console.WriteLine("info:Amenity fetched successfully"); return e; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return null; }
        }

        public void Update(Amenity entity)
        {
            try
            {
                // use provided entity directly
                entity.Update(entity.Name, entity.AdditionalFees);
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
