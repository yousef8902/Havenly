using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Entities;

[PrimaryKey(nameof(PropertyID), nameof(AmenitiesID))]
public class PropertyAmenity
{
    public long PropertyID { get; set; }
    [ForeignKey(nameof(PropertyID))]
    public Property Property { get; set; }

    public long AmenitiesID { get; set; }
    [ForeignKey(nameof(AmenitiesID))]
    public Amenity Amenity { get; set; }   
}