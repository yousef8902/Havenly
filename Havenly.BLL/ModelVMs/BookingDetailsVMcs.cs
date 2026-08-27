using System;
using System.Collections.Generic;
using System.Text;

namespace Havenly.BLL.ModelVMs
{
    public class BookingDetailsVM
    {
        public long BookingID { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Approved, etc.
    }
}
