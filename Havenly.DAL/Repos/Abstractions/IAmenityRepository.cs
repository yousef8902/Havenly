using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Entities;

namespace Havenly.DAL.Repos.Abstractions
{
    public interface IAmenityRepository
    {
        Task<Amenity?> GetById(long id);
        Task<IEnumerable<Amenity>> GetAll();
        Task<IEnumerable<Amenity>> Find(Expression<Func<Amenity, bool>> predicate);
        Task<Amenity?> Get(Func<Amenity, bool> predicate);
        Task Add(Amenity entity);
        void Update(Amenity entity);
        void Delete(Amenity entity);
        Task<int> SaveChanges();
    }
}
