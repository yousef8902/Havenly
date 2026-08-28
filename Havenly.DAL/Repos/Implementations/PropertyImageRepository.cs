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
    public class PropertyImageRepository : IPropertyImageRepository
    {
        private readonly HavenlyDbContext context;
        public PropertyImageRepository(HavenlyDbContext context) { this.context = context; }

        public async Task Add(PropertyImage entity)
        {
            try { await context.PropertyImages.AddAsync(entity); await context.SaveChangesAsync(); Console.WriteLine("info:PropertyImage added successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public void Delete(PropertyImage entity)
        {
            try { context.PropertyImages.Remove(entity); context.SaveChanges(); Console.WriteLine("info:PropertyImage deleted successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<IEnumerable<PropertyImage>> Find(Expression<Func<PropertyImage, bool>> predicate)
        {
            try { var list = await context.PropertyImages.Where(predicate).ToListAsync(); Console.WriteLine("info:PropertyImages fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<PropertyImage>(); }
        }

        public Task<PropertyImage?> Get(Func<PropertyImage, bool> predicate)
        {
            try { var e = context.PropertyImages.AsEnumerable().FirstOrDefault(predicate); return Task.FromResult(e); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Task.FromResult<PropertyImage?>(null); }
        }

        public async Task<IEnumerable<PropertyImage>> GetAll()
        {
            try { var list = await context.PropertyImages.ToListAsync(); Console.WriteLine("info:PropertyImages fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<PropertyImage>(); }
        }

        public async Task<PropertyImage?> GetById(long id)
        {
            try { var e = await context.PropertyImages.FindAsync(id); Console.WriteLine("info:PropertyImage fetched successfully"); return e; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return null; }
        }

        public void Update(PropertyImage entity)
        {
            try
            {
                // use provided entity directly
                entity.UpdatePath(entity.ImagePath);
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
