using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Entities;

[Index(nameof(PropertyID), IsUnique = true)]
public class Listing
{


    [Key]
    public long ListingID { get; private set; }

    public long PropertyID { get; private set; }
    [ForeignKey(nameof(PropertyID))]
    public Property Property { get; private set; }

    public string Description { get; private set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; private set; }

    public bool IsValid { get; private set; }

    public ICollection<Favorite> Favorites { get; private set; }
    public ICollection<Booking> Bookings { get; private set; }

    public void Create(long propertyId, string description, decimal price, bool isValid = true)
    {
        PropertyID = propertyId;
        Description = description;
        Price = price;
        IsValid = isValid;
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
}
