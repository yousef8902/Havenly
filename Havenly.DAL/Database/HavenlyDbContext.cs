using Havenly.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Havenly.DAL.Database;

public class HavenlyDbContext : IdentityDbContext<User>
{
    public HavenlyDbContext(DbContextOptions<HavenlyDbContext> options) : base(options)
    {
    }
  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Fix multiple cascade paths by restricting GuestUser relationship
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Guest)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.GuestUserID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Favorite>()
           .HasOne(b => b.User)
           .WithMany(u => u.Favorites)
           .HasForeignKey(b => b.UserID)
           .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
          .HasOne(b => b.User)
          .WithMany(u => u.Reviews)
          .HasForeignKey(b => b.UserID)
          .OnDelete(DeleteBehavior.Restrict);

           modelBuilder.Entity<Review>().
           HasOne(r => r.Property)
          .WithMany(p => p.Reviews)
          .HasForeignKey(r => r.PropertyID)
          .OnDelete(DeleteBehavior.NoAction);   // or DeleteBehavior.NoAction

        // Notification configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.NotificationID);
            entity.HasOne(n => n.User)
                  .WithMany()
                  .HasForeignKey(n => n.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(n => new { n.UserID, n.IsRead, n.CreatedAt })
                  .HasDatabaseName("IX_Notifications_User_Read_Date");
        });

        // PropertyBlockedDate configuration
        modelBuilder.Entity<PropertyBlockedDate>(entity =>
        {
            entity.HasKey(b => b.BlockedDateID);
            entity.HasOne(b => b.Property)
                  .WithMany()
                  .HasForeignKey(b => b.PropertyID)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(b => new { b.PropertyID, b.StartDate, b.EndDate })
                  .HasDatabaseName("IX_PropertyBlockedDates_Property_Dates");
        });

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HavenlyDbContext).Assembly);
    }

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
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<PropertyBlockedDate> PropertyBlockedDates { get; set; }
}