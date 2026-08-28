using Havenly.BLL.ModelVMs;
using Havenly.PL.Data;

using Microsoft.AspNetCore.Mvc;

namespace Havenly.PL.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Havenly — Find a place that feels like home";
        ViewData["Description"] = "Havenly is a short-term rental marketplace for considered homes.";
        ViewData["TransparentHeader"] = true;

        var approved = DemoCatalog.Properties.Where(p => p.Status == "approved").ToList();
        var vm = new HomeIndexVM
        {
            Search = new SearchFilterVM { Guests = 2 },
            Destinations = DemoCatalog.Destinations,
            Featured = approved.Take(4).Select((p, i) => DemoCatalog.ToCard(p, i == 0 ? "Guest Favourite" : i == 3 ? "New" : null)).ToList(),
            Recommended = approved.Skip(4).Take(4).Select((p, i) => DemoCatalog.ToCard(p, i == 1 ? "Top Rated" : null)).ToList()
        };
        return View(vm);
    }

    [HttpGet]
    public IActionResult Error()
    {
        ViewData["Title"] = "This page didn't load";
        return View();
    }

    [HttpGet]
    public IActionResult NotFoundPage()
    {
        Response.StatusCode = 404;
        ViewData["Title"] = "Page not found";
        return View("NotFound");
    }
}

