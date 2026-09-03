using System.ComponentModel.DataAnnotations;
using Havenly.DAL.Entities;
using Havenly.DAL.Enums;

namespace Havenly.BLL.ModelVMs
{
    public class HostPropertyFormVM
    {
        public long PropertyId { get; set; }

        [Required(ErrorMessage = "Property name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Property title")]
        public string PropertyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Detailed description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required")]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Street is required")]
        [StringLength(255)]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price per night is required")]
        [Range(10, 100000, ErrorMessage = "Price must be at least $10")]
        [Display(Name = "Nightly price ($)")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 50, ErrorMessage = "Guest capacity must be between 1 and 50")]
        [Display(Name = "Maximum guests")]
        public int NumberOfGuests { get; set; } = 2;

        [Required]
        [Range(1, 50, ErrorMessage = "Total capacity must be between 1 and 50")]
        [Display(Name = "Total bed capacity")]
        public int Capacity { get; set; } = 2;

        [Required]
        [Range(1, 20, ErrorMessage = "Bathroom count must be between 1 and 20")]
        [Display(Name = "Bathrooms count")]
        public int BathroomCount { get; set; } = 1;

        [Required(ErrorMessage = "Please select a category")]
        [Display(Name = "Category")]
        public string Category { get; set; } = "Design homes";

        public List<long> SelectedAmenityIds { get; set; } = new();
        public List<AmenityOptionVM> AvailableAmenities { get; set; } = new();

        public List<BedroomInputVM> Bedrooms { get; set; } = new();
        public List<PropertyImageItemVM> ExistingImages { get; set; } = new();
    }

    public class BedroomInputVM
    {
        public string RoomName { get; set; } = string.Empty;
        public BedType BedType { get; set; } = BedType.Double;
        public int BedQuantity { get; set; } = 1;
    }

    public class AmenityOptionVM
    {
        public long AmenityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class PropertyImageItemVM
    {
        public long ImageId { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }

    public class HostPropertiesPageVM
    {
        public List<HostPropertyCardVM> Properties { get; set; } = new();
    }

    public class HostPropertyCardVM
    {
        public long PropertyId { get; set; }
        public long ListingId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Guests { get; set; }
        public ListingStatus ListingStatus { get; set; }
        public bool IsValid { get; set; } = true;
        public int UpcomingBookingsCount { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public double Rating { get; set; }
        public long NumberOfReviews { get; set; }
    }
}
