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
    public class AddressRepository : IAddressRepository
    {
        private readonly HavenlyDbContext context;
        public AddressRepository(HavenlyDbContext context) { this.context = context; }

        public async Task Add(Address entity)
        {
            try { await context.Addresses.AddAsync(entity); await context.SaveChangesAsync(); Console.WriteLine("info:Address added successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public void Delete(Address entity)
        {
            try { context.Addresses.Remove(entity); context.SaveChanges(); Console.WriteLine("info:Address deleted successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<IEnumerable<Address>> Find(Expression<Func<Address, bool>> predicate)
        {
            try { var list = await context.Addresses.Where(predicate).ToListAsync(); Console.WriteLine("info:Addresses fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Address>(); }
        }

        public Task<Address?> Get(Func<Address, bool> predicate)
        {
            try { var e = context.Addresses.AsEnumerable().FirstOrDefault(predicate); return Task.FromResult(e); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Task.FromResult<Address?>(null); }
        }

        public async Task<IEnumerable<Address>> GetAll()
        {
            try { var list = await context.Addresses.ToListAsync(); Console.WriteLine("info:Addresses fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Address>(); }
        }

        public async Task<Address?> GetById(long id)
        {
            try { var e = await context.Addresses.FindAsync(id); Console.WriteLine("info:Address fetched successfully"); return e; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return null; }
        }

        public void Update(Address entity)
        {
            try
            {
                // use provided entity directly
                entity.Update(entity.Country, entity.City, entity.Street, entity.Latitude, entity.Longitude);
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
