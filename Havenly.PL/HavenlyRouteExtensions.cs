namespace Havenly.PL;

public static class HavenlyRouteExtensions
{
    /// <summary>
    /// Call from Program.cs after MapStaticAssets, before or instead of the default MapControllerRoute.
    /// Specific routes must be registered first.
    /// </summary>
    public static void MapHavenlyRoutes(this IEndpointRouteBuilder app)
    {
        app.MapControllerRoute(
            name: "property-details",
            pattern: "property/{id}",
            defaults: new { controller = "Property", action = "Details" });

        app.MapControllerRoute(
            name: "booking-confirmed",
            pattern: "booking-confirmed",
            defaults: new { controller = "Bookings", action = "Confirmed" });

        app.MapControllerRoute(
            name: "host-new-property",
            pattern: "host/properties/new",
            defaults: new { controller = "Host", action = "NewProperty" });

        app.MapControllerRoute(
            name: "host-bookings",
            pattern: "host/bookings",
            defaults: new { controller = "Host", action = "Bookings" });

        app.MapControllerRoute(
            name: "host",
            pattern: "host/{action=Index}",
            defaults: new { controller = "Host" });

        app.MapControllerRoute(
            name: "admin-activity",
            pattern: "admin/activity",
            defaults: new { controller = "Admin", action = "Activity" });

        app.MapControllerRoute(
            name: "admin",
            pattern: "admin/{action=Index}",
            defaults: new { controller = "Admin" });

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    }
}
