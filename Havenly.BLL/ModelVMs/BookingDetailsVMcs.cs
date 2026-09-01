using Havenly.DAL.Entities;
using Havenly.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havenly.BLL.ModelVMs
{
    public class BookingDetailsVM
    {
        public long BookingId { get; set; } 
        public long? PropertyId { get; set; } = 0;

        public string propertyName { get; set; } = "";
        public string Title { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public string? ImageUrl { get; set; }
        public string Guest { get; set; } = "";

        public string GuestID { get; set; } = "";
        public string GuestEmail { get; set; } = "";
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Guests { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } ="Pending";
        public string HostName { get;  set; } = string.Empty;
        public string? HostId { get;  set; }
        public int Nights { get; internal set; }
        public decimal Total { get; internal set; }
        public string StatusDisplay => Status.ToString();
        public string StatusColor => Status switch
        {
            "Approved" => "bg-emerald-100 text-emerald-800",
            "Pending" => "bg-yellow-100 text-yellow-800",
            "Cancelled" => "bg-red-100 text-red-800",
            "Completed"  => "bg-blue-100 text-blue-800",
            _ => "bg-gray-100 text-gray-800"
        };
    }
}
