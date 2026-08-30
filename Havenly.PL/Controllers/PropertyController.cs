<<<<<<< HEAD

using Havenly.BLL.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
=======
using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Database;
using Havenly.DAL.Entities;
using Havenly.PL.Data;

using Microsoft.AspNetCore.Mvc;

>>>>>>> 63b1b37 (add search by review and change relation between (review->booking) to (review->user))
namespace Havenly.PL.Controllers;

public class PropertyController : Controller
{
    private readonly IPropertyService _propertyService;

    public PropertyController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? city, string? category, int guests = 2)
    {
        var properties = await _propertyService.GetPropertiesAsync(city, category, guests);
        return View(properties);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(long id)
    {
        if (id == 0)
        {
            return NotFound();
        }

        var propertyVm = await _propertyService.GetPropertyDetailsByIdAsync(id);

        if (propertyVm == null)
        {
            return NotFound();
        }


      

        return View(propertyVm);
    }
}


