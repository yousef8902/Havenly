using System.ComponentModel.DataAnnotations;

namespace Havenly.BLL.ModelVMs
{
    public class SearchFilterVM
    {
        [Display(Name = "Destination")]
        public string? City { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Check-in")]
        public DateTime? CheckIn { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Check-out")]
        public DateTime? CheckOut { get; set; }

        [Range(1, 16)]
        public int Guests { get; set; } = 2;

        public string? Category { get; set; }

        [Display(Name = "Max price per night")]
        public int MaxPrice { get; set; } = 450;

        public string MinRooms { get; set; } = "any";

        public string MinRating { get; set; } = "any";

        public List<string> Amenities { get; set; } = [];

        public string Sort { get; set; } = "recommended";

        public int Page { get; set; } = 1;
    }
}
