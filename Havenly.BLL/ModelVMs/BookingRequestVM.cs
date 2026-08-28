using System.ComponentModel.DataAnnotations;

namespace Havenly.BLL.ModelVMs
{
   
        public class BookingRequestVM : IValidatableObject
        {
            public long ListingID { get; set; }
            public string PropertyName { get; set; } = "Property";
            public decimal PricePerNight { get; set; }

            [Required(ErrorMessage = "Check-in date is required.")]
            [DataType(DataType.Date)]
            public DateTime CheckIn { get; set; } = DateTime.Today.AddDays(1);

            [Required(ErrorMessage = "Check-out date is required.")]
            [DataType(DataType.Date)]
            public DateTime CheckOut { get; set; } = DateTime.Today.AddDays(3);

            [Range(1, 16, ErrorMessage = "Please select at least 1 guest.")]
            public int Guests { get; set; } = 1;

            
            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (CheckIn.Date < DateTime.Today)
                {
                    yield return new ValidationResult("Check-in date cannot be in the past.", new[] { nameof(CheckIn) });
                }

                if (CheckOut.Date <= CheckIn.Date)
                {
                    yield return new ValidationResult("Check-out date must be at least one night after check-in.", new[] { nameof(CheckOut) });
                }
            }
        }
}
