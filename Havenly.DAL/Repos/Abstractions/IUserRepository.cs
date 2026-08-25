using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Entities;

namespace Havenly.DAL.Repos.Abstractions
{
    public interface IUserRepository
    {
        Task<User?> GetById(long id);
        Task<User?> GetByEmail(string email);
        Task<IEnumerable<User>> GetAll();

        Task<IEnumerable<User>> Find(Expression<Func<User, bool>> predicate);

        Task<User?> Get(Func<User, bool> predicate);
        Task Add(User user);
        void Update(User user);
        void Delete(User user);
        Task<int> SaveChanges();
    }
}
