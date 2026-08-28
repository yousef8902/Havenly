using System;
using System.Collections.Generic;
using System.Text;

namespace Havenly.BLL.ModelVMs
{
    public class BookingDetailsVM
    {
        public long BookingId { get; set; } 
        public long PropertyId { get; set; } = 0;
        public string Title { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string Guest { get; set; } = "";
        public string GuestEmail { get; set; } = "";
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Guests { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "";
    }
}
