using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Entities;

namespace Havenly.DAL.Repos.Abstractions
{
    public interface IReviewRepository
    {
        Task<Review?> GetById(long id);
        Task<IEnumerable<Review>> GetAll();
        Task<IEnumerable<Review>> Find(Expression<Func<Review, bool>> predicate);
        Task<Review?> Get(Func<Review, bool> predicate);
        Task Add(Review entity);
        void Update(Review entity);
        void Delete(Review entity);
        Task<bool> RespondToReview(long id, string response);
        Task<int> SaveChanges();

    }
}
