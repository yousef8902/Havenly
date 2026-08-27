using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Entities;

namespace Havenly.DAL.Repos.Abstractions
{
    public interface IAddressRepository
    {
        Task<Address?> GetById(long id);
        Task<IEnumerable<Address>> GetAll();
        Task<IEnumerable<Address>> Find(Expression<Func<Address, bool>> predicate);
        Task<Address?> Get(Func<Address, bool> predicate);
        Task Add(Address entity);
        void Update(Address entity);
        void Delete(Address entity);
        Task<int> SaveChanges();
    }
}
