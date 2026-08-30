using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havenly.DAL.Entities;

public class Property
{

    [Key]
    public long PropertyID { get;  set; }

    public string OwnerUserID { get;  set; }
    [ForeignKey(nameof(OwnerUserID))]
    [InverseProperty("Properties")]
    public User Owner { get;  set; }

    public long AddressID { get;  set; }
    [ForeignKey(nameof(AddressID))]
    public Address Address { get;  set; }

    [Required]
    [StringLength(100)]
    public string PropertyName { get;  set; }

    public string Description { get;  set; }
<<<<<<< HEAD

    public int NumberOfGuests { get;  set; }
    public int Capacity { get;  set; }
    public int BathroomCount { get;  set; }
    public bool IsDeleted { get;  set; }
=======

    public int NumberOfGuests { get;  set; }
    public int Capacity { get;  set; }
    public int BathroomCount { get;  set; }
    public bool IsDeleted { get;  set; }
    public double Rating { get; set; } = 0;
    public long NumberOfReviews { get; set; } = 0;

>>>>>>> 63b1b37 (add search by review and change relation between (review->booking) to (review->user))

    // Navigation
    public ICollection<Bedroom> Bedrooms { get;  set; }
    public ICollection<PropertyImage> Images { get;  set; }
    public Listing Listing { get;  set; }
    public ICollection<PropertyAmenity> PropertyAmenities { get;  set; }
<<<<<<< HEAD
=======
    public ICollection<Review>Reviews  { get; set; }

>>>>>>> 63b1b37 (add search by review and change relation between (review->booking) to (review->user))

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
    public void UpdateReview(double Rating)
    {
        this.NumberOfReviews++;
        this.Rating = (this.Rating+Rating)/this.NumberOfReviews;
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