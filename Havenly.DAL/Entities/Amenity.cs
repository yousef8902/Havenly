using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havenly.DAL.Entities;

public class Amenity
{
    [Key]
    public long AmenitiesID { get; private set; }

    [Required]
    [StringLength(100)]
    public string Name { get; private set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? AdditionalFees { get; private set; }

    public ICollection<PropertyAmenity> PropertyAmenities { get; private set; }

    public void Create(long amenitiesId, string name, decimal? additionalFees = null)
    {
        AmenitiesID = amenitiesId;
        Name = name;
        AdditionalFees = additionalFees;
    }

    public void Update(string name, decimal? additionalFees)
    {
        Name = name;
        AdditionalFees = additionalFees;
    }
}
