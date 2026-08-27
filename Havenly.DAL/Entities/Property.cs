using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havenly.DAL.Entities;

public class Property
{
    [Key]
    public long PropertyID { get; set; }

    public string OwnerUserID { get; set; }
    [ForeignKey(nameof(OwnerUserID))]
    [InverseProperty("Properties")]
    public User Owner { get; set; }

    public long AddressID { get; set; }
    [ForeignKey(nameof(AddressID))]
    public Address Address { get; set; }

    [Required]
    [StringLength(100)]
    public string PropertyName { get; set; }

    public string Description { get; set; }

    public int NumberOfGuests { get; set; }
    public int Capacity { get; set; }
    public int BathroomCount { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation
    public ICollection<Bedroom> Bedrooms { get; set; }
    public ICollection<PropertyImage> Images { get; set; }
    public Listing Listing { get; set; }
    public ICollection<PropertyAmenity> PropertyAmenities { get; set; }   
}