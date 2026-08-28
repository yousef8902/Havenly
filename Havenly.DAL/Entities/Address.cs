using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havenly.DAL.Entities;

public class Address
{
    [Key]
    public long AddressID { get; private set; }

    [Required]
    [StringLength(100)]
    public string Country { get; private set; }

    [Required]
    [StringLength(100)]
    public string City { get; private set; }

    [Required]
    [StringLength(255)]
    public string Street { get; private set; }

    [Column(TypeName = "decimal(9,6)")]
    public decimal Latitude { get; private set; }

    [Column(TypeName = "decimal(9,6)")]
    public decimal Longitude { get; private set; }

    public void Create(long addressID, string country, string city, string street, decimal latitude, decimal longitude)
    {
        AddressID = addressID;
        Country = country;
        City = city;
        Street = street;
        Latitude = latitude;
        Longitude = longitude;
    }

    // Update method
    public void Update(string country, string city, string street, decimal latitude, decimal longitude)
    {
        Country = country;
        City = city;
        Street = street;
        Latitude = latitude;
        Longitude = longitude;
    }
}
    
