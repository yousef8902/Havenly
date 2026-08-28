using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Entities;

namespace Havenly.DAL.Repos.Abstractions
{
    public interface IFavoriteRepository
    {
        Task<Favorite?> GetById(long id);
        Task<IEnumerable<Favorite>> GetAll();
        Task<IEnumerable<Favorite>> Find(Expression<Func<Favorite, bool>> predicate);
        Task<Favorite?> Get(Func<Favorite, bool> predicate);
        Task Add(Favorite entity);
        void Update(Favorite entity);
        void Delete(Favorite entity);
        Task<int> SaveChanges();
    }
}
