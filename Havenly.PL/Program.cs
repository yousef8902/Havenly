
using Havenly.BLL.Services.Abstractions;
using Havenly.BLL.Services.Implementations;
using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Havenly.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();

            // Get and display the connection string
            Console.WriteLine(
                builder.Configuration.GetConnectionString("DefaultConnection")
            );

            // Register DbContext
            builder.Services.AddDbContext<HavenlyDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );

            // Register repository implementations
            builder.Services.AddScoped<
                Havenly.DAL.Repos.Abstractions.IPropertyAmenityRepository,
                Havenly.DAL.Repos.Implementations.PropertyAmenityRepository>();

            builder.Services.AddScoped<
                Havenly.DAL.Repos.Abstractions.IUserRepository,
                Havenly.DAL.Repos.Implementations.UserRepository>();

            builder.Services.AddScoped<
                Havenly.DAL.Repos.Abstractions.IPropertyRepository,
                Havenly.DAL.Repos.Implementations.PropertyRepository>();

            builder.Services.AddScoped<
                Havenly.DAL.Repos.Abstractions.IBookingRepository,
                Havenly.DAL.Repos.Implementations.BookingRepository>();

            builder.Services.AddScoped<
                Havenly.DAL.Repos.Abstractions.IFavoriteRepository,
                Havenly.DAL.Repos.Implementations.FavoriteRepository>();

            builder.Services.AddScoped<
                Havenly.DAL.Repos.Abstractions.IReviewRepository,
                Havenly.DAL.Repos.Implementations.ReviewRepository>();

            builder.Services.AddScoped<
                Havenly.DAL.Repos.Abstractions.IListingRepository,
                Havenly.DAL.Repos.Implementations.ListingRepository>();

            builder.Services.AddScoped<
                Havenly.DAL.Repos.Abstractions.IAddressRepository,
                Havenly.DAL.Repos.Implementations.AddressRepository>();

            builder.Services.AddScoped<
                Havenly.DAL.Repos.Abstractions.IPaymentRepository,
                Havenly.DAL.Repos.Implementations.PaymentRepository>();

            builder.Services.AddScoped<
                Havenly.DAL.Repos.Abstractions.IBedroomRepository,
                Havenly.DAL.Repos.Implementations.BedroomRepository>();

            builder.Services.AddScoped<
                Havenly.DAL.Repos.Abstractions.IBedRepository,
                Havenly.DAL.Repos.Implementations.BedRepository>();

            builder.Services.AddScoped<
                Havenly.DAL.Repos.Abstractions.IPropertyImageRepository,
                Havenly.DAL.Repos.Implementations.PropertyImageRepository>();

            builder.Services.AddScoped<
                Havenly.DAL.Repos.Abstractions.IAmenityRepository,
                Havenly.DAL.Repos.Implementations.AmenityRepository>();

            // Register services
            builder.Services.AddScoped<IListingServices, ListingServices>();
            builder.Services.AddScoped<IReportService, ReportService>();

            // Authentication
            builder.Services.AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme
            )
            .AddCookie(
                CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                    options.AccessDeniedPath = new PathString("/Account/Login");
                }
            );

            // ASP.NET Core Identity
            builder.Services.AddIdentityCore<User>(identityOptions =>
            {
                // Email confirmation is disabled for now
                // because no email service is configured.
                identityOptions.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<HavenlyDbContext>()
            .AddSignInManager()
            .AddTokenProvider<DataProtectorTokenProvider<User>>(
                TokenOptions.DefaultProvider
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Authentication must come before Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            )
            .WithStaticAssets();

            app.Run();
        }
    }
}
