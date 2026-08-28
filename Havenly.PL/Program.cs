
using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

using Havenly.BLL.Services.Abstractions;
using Havenly.BLL.Services.Implementations;
using Havenly.BLL.Mappers;

using Havenly.DAL.Entities;
using Havenly.DAL.Database;
using Havenly.DAL.Repos.Abstractions;
using Havenly.DAL.Repos.Implementations;
using Havenly.DAL.Database.Seed;

namespace Havenly.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

           
            builder.Services.AddDbContext<HavenlyDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
            })
          .AddEntityFrameworkStores<HavenlyDbContext>()
          .AddDefaultTokenProviders();

            // Register repository implementations
            builder.Services.AddScoped<Havenly.DAL.Repos.Abstractions.IUserRepository, Havenly.DAL.Repos.Implementations.UserRepository>();
            builder.Services.AddScoped<Havenly.DAL.Repos.Abstractions.IPropertyRepository, Havenly.DAL.Repos.Implementations.PropertyRepository>();
            builder.Services.AddScoped<Havenly.DAL.Repos.Abstractions.IBookingRepository, Havenly.DAL.Repos.Implementations.BookingRepository>();
            builder.Services.AddScoped<Havenly.DAL.Repos.Abstractions.IFavoriteRepository, Havenly.DAL.Repos.Implementations.FavoriteRepository>();
            builder.Services.AddScoped<Havenly.DAL.Repos.Abstractions.IReviewRepository, Havenly.DAL.Repos.Implementations.ReviewRepository>();
            builder.Services.AddScoped<Havenly.DAL.Repos.Abstractions.IListingRepository, Havenly.DAL.Repos.Implementations.ListingRepository>();
            builder.Services.AddScoped<Havenly.DAL.Repos.Abstractions.IAddressRepository, Havenly.DAL.Repos.Implementations.AddressRepository>();
            builder.Services.AddScoped<Havenly.DAL.Repos.Abstractions.IPaymentRepository, Havenly.DAL.Repos.Implementations.PaymentRepository>();
            builder.Services.AddScoped<Havenly.DAL.Repos.Abstractions.IBedroomRepository, Havenly.DAL.Repos.Implementations.BedroomRepository>();
            builder.Services.AddScoped<Havenly.DAL.Repos.Abstractions.IBedRepository, Havenly.DAL.Repos.Implementations.BedRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<Havenly.DAL.Repos.Abstractions.IPropertyImageRepository, Havenly.DAL.Repos.Implementations.PropertyImageRepository>();
            builder.Services.AddScoped<Havenly.DAL.Repos.Abstractions.IAmenityRepository, Havenly.DAL.Repos.Implementations.AmenityRepository>();
            builder.Services.AddScoped<Havenly.DAL.Repos.Abstractions.IPropertyAmenityRepository, Havenly.DAL.Repos.Implementations.PropertyAmenityRepository>();
            builder.Services.AddScoped<IListingServices, ListingServices>();

          

            //Mapper
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<BookingMappingProfile>();
                cfg.AddProfile<PropertyMappingProfile>();
            });

          

            // Business Services
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IPropertyService, PropertyService>();

           


            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                //try
                //{
                    var context = services.GetRequiredService<HavenlyDbContext>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    await context.Database.MigrateAsync();
                    await DatabaseSeeder.SeedAsync(context, userManager, roleManager);
                //}
                //catch (Exception ex)
                //{
                //    var logger = services.GetRequiredService<ILogger<Program>>();
                //    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                //}
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Booking}/{action=MyBookings}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
