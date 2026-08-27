using System.ComponentModel.DataAnnotations;

namespace Havenly.BLL.ModelVMs
{
    public class BookingRequestVM
    {
        public long ListingID { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage = "Please select a check-in date.")]
        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Please select a check-out date.")]
        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; } = DateTime.Today.AddDays(2);

        // Calculated client-side/server-side for summary preview
        public int TotalNights => (CheckOut > CheckIn) ? (CheckOut - CheckIn).Days : 0;
        public decimal TotalPrice => TotalNights * PricePerNight;
    }
}
