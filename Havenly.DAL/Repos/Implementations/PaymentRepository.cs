using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Havenly.DAL.Repos.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Repos.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly HavenlyDbContext context;
        public PaymentRepository(HavenlyDbContext context) { this.context = context; }

        public async Task Add(Payment entity)
        {
            try { await context.Payments.AddAsync(entity); await context.SaveChangesAsync(); Console.WriteLine("info:Payment added successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public void Delete(Payment entity)
        {
            try { context.Payments.Remove(entity); context.SaveChanges(); Console.WriteLine("info:Payment deleted successfully"); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<IEnumerable<Payment>> Find(Expression<Func<Payment, bool>> predicate)
        {
            try { var list = await context.Payments.Where(predicate).ToListAsync(); Console.WriteLine("info:Payments fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Payment>(); }
        }

        public Task<Payment?> Get(Func<Payment, bool> predicate)
        {
            try { var e = context.Payments.AsEnumerable().FirstOrDefault(predicate); return Task.FromResult(e); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Task.FromResult<Payment?>(null); }
        }

        public async Task<IEnumerable<Payment>> GetAll()
        {
            try { var list = await context.Payments.ToListAsync(); Console.WriteLine("info:Payments fetched successfully"); return list; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return Enumerable.Empty<Payment>(); }
        }

        public async Task<Payment?> GetById(long id)
        {
            try { var e = await context.Payments.FindAsync(id); Console.WriteLine("info:Payment fetched successfully"); return e; }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return null; }
        }

        public void Update(Payment entity)
        {
            try
            {
                // use provided entity directly
                entity.Update(entity.Gateway, entity.Amount, entity.TransactionID);
                context.SaveChanges();
            }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); }
        }

        public async Task<int> SaveChanges()
        {
            try { return await context.SaveChangesAsync(); }
            catch (Exception ex) { Console.WriteLine("Error:" + ex.Message); return 0; }
        }
    }
}
