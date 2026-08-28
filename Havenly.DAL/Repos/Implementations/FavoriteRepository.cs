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
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly HavenlyDbContext context;
        public FavoriteRepository(HavenlyDbContext context) { this.context = context; }

        public async Task Add(Favorite entity)
        {
            try { await context.Favorites.AddAsync(entity); await context.SaveChangesAsync(); Console.WriteLine("info:Favorite added successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public void Delete(Favorite entity)
        {
            try { context.Favorites.Remove(entity); context.SaveChanges(); Console.WriteLine("info:Favorite deleted successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<IEnumerable<Favorite>> Find(Expression<Func<Favorite, bool>> predicate)
        {
            try { var list = await context.Favorites.Where(predicate).ToListAsync(); Console.WriteLine("info:Favorites fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Favorite>(); }
        }

        public Task<Favorite?> Get(Func<Favorite, bool> predicate)
        {
            try { var e = context.Favorites.AsEnumerable().FirstOrDefault(predicate); return Task.FromResult(e); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Task.FromResult<Favorite?>(null); }
        }

        public async Task<IEnumerable<Favorite>> GetAll()
        {
            try { var list = await context.Favorites.ToListAsync(); Console.WriteLine("info:Favorites fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Favorite>(); }
        }

        public async Task<Favorite?> GetById(long id)
        {
            try { var e = await context.Favorites.FindAsync(id); Console.WriteLine("info:Favorite fetched successfully"); return e; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return null; }
        }

        public void Update(Favorite entity)
        {
            try { context.Favorites.Update(entity); context.SaveChanges(); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<int> SaveChanges()
        {
            try { return await context.SaveChangesAsync(); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return 0; }
        }
    }
}
