using AutoMapper;
using Havenly.BLL.Mappers;
using Havenly.BLL.Services.Abstractions;
using Havenly.BLL.Services.Implementations;
using Havenly.DAL.Database;
using Havenly.DAL.Database.Seed;
using Havenly.DAL.Repos.Abstractions;
using Havenly.DAL.Repos.Implementations;
using Microsoft.EntityFrameworkCore;
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


            //Mapper
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<BookingMappingProfile>();
                cfg.AddProfile<PropertyMappingProfile>();
            });

            // Repositories & Unit of Work
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();

            // Business Services
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IPropertyService, PropertyService>();


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
