using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppTrade.Models;
using App.Data;
using Microsoft.AspNetCore.Identity;
using App.Models;
using Microsoft.EntityFrameworkCore;

namespace AppTrade.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _context;


    public HomeController(AppDbContext context, 
                            ILogger<HomeController> logger, 
                            UserManager<AppUser> userManager, 
                            RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;

    }

    public IActionResult Index()
    {
        var products = _context.Products.Include(p => p.Author)
                                        .Include(p => p.Photos)
                                        .Include(p => p.ProductCategoryProducts)
                                        .ThenInclude(pc => pc.CategoryProduct)
                                        .AsQueryable(); 

        products = products.OrderByDescending(p => p.DateUpdated).Take(5);
        ViewBag.products = products;

        return View();
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
