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
    public class ReviewRepository : IReviewRepository
    {
        private readonly HavenlyDbContext context;
        public ReviewRepository(HavenlyDbContext context) { this.context = context; }

        public async Task Add(Review entity)
        {
            try 
            {
                await context.Reviews.AddAsync(entity);
                await context.SaveChangesAsync();
                Console.WriteLine("info:Review added successfully");
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }

        public void Delete(Review entity)
        {
            try { context.Reviews.Remove(entity); context.SaveChanges(); Console.WriteLine("info:Review deleted successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<IEnumerable<Review>> Find(Expression<Func<Review, bool>> predicate)
        {
            try { var list = await context.Reviews.Where(predicate).ToListAsync(); Console.WriteLine("info:Reviews fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Review>(); }
        }

        public Task<Review?> Get(Func<Review, bool> predicate)
        {
            try { var e = context.Reviews.AsEnumerable().FirstOrDefault(predicate); return Task.FromResult(e); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Task.FromResult<Review?>(null); }
        }

        public async Task<IEnumerable<Review>> GetAll()
        {
            try { var list = await context.Reviews.ToListAsync(); Console.WriteLine("info:Reviews fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Review>(); }
        }

        public async Task<Review?> GetById(long id)
        {
            try { var e = await context.Reviews.FindAsync(id); Console.WriteLine("info:Review fetched successfully"); return e; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return null; }
        }

        public void Update(Review entity)
        {
            try
            {
                // use provided entity directly
                entity.Update(entity.Rating, entity.Comment);
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
