using Havenly.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Database;

public class HavenlyDbContext : DbContext
{
    public HavenlyDbContext(DbContextOptions<HavenlyDbContext> options) : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }
    
    
    // --- DbSets ---
    public DbSet<User> Users { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Bedroom> Bedrooms { get; set; }
    public DbSet<Bed> Beds { get; set; }
    public DbSet<PropertyImage> PropertyImages { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Listing> Listings { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<PropertyAmenity> PropertyAmenities { get; set; }
    
    
}