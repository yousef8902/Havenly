using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Repos.Abstractions;
using Havenly.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Havenly.BLL.ModelVMs;
using Havenly.DAL.Entities;
using AutoMapper;

namespace Havenly.BLL.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Checks date overlap 
        public async Task<bool> IsPropertyAvailableAsync(long listingId, DateTime checkIn, DateTime checkOut)
        {
            // Validation
            if (checkIn >= checkOut || checkIn.Date < DateTime.UtcNow.Date)
                return false;

            // Active booking statuses that lock the calendar
            var blockingStatuses = new[] { BookingStatus.Pending, BookingStatus.Approved };

            //  (Existing.CheckIn < Requested.CheckOut) AND (Existing.CheckOut > Requested.CheckIn)
            bool hasOverlap = await _unitOfWork.Bookings.GetAll()
                .AsNoTracking()
                .AnyAsync(b => b.ListingID == listingId
                            && blockingStatuses.Contains(b.Status)
                            && b.CheckIn < checkOut
                            && b.CheckOut > checkIn);

            return !hasOverlap;
        }

        public async Task<BookingResultVM> CreateBookingAsync(BookingCreateVM dto)
        {

            bool isAvailable = await IsPropertyAvailableAsync(dto.ListingID, dto.CheckIn, dto.CheckOut);
            if (!isAvailable)
            {
                return new BookingResultVM
                {
                    Success = false,
                    Message = "The selected dates are no longer available. Please choose different dates."
                };
            }

            // calc total nights and total price
            int totalNights = (dto.CheckOut.Date - dto.CheckIn.Date).Days;
            decimal totalPrice = totalNights * dto.PricePerNight;

            // Mapping
            var booking = new Booking
            {
                GuestUserID = dto.GuestUserID,
                ListingID = dto.ListingID,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                TotalPrice = totalPrice,
                Status = BookingStatus.Pending
            };

            //Save via Unit of Work
            await _unitOfWork.Bookings.AddBooking(booking);

            await _unitOfWork.SaveChangesAsync();

            return new BookingResultVM
            {
                Success = true,
                Message = "Booking request submitted successfully!",
                BookingID = booking.BookingID
            };
        }

        public async Task<IEnumerable<BookingDetailsVM>> GetBookingsByUserAsync(long userId)
        {
            var bookings = await _unitOfWork.Bookings.GetAll()
       .Include(b => b.Guest)                    // For guest name/email
       .Include(b => b.Listing)                  // For listing info
           .ThenInclude(l => l.Property)         // For property details
               .ThenInclude(p => p.Address)      // For city/country
       .Include(b => b.Listing)                  // For property images
           .ThenInclude(l => l.Property)
               .ThenInclude(p => p.Images)
       .Where(b => b.GuestUserID == userId)
       .OrderByDescending(b => b.CheckIn)
       .ToListAsync();

            return _mapper.Map<IEnumerable<BookingDetailsVM>>(bookings);
        }
    }
}