using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCPhones.Data;
using MVCPhones.Models;

namespace MVCPhones.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PhonesContext _phonesContext;
    public  Phone? Phone { get; set; }  

    public HomeController(ILogger<HomeController> logger,
                          PhonesContext phoneConext)
    {
        _logger = logger;
        _phonesContext = phoneConext;
    }

    public async Task<IActionResult> Index()
    {
        var phones = await _phonesContext.Phones
                                   .OrderByDescending(p=>p.Modified)
                                   .Take(2)
                                   .ToListAsync();
        return View(phones);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
