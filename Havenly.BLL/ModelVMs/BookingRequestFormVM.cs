using System.ComponentModel.DataAnnotations;

namespace Havenly.BLL.ModelVMs
{
    public class BookingRequestFormVM
    {
        public string GuestUserID { get; set; }

        public long ListingID { get; set; }

        /// <summary>Lovable listings use slug ids (e.g. olive-ridge). Keep until ListingID is numeric.</summary>
        [Required]
        public string PropertySlug { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-in")]
        public DateTime CheckIn { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-out")]
        public DateTime CheckOut { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PricePerNight { get; set; }

        [Range(1, 16)]
        public int Guests { get; set; } = 2;

        public decimal Total { get; set; }
    }
}