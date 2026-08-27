using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace Havenly.DAL.Repos.Implementations
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly HavenlyDbContext context;
        public PropertyRepository(HavenlyDbContext context) { this.context = context; }

        public async Task Add(Property entity)
        {
            try { await context.Properties.AddAsync(entity); await context.SaveChangesAsync(); Console.WriteLine("info:Property added successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public void Delete(Property entity)
        {
            try
            {
                // use provided entity directly
                entity.Delete();
                context.SaveChanges();
                Console.WriteLine("info:Property deleted successfully");
            }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<IEnumerable<Property>> Find(Expression<Func<Property, bool>> predicate)
        {
            try { var list = await context.Properties.Where(predicate).ToListAsync(); Console.WriteLine("info:Properties fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Property>(); }
        }

        public Task<Property?> Get(Func<Property, bool> predicate)
        {
            try { var e = context.Properties.AsEnumerable().FirstOrDefault(predicate); return Task.FromResult(e); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Task.FromResult<Property?>(null); }
        }

        public async Task<IEnumerable<Property>> GetAll()
        {
            try { var list = await context.Properties.ToListAsync(); Console.WriteLine("info:Properties fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Property>(); }
        }

        public async Task<Property?> GetById(long id)
        {
            try { var e = await context.Properties.FindAsync(id); Console.WriteLine("info:Property fetched successfully"); return e; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return null; }
        }

        public void Update(Property entity)
        {
            try
            {
                // update the passed entity directly
                entity.Update(entity.PropertyName, entity.Description, entity.NumberOfGuests, entity.Capacity, entity.BathroomCount);
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
