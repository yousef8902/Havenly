using Havenly.BLL.Services.Abstractions;
using Havenly.BLL.Services.Implementations;
using Havenly.DAL.Database;
using Havenly.DAL.Repos.Abstractions;
using Havenly.DAL.Repos.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Havenly.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
                builder.Services.AddDbContext<HavenlyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Repositories & Unit of Work
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Business Services
            builder.Services.AddScoped<IBookingService, BookingService>();
            var app = builder.Build();

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
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
