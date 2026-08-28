using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Havenly.DAL.Enums;

namespace Havenly.DAL.Entities;

[Index(nameof(PropertyID), IsUnique = true)]
public class Listing
{


    [Key]
    public long    ListingID { get; private set; }

    public long PropertyID { get;  set; }
    [ForeignKey(nameof(PropertyID))]
    public Property Property { get;  set; }

    public string Description { get;  set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get;  set; }

    public bool IsValid { get;  set; }

    public ListingStatus ListingStatus { get;  set; }

    public ICollection<Favorite> Favorites { get;  set; }
    public ICollection<Booking> Bookings { get;  set; }

    public void Create(long propertyId, string description, decimal price, bool isValid = true)
    {
        PropertyID = propertyId;
        Description = description;
        Price = price;
        IsValid = isValid;
        this.ListingStatus = ListingStatus.Pending;
    }

    public void Update(string description, decimal price)
    {
        Description = description;
        Price = price;
    }

    public void SetValidity(bool isValid)
    {
        IsValid = isValid;
    }

    public void Approve() { 
    ListingStatus = ListingStatus.Approved;
    }

    public void Decline() { 
    ListingStatus= ListingStatus.Declined;
    }
}
