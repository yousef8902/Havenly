using Havenly.BLL.ModelVMs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havenly.BLL.Services.Abstractions
{

    public interface IBookingService
    {
        Task<bool> IsPropertyAvailableAsync(long listingId, DateTime checkIn, DateTime checkOut);
        Task<BookingResultVM> CreateBookingAsync(BookingCreateVM dto);
        Task<IEnumerable<BookingDetailsVM>> GetBookingsByUserAsync(long userId);
    }
}