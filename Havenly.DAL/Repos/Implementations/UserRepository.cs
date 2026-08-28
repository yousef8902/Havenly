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
    public class UserRepository : IUserRepository
    {
        private readonly HavenlyDbContext context;
        public UserRepository(HavenlyDbContext context) { this.context = context; }

        public async Task Add(User user)
        {
            try {
                Console.WriteLine("info:try to add User to database ");

                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                Console.WriteLine("info:User added successfully");
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Error:" + ex.Message); 
            }
        }

        public void Delete(User user)
        {
            try
            {
                //var u = context.Users.Where(x => x.UserID == user.UserID).FirstOrDefault();
                if (user != null)
                {
                    user.Delete();
                    context.SaveChanges();
                    Console.WriteLine("info:User deleted successfully");
                }
            }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<IEnumerable<User>> Find(Expression<Func<User, bool>> predicate)
        {
            try 
            { 
                var list = await context.Users.Where(predicate).ToListAsync();
                Console.WriteLine("info:Users fetched successfully"); return list;
            }
            catch (Exception ex) 
            { 
                Console.WriteLine("Error:" + ex.Message);
                return Enumerable.Empty<User>();
            }
        }

        public Task<User?> Get(Func<User, bool> predicate)
        {
            try { 
                var e = context.Users.AsEnumerable().FirstOrDefault(predicate);
                return Task.FromResult(e); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Task.FromResult<User?>(null); }
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            try { var list = await context.Users.ToListAsync(); Console.WriteLine("info:Users fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<User>(); }
        }

        public async Task<User?> GetById(string id)
        {
            try { 
                var e = await context.Users.FindAsync(id);
                Console.WriteLine("info:User fetched successfully");
                return e;
            }
            catch (Exception ex) 
            { 
                Console.WriteLine("Error:" + ex.Message); 
                return null; 
            }
        }

        public async Task<User?> GetByEmail(string email)
        {
            try { 
                var e = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
                return e; 
            }
            catch (Exception ex) { 
                Console.WriteLine("Error:" + ex.Message); return null;
            }
        }

        public async Task<int> SaveChanges()
        {
            try { return await context.SaveChangesAsync(); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return 0; }
        }

        public void Update(User user)
        {
            try
            {
                // assume caller provided a tracked entity; update domain state and save
                user.Update(user.Name, user.PasswordHash, user.Email, user.Role);
                context.SaveChanges();
            }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }
    }
}
