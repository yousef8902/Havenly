
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havenly.DAL.Entities;

namespace Havenly.DAL.Repos.Abstractions
{
    public interface IBookingRepository
    {
        Task<Booking?> GetById(long id);
        //Task<IEnumerable<Booking>> GetAll();
        Task<IEnumerable<Booking>> Find(Expression<Func<Booking, bool>> predicate);
        Task<Booking?> Get(Func<Booking, bool> predicate);
       
        void Update(Booking entity);
        void Delete(Booking entity);
        Task<int> SaveChanges();
    
        public Task<IEnumerable<Booking>> GetActiveBookingsForListingAsync(long listingId, DateTime checkIn, DateTime checkOut);
        public Task AddBooking(Booking B);
       
        public IQueryable<Booking> GetAll();
    }
    

}
