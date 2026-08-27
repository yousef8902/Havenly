using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havenly.DAL.Entities;

public class Property
{

    [Key]
    public long PropertyID { get; private set; }

    public string OwnerUserID { get; private set; }
    [ForeignKey(nameof(OwnerUserID))]
    [InverseProperty("Properties")]
    public User Owner { get; private set; }

    public long AddressID { get; private set; }
    [ForeignKey(nameof(AddressID))]
    public Address Address { get; private set; }

    [Required]
    [StringLength(100)]
    public string PropertyName { get; private set; }

    public string Description { get; private set; }

    public int NumberOfGuests { get; private set; }
    public int Capacity { get; private set; }
    public int BathroomCount { get; private set; }
    public bool IsDeleted { get; private set; }

    // Navigation
    public ICollection<Bedroom> Bedrooms { get; private set; }
    public ICollection<PropertyImage> Images { get; private set; }
    public Listing Listing { get; private set; }
    public ICollection<PropertyAmenity> PropertyAmenities { get; private set; }

    public void Create(string ownerUserId, long addressId, string propertyName, string description, int numberOfGuests, int capacity, int bathroomCount)
    {
        OwnerUserID = ownerUserId;
        AddressID = addressId;
        PropertyName = propertyName;
        Description = description;
        NumberOfGuests = numberOfGuests;
        Capacity = capacity;
        BathroomCount = bathroomCount;
        IsDeleted = false;
        Bedrooms = new List<Bedroom>();
        Images = new List<PropertyImage>();
        PropertyAmenities = new List<PropertyAmenity>();
    }

    public void Update(string propertyName, string description, int numberOfGuests, int capacity, int bathroomCount)
    {
        PropertyName = propertyName;
        Description = description;
        NumberOfGuests = numberOfGuests;
        Capacity = capacity;
        BathroomCount = bathroomCount;
    }

    public void Delete()
    {
        IsDeleted = true;
    }

    public void AddBedroom(Bedroom bedroom) => Bedrooms.Add(bedroom);
    public void RemoveBedroom(Bedroom bedroom) => Bedrooms.Remove(bedroom);
    public void AddImage(PropertyImage image) => Images.Add(image);
    public void RemoveImage(PropertyImage image) => Images.Remove(image);
    public void AddAmenity(PropertyAmenity pa) => PropertyAmenities.Add(pa);
    public void RemoveAmenity(PropertyAmenity pa) => PropertyAmenities.Remove(pa);
}