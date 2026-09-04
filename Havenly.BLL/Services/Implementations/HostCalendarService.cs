using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Havenly.BLL.ModelVMs.Host;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace Havenly.BLL.Services.Implementations
{
    public class HostCalendarService : IHostCalendarService
    {
        private readonly HavenlyDbContext _context;

        public HostCalendarService(HavenlyDbContext context)
        {
            _context = context;
        }

        public async Task<HostCalendarPageVM> GetCalendarDataAsync(string hostUserId, long? propertyId, int? year, int? month)
        {
            var today = DateTime.UtcNow.Date;
            int targetYear = year ?? today.Year;
            int targetMonth = month ?? today.Month;

            if (targetMonth < 1)
            {
                targetMonth = 12;
                targetYear--;
            }
            else if (targetMonth > 12)
            {
                targetMonth = 1;
                targetYear++;
            }

            var currentMonthDate = new DateTime(targetYear, targetMonth, 1);
            var prevMonthDate = currentMonthDate.AddMonths(-1);
            var nextMonthDate = currentMonthDate.AddMonths(1);

            // Fetch host properties
            var properties = await _context.Properties
                .Include(p => p.Images)
                .Where(p => p.OwnerUserID == hostUserId)
                .OrderBy(p => p.PropertyName)
                .ToListAsync();

            if (!properties.Any())
            {
                return new HostCalendarPageVM
                {
                    Year = targetYear,
                    Month = targetMonth,
                    MonthName = currentMonthDate.ToString("MMMM yyyy"),
                    CurrentMonthDate = currentMonthDate,
                    PrevMonthDate = prevMonthDate,
                    NextMonthDate = nextMonthDate
                };
            }

            var selectedProperty = propertyId.HasValue
                ? properties.FirstOrDefault(p => p.PropertyID == propertyId.Value) ?? properties.First()
                : properties.First();

            var hostPropertySelectItems = properties.Select(p => new PropertySelectItemVM
            {
                PropertyId = p.PropertyID,
                PropertyName = p.PropertyName,
                ImageUrl = p.Images?.FirstOrDefault(i => i.IsPrimary == true)?.ImagePath
                           ?? p.Images?.FirstOrDefault()?.ImagePath
                           ?? "/images/p1.jpg",
                IsSelected = p.PropertyID == selectedProperty.PropertyID
            }).ToList();

            // Calendar grid calculation (Sunday start)
            int daysInMonth = DateTime.DaysInMonth(targetYear, targetMonth);
            var firstDayOfMonth = new DateTime(targetYear, targetMonth, 1);
            int startOffset = (int)firstDayOfMonth.DayOfWeek; // 0 = Sunday
            var gridStart = firstDayOfMonth.AddDays(-startOffset);
            var lastDayOfMonth = new DateTime(targetYear, targetMonth, daysInMonth);
            int endOffset = 6 - (int)lastDayOfMonth.DayOfWeek;
            var gridEnd = lastDayOfMonth.AddDays(endOffset);

            // Ensure at least 35 or 42 cells
            int totalDays = (gridEnd - gridStart).Days + 1;
            if (totalDays < 42 && totalDays <= 35)
            {
                gridEnd = gridEnd.AddDays(7);
                totalDays += 7;
            }

            // Query active bookings overlapping this range
            var bookings = await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Listing)
                .Where(b => b.Listing != null && b.Listing.PropertyID == selectedProperty.PropertyID
                         && b.Status != BookingStatus.Cancelled
                         && b.CheckIn <= gridEnd
                         && b.CheckOut >= gridStart)
                .ToListAsync();

            // Query blocked date ranges overlapping this range
            var blockedRanges = await _context.PropertyBlockedDates
                .Where(b => b.PropertyID == selectedProperty.PropertyID
                         && b.StartDate <= gridEnd
                         && b.EndDate >= gridStart)
                .ToListAsync();

            var days = new List<CalendarDayCellVM>();
            int bookingsThisMonth = 0;
            var bookedDaySet = new HashSet<DateTime>();
            var blockedDaySet = new HashSet<DateTime>();

            for (var d = gridStart; d <= gridEnd; d = d.AddDays(1))
            {
                var currentDate = d.Date;
                bool isCurrentMonth = currentDate.Month == targetMonth && currentDate.Year == targetYear;
                bool isToday = currentDate == today;
                bool isPast = currentDate < today;

                var matchingBookings = bookings.Where(b =>
                    currentDate >= b.CheckIn.Date && currentDate < b.CheckOut.Date
                ).Select(b => new CalendarBookingEventVM
                {
                    BookingId = b.BookingID,
                    GuestName = b.Guest?.Name ?? b.Guest?.UserName ?? "Guest",
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    Status = b.Status.ToString(),
                    StatusColor = b.Status switch
                    {
                        BookingStatus.Completed => "bg-emerald-100 text-emerald-800 dark:bg-emerald-950 dark:text-emerald-300",
                        BookingStatus.Approved => "bg-emerald-100 text-emerald-800 dark:bg-emerald-950 dark:text-emerald-300",
                        BookingStatus.Pending => "bg-blue-100 text-blue-800 dark:bg-blue-950 dark:text-blue-300",
                        _ => "bg-gray-100 text-gray-800"
                    }
                }).ToList();

                var matchingBlocked = blockedRanges.Where(b =>
                    currentDate >= b.StartDate.Date && currentDate <= b.EndDate.Date
                ).Select(b => new CalendarBlockedEventVM
                {
                    BlockedDateId = b.BlockedDateID,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Reason = b.Reason
                }).ToList();

                bool isBooked = matchingBookings.Any();
                bool isBlocked = matchingBlocked.Any();

                if (isCurrentMonth)
                {
                    if (isBooked) bookedDaySet.Add(currentDate);
                    if (isBlocked) blockedDaySet.Add(currentDate);
                }

                days.Add(new CalendarDayCellVM
                {
                    DayNumber = currentDate.Day,
                    Date = currentDate,
                    IsCurrentMonth = isCurrentMonth,
                    IsToday = isToday,
                    IsPast = isPast,
                    IsBooked = isBooked,
                    IsBlocked = isBlocked,
                    Bookings = matchingBookings,
                    BlockedRanges = matchingBlocked
                });
            }

            bookingsThisMonth = bookings.Count(b =>
                (b.CheckIn.Month == targetMonth && b.CheckIn.Year == targetYear) ||
                (b.CheckOut.Month == targetMonth && b.CheckOut.Year == targetYear));

            return new HostCalendarPageVM
            {
                PropertyId = selectedProperty.PropertyID,
                PropertyName = selectedProperty.PropertyName,
                HostProperties = hostPropertySelectItems,
                Year = targetYear,
                Month = targetMonth,
                MonthName = currentMonthDate.ToString("MMMM yyyy"),
                CurrentMonthDate = currentMonthDate,
                PrevMonthDate = prevMonthDate,
                NextMonthDate = nextMonthDate,
                Days = days,
                TotalBookingsThisMonth = bookingsThisMonth,
                TotalBlockedDaysThisMonth = blockedDaySet.Count
            };
        }

        public async Task<bool> BlockDatesAsync(string hostUserId, BlockDateRequestVM model)
        {
            if (model.StartDate.Date > model.EndDate.Date)
            {
                return false;
            }

            // Verify property belongs to host
            var property = await _context.Properties
                .FirstOrDefaultAsync(p => p.PropertyID == model.PropertyId && p.OwnerUserID == hostUserId);

            if (property == null) return false;

            // Check if dates conflict with any active booking
            var hasBookingConflict = await _context.Bookings
                .AnyAsync(b => b.Listing != null && b.Listing.PropertyID == model.PropertyId
                            && b.Status != BookingStatus.Cancelled
                            && b.CheckIn.Date <= model.EndDate.Date
                            && b.CheckOut.Date > model.StartDate.Date);

            if (hasBookingConflict)
            {
                return false; // Cannot block dates that already have confirmed/pending reservations
            }

            var blocked = new PropertyBlockedDate
            {
                PropertyID = model.PropertyId,
                StartDate = model.StartDate.Date,
                EndDate = model.EndDate.Date,
                Reason = string.IsNullOrWhiteSpace(model.Reason) ? "Maintenance / Personal Stay" : model.Reason.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.PropertyBlockedDates.Add(blocked);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnblockDatesAsync(string hostUserId, long blockedDateId)
        {
            var blocked = await _context.PropertyBlockedDates
                .Include(b => b.Property)
                .FirstOrDefaultAsync(b => b.BlockedDateID == blockedDateId && b.Property != null && b.Property.OwnerUserID == hostUserId);

            if (blocked == null) return false;

            _context.PropertyBlockedDates.Remove(blocked);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PropertyAvailabilityDatesVM> GetPropertyDisabledDatesAsync(long propertyId)
        {
            var today = DateTime.UtcNow.Date;

            // Active bookings from today forward
            var bookings = await _context.Bookings
                .Where(b => b.Listing != null && b.Listing.PropertyID == propertyId
                         && b.Status != BookingStatus.Cancelled
                         && b.CheckOut.Date >= today)
                .ToListAsync();

            // Active blocked ranges from today forward
            var blockedRanges = await _context.PropertyBlockedDates
                .Where(b => b.PropertyID == propertyId && b.EndDate.Date >= today)
                .ToListAsync();

            var disabledDateStrings = new HashSet<string>();
            var bookedRangeStrings = new List<string>();
            var blockedRangeStrings = new List<string>();

            foreach (var b in bookings)
            {
                bookedRangeStrings.Add($"{b.CheckIn:yyyy-MM-dd} to {b.CheckOut:yyyy-MM-dd}");
                for (var d = b.CheckIn.Date; d < b.CheckOut.Date; d = d.AddDays(1))
                {
                    if (d >= today)
                    {
                        disabledDateStrings.Add(d.ToString("yyyy-MM-dd"));
                    }
                }
            }

            foreach (var b in blockedRanges)
            {
                blockedRangeStrings.Add($"{b.StartDate:yyyy-MM-dd} to {b.EndDate:yyyy-MM-dd} ({b.Reason})");
                for (var d = b.StartDate.Date; d <= b.EndDate.Date; d = d.AddDays(1))
                {
                    if (d >= today)
                    {
                        disabledDateStrings.Add(d.ToString("yyyy-MM-dd"));
                    }
                }
            }

            return new PropertyAvailabilityDatesVM
            {
                PropertyId = propertyId,
                DisabledDates = disabledDateStrings.OrderBy(s => s).ToList(),
                BookedRanges = bookedRangeStrings,
                BlockedRanges = blockedRangeStrings
            };
        }
    }
}
