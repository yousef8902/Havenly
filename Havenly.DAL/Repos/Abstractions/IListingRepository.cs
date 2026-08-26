using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using System.Linq.Expressions;

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
        Task<bool> ApproveListing(long id);
        Task<bool> DeclineListing(long id);
        Task<IEnumerable<Listing>> GetByStatus(ListingStatus status);
    }
}
