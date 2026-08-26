using System;
using System.Collections.Generic;
using System.Text;

namespace Havenly.BLL.ModelVMs
{
    public class BookingCreateVM
    {
      
            public long GuestUserID { get; set; }
            public long ListingID { get; set; }
            public DateTime CheckIn { get; set; }
            public DateTime CheckOut { get; set; }
            public decimal PricePerNight { get; set; }
        
    }
}
