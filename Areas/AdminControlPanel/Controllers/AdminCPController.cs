using App.Data;
using App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.AdminControlPanel.Controllers;

[Area("AdminControlPanel")]
[Authorize(Roles = RoleName.Administrator)]
public class AdminCPController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    public AdminCPController (UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }
    [Route("/admincp")]
    public IActionResult Index () => View();

}