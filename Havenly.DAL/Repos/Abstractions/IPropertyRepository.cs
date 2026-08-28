using Havenly.DAL.Entities;
using System.Linq.Expressions;

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
        Task<Property?> GetDetailbyId(long id);
    }
    
}
