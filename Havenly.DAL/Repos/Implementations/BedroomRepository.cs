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
    public class BedroomRepository : IBedroomRepository
    {
        private readonly HavenlyDbContext context;
        public BedroomRepository(HavenlyDbContext context) { this.context = context; }

        public async Task Add(Bedroom entity)
        {
            try { await context.Bedrooms.AddAsync(entity); await context.SaveChangesAsync(); Console.WriteLine("info:Bedroom added successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public void Delete(Bedroom entity)
        {
            try { context.Bedrooms.Remove(entity); context.SaveChanges(); Console.WriteLine("info:Bedroom deleted successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<IEnumerable<Bedroom>> Find(Expression<Func<Bedroom, bool>> predicate)
        {
            try { var list = await context.Bedrooms.Where(predicate).ToListAsync(); Console.WriteLine("info:Bedrooms fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Bedroom>(); }
        }

        public Task<Bedroom?> Get(Func<Bedroom, bool> predicate)
        {
            try { var e = context.Bedrooms.AsEnumerable().FirstOrDefault(predicate); return Task.FromResult(e); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Task.FromResult<Bedroom?>(null); }
        }

        public async Task<IEnumerable<Bedroom>> GetAll()
        {
            try { var list = await context.Bedrooms.ToListAsync(); Console.WriteLine("info:Bedrooms fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Bedroom>(); }
        }

        public async Task<Bedroom?> GetById(long id)
        {
            try { var e = await context.Bedrooms.FindAsync(id); Console.WriteLine("info:Bedroom fetched successfully"); return e; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return null; }
        }

        public void Update(Bedroom entity)
        {
            try
            {
                // use provided entity directly
                entity.Update(entity.RoomNumber, entity.RoomName);
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
