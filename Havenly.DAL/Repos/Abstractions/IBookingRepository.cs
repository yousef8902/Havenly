using Havenly.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Havenly.DAL.Repos.Abstractions
{
   public interface IBookingRepository
    {
        public Task<IEnumerable<Booking>> GetActiveBookingsForListingAsync(long listingId, DateTime checkIn, DateTime checkOut);
        public Task AddBooking(Booking B);
        Task<IEnumerable<Booking>> Find(Expression<Func<Booking, bool>> predicate);
        public IQueryable<Booking> GetAll();
    }
    
}
