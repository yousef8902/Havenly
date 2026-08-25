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
    public class PropertyAmenityRepository : IPropertyAmenityRepository
    {
        private readonly HavenlyDbContext context;
        public PropertyAmenityRepository(HavenlyDbContext context) { this.context = context; }

        public async Task Add(PropertyAmenity entity)
        {
            try { await context.PropertyAmenities.AddAsync(entity); await context.SaveChangesAsync(); Console.WriteLine("info:PropertyAmenity added successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public void Delete(PropertyAmenity entity)
        {
            try { context.PropertyAmenities.Remove(entity); context.SaveChanges(); Console.WriteLine("info:PropertyAmenity deleted successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<IEnumerable<PropertyAmenity>> Find(Expression<Func<PropertyAmenity, bool>> predicate)
        {
            try { var list = await context.PropertyAmenities.Where(predicate).ToListAsync(); Console.WriteLine("info:PropertyAmenities fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<PropertyAmenity>(); }
        }

        public Task<PropertyAmenity?> Get(Func<PropertyAmenity, bool> predicate)
        {
            try { var e = context.PropertyAmenities.AsEnumerable().FirstOrDefault(predicate); return Task.FromResult(e); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Task.FromResult<PropertyAmenity?>(null); }
        }

        public async Task<IEnumerable<PropertyAmenity>> GetAll()
        {
            try { var list = await context.PropertyAmenities.ToListAsync(); Console.WriteLine("info:PropertyAmenities fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<PropertyAmenity>(); }
        }

        public async Task<PropertyAmenity?> GetById(long id)
        {
            try { var e = await context.PropertyAmenities.FindAsync(id); Console.WriteLine("info:PropertyAmenity fetched successfully"); return e; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return null; }
        }

        public void Update(PropertyAmenity entity)
        {
            try { context.PropertyAmenities.Update(entity); context.SaveChanges(); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<int> SaveChanges()
        {
            try { return await context.SaveChangesAsync(); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return 0; }
        }
    }
}
