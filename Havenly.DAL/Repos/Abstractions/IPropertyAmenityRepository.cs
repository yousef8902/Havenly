using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Entities;

namespace Havenly.DAL.Repos.Abstractions
{
    public interface IPropertyAmenityRepository
    {
        Task<PropertyAmenity?> GetById(long id);
        Task<IEnumerable<PropertyAmenity>> GetAll();
        Task<IEnumerable<PropertyAmenity>> Find(Expression<Func<PropertyAmenity, bool>> predicate);
        Task<PropertyAmenity?> Get(Func<PropertyAmenity, bool> predicate);
        Task Add(PropertyAmenity entity);
        void Update(PropertyAmenity entity);
        void Delete(PropertyAmenity entity);
        Task<int> SaveChanges();
    }
}
