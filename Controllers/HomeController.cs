using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppTrade.Models;
using App.Data;
using Microsoft.AspNetCore.Identity;
using App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using App.Areas.Community.Models;

namespace AppTrade.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _context;
    private readonly  IAuthorizationService _authorizationService;


    public HomeController(AppDbContext context, 
                            ILogger<HomeController> logger, 
                            UserManager<AppUser> userManager, 
                            RoleManager<IdentityRole> roleManager,
                            IAuthorizationService authorizationService)
    {
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _authorizationService = authorizationService;
    }

    [HttpGet("/api/get-user-is-login")]
    public async Task<ActionResult> GetUserLogin_API ()
    {
        var result = await _authorizationService.AuthorizeAsync(this.User, "ConnectChat");
        if (result.Succeeded){
            return Json(new {
                login = true
            });
        }
        else{
            return Json(new {
                login = false
            });
        }
    }

    [HttpPost("/api/check-user-have-message")]
    [Authorize]
    public async Task<ActionResult> CheckUserHaveMessageNotRead ()
    {
        var user = await _userManager.GetUserAsync(User);
        if(user == null) return Json(new{
            success = false,
        });
        
        var filename = Path.Combine("Uploads", $"Chat/{user.Id}.json");
        var haveMess = false;
        List<DataChatUserModel> datas = new List<DataChatUserModel>();
        // if have exist file chat between user1 and user2
        if (System.IO.File.Exists(filename))
        {
            using (StreamReader r = new StreamReader(filename))
            {
                string json = r.ReadToEnd();
                datas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataChatUserModel>>(json);
            }
            foreach(var data in datas){
                if(data.HaveMessage == true){
                    haveMess = true;
                    break;
                }
            }
        }
        return Json(new{
            success = true,
            havemess = haveMess
        });
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
