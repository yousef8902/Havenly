using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Entities;

namespace Havenly.DAL.Repos.Abstractions
{
    public interface IPropertyRepository
    {
        Task<Property?> GetById(long id);
        Task<IEnumerable<Property>> GetAll();
        Task<IEnumerable<Property>> Find(Expression<Func<Property, bool>> predicate);
        Task<Property?> Get(Func<Property, bool> predicate);
        Task Add(Property entity);
        void Update(Property entity);
        void Delete(Property entity);
        Task<int> SaveChanges();
    }
}
