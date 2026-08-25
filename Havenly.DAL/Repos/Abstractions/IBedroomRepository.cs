using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Entities;

namespace Havenly.DAL.Repos.Abstractions
{
    public interface IBedroomRepository
    {
        Task<Bedroom?> GetById(long id);
        Task<IEnumerable<Bedroom>> GetAll();
        Task<IEnumerable<Bedroom>> Find(Expression<Func<Bedroom, bool>> predicate);
        Task<Bedroom?> Get(Func<Bedroom, bool> predicate);
        Task Add(Bedroom entity);
        void Update(Bedroom entity);
        void Delete(Bedroom entity);
        Task<int> SaveChanges();
    }
}
