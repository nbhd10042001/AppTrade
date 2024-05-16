using App.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.AdminControlPanel.Controllers;

[Area("AdminControlPanel")]
[Authorize(Roles = RoleName.Administrator)]
public class AdminCPController : Controller
{
    [Route("/admincp")]
    public IActionResult Index () => View();
}