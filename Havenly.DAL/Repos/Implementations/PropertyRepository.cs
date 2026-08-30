
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

        public async Task<IEnumerable<Property>> GetByOwner(string ownerUserId)
        {
            try
            {
                var list = await context.Properties
                    .Where(property => property.OwnerUserID == ownerUserId && !property.IsDeleted)
                    .Include(property => property.Address)
                    .Include(property => property.Listing)
                    .Include(property => property.Images)
                    .ToListAsync();

                Console.WriteLine("info:Host properties fetched successfully");
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
                return Enumerable.Empty<Property>();
            }
        }

        public async Task<Property?> GetPropertyDetails(long id)
        {
            try
            {
                // Property management needs the complete graph for details and updates.
                return await context.Properties
                    .Include(property => property.Address)
                    .Include(property => property.Listing)
                    .Include(property => property.Images)
                    .Include(property => property.PropertyAmenities)
                        .ThenInclude(propertyAmenity => propertyAmenity.Amenity)
                    .Include(property => property.Bedrooms)
                        .ThenInclude(bedroom => bedroom.Beds)
                    .FirstOrDefaultAsync(property => property.PropertyID == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
                return null;
            }
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

        

        public async  Task<Property?> GetDetailbyId(long id)
        {
            return await context.Properties
       .Include(p => p.Listing)
       .Include(p => p.Images)
       .Include(p=>p.PropertyAmenities)
       .Include(p=>p.Address)
       .FirstOrDefaultAsync(p => p.PropertyID == id);
        }
    }
}
