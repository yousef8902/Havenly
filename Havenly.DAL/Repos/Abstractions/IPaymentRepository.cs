using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Entities;

namespace Havenly.DAL.Repos.Abstractions
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetById(long id);
        Task<IEnumerable<Payment>> GetAll();
        Task<IEnumerable<Payment>> Find(Expression<Func<Payment, bool>> predicate);
        Task<Payment?> Get(Func<Payment, bool> predicate);
        Task Add(Payment entity);
        void Update(Payment entity);
        void Delete(Payment entity);
        Task<int> SaveChanges();
    }
}
