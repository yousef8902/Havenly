using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Entities;

namespace Havenly.DAL.Repos.Abstractions
{
    public interface IPropertyImageRepository
    {
        Task<PropertyImage?> GetById(long id);
        Task<IEnumerable<PropertyImage>> GetAll();
        Task<IEnumerable<PropertyImage>> Find(Expression<Func<PropertyImage, bool>> predicate);
        Task<PropertyImage?> Get(Func<PropertyImage, bool> predicate);
        Task Add(PropertyImage entity);
        void Update(PropertyImage entity);
        void Delete(PropertyImage entity);
        Task<int> SaveChanges();
    }
}
