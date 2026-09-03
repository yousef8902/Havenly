using AutoMapper;
using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Havenly.DAL.Repos.Abstractions;
using Havenly.DAL.Repos.Implementations;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace Havenly.BLL.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IListingRepository _listingRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IEmailServices _emailServices;

        public BookingService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IListingRepository listingRepository,
            IPropertyRepository propertyRepository,
            IEmailServices emailServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _listingRepository = listingRepository;
            _propertyRepository = propertyRepository;
            _emailServices = emailServices;
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
            if (dto.CheckIn.Date < DateTime.Today)
            {
                return new BookingResultVM
                {
                    Success = false,
                    Message = "Check-in date cannot be in the past."
                };
            }

            if (dto.CheckOut.Date <= dto.CheckIn.Date)
            {
                return new BookingResultVM
                {
                    Success = false,
                    Message = "Check-out date must be at least one day after check-in."
                };
            }
            bool isAvailable = await IsPropertyAvailableAsync(dto.ListingID, dto.CheckIn, dto.CheckOut);
            if (!isAvailable)
            {
                return new BookingResultVM
                {
                    Success = false,
                    Message = "The selected dates are no longer available. Please choose different dates."
                };
            }

            var listing = await _listingRepository.GetById(dto.ListingID);
            if (listing == null || !listing.IsValid || listing.ListingStatus != ListingStatus.Approved)
            {
                return new BookingResultVM
                {
                    Success = false,
                    Message = "This listing is currently suspended by the host and not accepting new reservations."
                };
            }

            var property = await _propertyRepository.GetById(listing.PropertyID);
            if (property != null)
            {
                if (property.IsDeleted)
                {
                    return new BookingResultVM
                    {
                        Success = false,
                        Message = "This listing is no longer available."
                    };
                }

                if (string.Equals(property.OwnerUserID, dto.GuestUserID, StringComparison.OrdinalIgnoreCase))
                {
                    return new BookingResultVM
                    {
                        Success = false,
                        Message = "Hosts cannot make reservations for their own properties."
                    };
                }
            }

            // calc total nights and total price from authoritative database listing price
            decimal nightlyPrice = listing.Price > 0 ? listing.Price : dto.PricePerNight;
            int totalNights = (dto.CheckOut.Date - dto.CheckIn.Date).Days;
            decimal subtotal = totalNights * nightlyPrice;
            decimal serviceFee = Math.Round(subtotal * 0.09m, 2);
            decimal totalPrice = subtotal + serviceFee;

            // Mapping
            var booking = new Booking( );
            booking.Create(dto.GuestUserID, dto.ListingID, dto.CheckIn, dto.CheckOut, totalPrice, BookingStatus.Pending);


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

        public async Task<IEnumerable<BookingDetailsVM>> GetBookingsByUserAsync(string userId)
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

        public async Task<bool> CancelBookingAsync( long bookingId, string guestUserId)
        {
            var bookingList = await _unitOfWork.Bookings.Find(b => b.BookingID == bookingId && b.GuestUserID == guestUserId);
            var booking = bookingList.FirstOrDefault();

            
            if (booking == null || booking.Status == BookingStatus.Cancelled)
            {
                return false;
            }

            
            booking.UpdateStatus(BookingStatus.Cancelled);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<int> ProcessAutomaticCheckoutsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var pastApprovedBookings = await _unitOfWork.Bookings.GetAll()
                    .Include(b => b.Guest)
                    .Include(b => b.Listing)
                        .ThenInclude(l => l.Property)
                            .ThenInclude(p => p.Owner)
                    .Where(b => b.Status == BookingStatus.Approved && b.CheckOut <= now)
                    .ToListAsync();

                if (!pastApprovedBookings.Any())
                    return 0;

                int completedCount = 0;
                foreach (var booking in pastApprovedBookings)
                {
                    booking.UpdateStatus(BookingStatus.Completed);
                    completedCount++;

                    // Send email review invitation to guest
                    try
                    {
                        if (booking.Guest != null && !string.IsNullOrEmpty(booking.Guest.Email))
                        {
                            var hostName = booking.Listing?.Property?.Owner?.Name ?? "Your Host";
                            var propName = booking.Listing?.Property?.PropertyName ?? "your stay";
                            await _emailServices.SendReviewInvitationToGuestAsync(
                                booking.Guest.Email,
                                booking.Guest.Name ?? "Traveler",
                                hostName,
                                propName,
                                booking.BookingID);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[AUTO CHECKOUT EMAIL ERROR] {ex.Message}");
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return completedCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AUTO CHECKOUT ERROR] {ex.Message}");
                return 0;
            }
        }
    }
}