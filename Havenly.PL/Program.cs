

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

            // Add services to the container
            builder.Services.AddControllersWithViews();


           
            // Register DbContext
            builder.Services.AddDbContext<HavenlyDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );



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
            builder.Services.AddScoped<IUserRepository,UserRepository>();
            builder.Services.AddScoped<IPropertyRepository,PropertyRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IFavoriteRepository,FavoriteRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IListingRepository,ListingRepository>();
            builder.Services.AddScoped<IAddressRepository,AddressRepository>();
            builder.Services.AddScoped<IPaymentRepository,PaymentRepository>();
            builder.Services.AddScoped<IBedroomRepository,BedroomRepository>();
            builder.Services.AddScoped<IBedRepository, BedRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
            builder.Services.AddScoped<IAmenityRepository, AmenityRepository>();
            builder.Services.AddScoped<IPropertyAmenityRepository, PropertyAmenityRepository>();

           


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

          

            //Mapper
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<BookingMappingProfile>();
                cfg.AddProfile<PropertyMappingProfile>();
            });

          

            // Business Services
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IPropertyService, PropertyService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IListingServices, ListingServices>();    
            builder.Services.AddScoped<IReviewServices, ReviewServices>();





            var app = builder.Build();
            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    //try
            //    //{
            //        var context = services.GetRequiredService<HavenlyDbContext>();
            //        var userManager = services.GetRequiredService<UserManager<User>>();
            //        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            //        await context.Database.MigrateAsync();
            //        await DatabaseSeeder.SeedAsync(context, userManager, roleManager);
            //    //}
            //    //catch (Exception ex)
            //    //{
            //    //    var logger = services.GetRequiredService<ILogger<Program>>();
            //    //    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
            //    //}
            //}

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

                pattern: "{controller=Admin}/{action=Dashboard}/{id?}"
            );



            app.Run();
        }
    }
}
