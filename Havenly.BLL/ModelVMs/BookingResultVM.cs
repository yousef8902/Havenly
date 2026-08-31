namespace Havenly.BLL.ModelVMs
{
    public class BookingResultVM
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public long? BookingID { get; set; }
    }
    public class BookingStats
    {
        public int Total { get; set; }
        public int Confirmed { get; set; }
        public int Pending { get; set; }
        public int Cancelled { get; set; }
        public int Completed { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageBookingValue { get; set; }
    }

    public class BookingFilters
    {
        public string Status { get; set; } = "all";
        public string SearchTerm { get; set; } = string.Empty;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        
    }
    public class AdminBookingsVM
    {
        public List<BookingDetailsVM> Bookings { get; set; } = new();
        public PaginationInfo Pagination { get; set; } = new();
        public BookingStats Stats { get; set; } = new();
        public BookingFilters Filters { get; set; } = new();
    }
}
