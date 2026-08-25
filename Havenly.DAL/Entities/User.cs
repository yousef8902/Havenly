using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havenly.DAL.Entities;

public class User
{

    [Key]
    public long UserID { get; private set; }

    [Required]
    [StringLength(100)]
    public string Name { get; private set; }

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; private set; }

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; private set; }

    [Required]
    [StringLength(50)]
    public string Role { get; private set; }

    public bool IsDeleted { get; private set; }

    // Navigation Properties
    [InverseProperty("Owner")]
    public ICollection<Property> Properties { get; private set; }

    [InverseProperty("Guest")]
    public ICollection<Booking> Bookings { get; private set; }

    public ICollection<Favorite> Favorites { get; private set; }
    public ICollection<Review> Reviews { get; private set; }

    public void Create(string name, string passwordHash, string email, string role)
    {
        Name = name;
        PasswordHash = passwordHash;
        Email = email;
        Role = role;
        IsDeleted = false;

        Properties = new List<Property>();
        Bookings = new List<Booking>();
        Favorites = new List<Favorite>();
        Reviews = new List<Review>();
    }

    public void Update(string name, string passwordHash, string email, string role)
    {
        Name = name;
        PasswordHash = passwordHash;
        Email = email;
        Role = role;
    }

    public void Delete()
    {
        IsDeleted = true;
    }


}
