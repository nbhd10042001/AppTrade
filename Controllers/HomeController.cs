using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppTrade.Models;
using App.Data;
using Microsoft.AspNetCore.Identity;
using App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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

    // [Authorize(Roles = RoleName.Member)]
    public IActionResult TestAPI ()
    {
        var userid = _userManager.GetUserId(User);
        if(userid == null) return Content("yeu cau login");
        
        var roles = _context.UserRoles.Where(r => r.UserId == userid).ToList();
        var allRoles = _context.Roles.ToList();
        var isAccess = false;

        if(roles.Count > 0)
        {
            foreach (var role in roles)
            {
                foreach(var nameR in allRoles)
                {
                    if(role.RoleId == nameR.Id)
                    {
                        if(nameR.Name == RoleName.Vip)
                            isAccess = true;
                        else if(nameR.Name == RoleName.Member)
                            isAccess = true;
                    }
                }
            }

            if(isAccess)
                return Content("Success");   
            else
                return Content("Khong duoc truy cap");         
        }
        return BadRequest();
    }

    public IActionResult Index()
    {
        var products = _context.Products.Include(p => p.Author)
                                        .Include(p => p.Photos)
                                        .Include(p => p.ProductCategoryProducts)
                                        .ThenInclude(pc => pc.CategoryProduct)
                                        .AsQueryable(); 

        products = products.OrderByDescending(p => p.DateCreated).Take(10);
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
