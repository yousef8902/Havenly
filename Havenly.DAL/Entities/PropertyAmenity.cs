using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Entities;

[PrimaryKey(nameof(PropertyID), nameof(AmenitiesID))]
public class PropertyAmenity
{
    public long PropertyID { get; private set; }
    [ForeignKey(nameof(PropertyID))]
    public Property Property { get; private set; }

    public long AmenitiesID { get; private set; }
    [ForeignKey(nameof(AmenitiesID))]
    public Amenity Amenity { get; private set; }

    public void Create(long propertyId, long amenityId)
    {
        PropertyID = propertyId;
        AmenitiesID = amenityId;
    }
}
