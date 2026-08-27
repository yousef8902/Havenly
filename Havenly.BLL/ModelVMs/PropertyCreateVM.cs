using System.ComponentModel.DataAnnotations;

namespace Havenly.BLL.ModelVMs
{
    public class PropertyCreateVM
    {
        [Required]
        [Display(Name = "Listing title")]
        public string Title { get; set; } = "";

        [Required]
        public string Category { get; set; } = "";

        [Required]
        public string City { get; set; } = "";

        [Required]
        public string Country { get; set; } = "";

        [Required]
        [MinLength(40, ErrorMessage = "Descriptions need at least 40 characters.")]
        public string Description { get; set; } = "";

        [Range(1, 32)]
        public int Guests { get; set; } = 4;

        [Range(1, 20)]
        public int Bedrooms { get; set; } = 2;

        [Range(1, 20)]
        public int Beds { get; set; } = 2;

        [Range(1, 20)]
        public int Baths { get; set; } = 1;

        public List<string> Amenities { get; set; } = [];

        [Display(Name = "Nightly price (USD)")]
        [Range(1, 100000)]
        public decimal Price { get; set; } = 180;

        [Display(Name = "Cleaning fee (USD)")]
        [Range(0, 100000)]
        public decimal Cleaning { get; set; } = 60;
    }
}