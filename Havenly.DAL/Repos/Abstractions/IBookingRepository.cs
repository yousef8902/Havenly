using Havenly.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havenly.DAL.Repos.Abstractions
{
   public interface IBookingRepository
    {
        public Task<IEnumerable<Booking>> GetActiveBookingsForListingAsync(long listingId, DateTime checkIn, DateTime checkOut);
        public Task AddBooking(Booking B);

        public IQueryable<Booking> GetAll();
    }
}
