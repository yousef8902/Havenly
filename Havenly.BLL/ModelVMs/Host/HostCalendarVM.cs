using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Havenly.BLL.ModelVMs.Host
{
    public class PropertySelectItemVM
    {
        public long PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class CalendarBookingEventVM
    {
        public long BookingId { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
    }

    public class CalendarBlockedEventVM
    {
        public long BlockedDateId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class CalendarDayCellVM
    {
        public int DayNumber { get; set; }
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public bool IsPast { get; set; }
        public bool IsBooked { get; set; }
        public bool IsBlocked { get; set; }
        public List<CalendarBookingEventVM> Bookings { get; set; } = new();
        public List<CalendarBlockedEventVM> BlockedRanges { get; set; } = new();
    }

    public class HostCalendarPageVM
    {
        public long PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public List<PropertySelectItemVM> HostProperties { get; set; } = new();
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public DateTime CurrentMonthDate { get; set; }
        public DateTime PrevMonthDate { get; set; }
        public DateTime NextMonthDate { get; set; }
        public List<CalendarDayCellVM> Days { get; set; } = new();
        public int TotalBookingsThisMonth { get; set; }
        public int TotalBlockedDaysThisMonth { get; set; }
    }

    public class BlockDateRequestVM
    {
        [Required]
        public long PropertyId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [MaxLength(200)]
        public string Reason { get; set; } = "Maintenance / Personal Use";
    }

    public class PropertyAvailabilityDatesVM
    {
        public long PropertyId { get; set; }
        public List<string> DisabledDates { get; set; } = new(); // "yyyy-MM-dd"
        public List<string> BookedRanges { get; set; } = new();   // "yyyy-MM-dd to yyyy-MM-dd"
        public List<string> BlockedRanges { get; set; } = new();  // "yyyy-MM-dd to yyyy-MM-dd"
    }
}
