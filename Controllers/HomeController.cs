using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppTrade.Models;
using App.Data;
using Microsoft.AspNetCore.Identity;
using App.Models;

namespace AppTrade.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        // tao ra cac roles duoc dinh nghia san trong Data\RoleNames.cs
        var roles = typeof(RoleName).GetFields().ToList();
        foreach (var role in roles)
        {
            var roleName = (string)role.GetRawConstantValue();
            var isHasRole = await _roleManager.FindByNameAsync(roleName);
            if(isHasRole == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // tao ra user Admin
        for (int i = 0; i<4;i++)
        {
            var userAdmin = await _userManager.FindByEmailAsync(@$"admin{i}@example.com");
            if (userAdmin == null)
            {
                userAdmin = new AppUser()
                {
                    UserName = @$"admin{i}",
                    Email = @$"admin{i}@example.com",
                    EmailConfirmed = true,
                };

                await _userManager.CreateAsync(userAdmin, "123123");
                await _userManager.AddToRoleAsync(userAdmin, RoleName.Administrator);
            }
        }

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
