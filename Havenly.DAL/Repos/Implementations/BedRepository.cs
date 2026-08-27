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
    public class BedRepository : IBedRepository
    {
        private readonly HavenlyDbContext context;
        public BedRepository(HavenlyDbContext context) { this.context = context; }

        public async Task Add(Bed entity)
        {
            try { await context.Beds.AddAsync(entity); await context.SaveChangesAsync(); Console.WriteLine("info:Bed added successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public void Delete(Bed entity)
        {
            try { context.Beds.Remove(entity); context.SaveChanges(); Console.WriteLine("info:Bed deleted successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<IEnumerable<Bed>> Find(Expression<Func<Bed, bool>> predicate)
        {
            try { var list = await context.Beds.Where(predicate).ToListAsync(); Console.WriteLine("info:Beds fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Bed>(); }
        }

        public Task<Bed?> Get(Func<Bed, bool> predicate)
        {
            try { var e = context.Beds.AsEnumerable().FirstOrDefault(predicate); return Task.FromResult(e); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Task.FromResult<Bed?>(null); }
        }

        public async Task<IEnumerable<Bed>> GetAll()
        {
            try { var list = await context.Beds.ToListAsync(); Console.WriteLine("info:Beds fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Bed>(); }
        }

        public async Task<Bed?> GetById(long id)
        {
            try { var e = await context.Beds.FindAsync(id); Console.WriteLine("info:Bed fetched successfully"); return e; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return null; }
        }

        public void Update(Bed entity)
        {
            try
            {
                // use provided entity directly
                entity.Update(entity.BedType, entity.Quantity);
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
