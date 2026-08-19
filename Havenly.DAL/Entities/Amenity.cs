using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havenly.DAL.Entities;

public class Amenity
{
    [Key]
    public long AmenitiesID { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? AdditionalFees { get; set; }
        
    public ICollection<PropertyAmenity> PropertyAmenities { get; set; }   
}