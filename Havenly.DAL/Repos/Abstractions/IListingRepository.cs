using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Entities;

namespace Havenly.DAL.Repos.Abstractions
{
    public interface IListingRepository
    {
        Task<Listing?> GetById(long id);
        Task<IEnumerable<Listing>> GetAll();
        Task<IEnumerable<Listing>> Find(Expression<Func<Listing, bool>> predicate);
        Task<Listing?> Get(Func<Listing, bool> predicate);
        Task Add(Listing entity);
        void Update(Listing entity);
        void Delete(Listing entity);
        Task<int> SaveChanges();
    }
}
