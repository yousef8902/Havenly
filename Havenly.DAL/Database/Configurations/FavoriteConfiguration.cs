using Havenly.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Havenly.DAL.Database.Configurations
{
    public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.HasKey(f => f.FavoriteID);

            // Favorite -> User
            builder.HasOne(f => f.User)
                   .WithMany(u => u.Favorites)
                   .HasForeignKey(f => f.UserID)
                   .OnDelete(DeleteBehavior.Restrict); 

           
        }
    }
}
