using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Entities;

namespace Havenly.DAL.Repos.Abstractions
{
    public interface IBedRepository
    {
        Task<Bed?> GetById(long id);
        Task<IEnumerable<Bed>> GetAll();
        Task<IEnumerable<Bed>> Find(Expression<Func<Bed, bool>> predicate);
        Task<Bed?> Get(Func<Bed, bool> predicate);
        Task Add(Bed entity);
        void Update(Bed entity);
        void Delete(Bed entity);
        Task<int> SaveChanges();
    }
}
