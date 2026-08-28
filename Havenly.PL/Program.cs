
using Havenly.BLL.Services.Abstractions;
using Havenly.BLL.Services.Implementations;
using Havenly.DAL.Database;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Havenly.DAL.Database;
using Havenly.DAL.Entities;

using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Havenly.BLL.Mappers;

using Havenly.DAL.Database.Seed;
using Havenly.DAL.Repos.Abstractions;
using Havenly.DAL.Repos.Implementations;

namespace Havenly.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));
            builder.Services.AddDbContext<HavenlyDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
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

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.LoginPath = new PathString("/Account/Login");
                        options.AccessDeniedPath = new PathString("/Account/Login");
                    });
            builder.Services.AddIdentityCore<User>(identityOptions =>
                    identityOptions.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<HavenlyDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<User>>(
                    TokenOptions.DefaultProvider);
            
            var app = builder.Build();
            //using (var scope = app.Services.CreateScope())
            //{
            //    var dbContext = scope.ServiceProvider.GetRequiredService<HavenlyDbContext>();

               
            //    await dbContext.Database.EnsureCreatedAsync();

            //    await DatabaseSeeder.SeedAsync(dbContext);
            //}

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
                pattern: "{controller=Property}/{action=Details}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
