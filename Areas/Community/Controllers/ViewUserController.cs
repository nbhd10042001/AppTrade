using App.Areas.Community.Models;
using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Community.Controllers
{
    [Area("Community")]
    public class ViewUserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ViewUserController (AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("viewuser/profile/{name}")]
        public async Task<ActionResult> Index(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            if(user == null)
                return Content("Not found user!");

            ViewUserModel viewUser = new ViewUserModel()
            {
                UserName = user.UserName,
                UserEmail = user.Email,
                PhoneNumber = user.PhoneNumber,
                HomeAdress = user.HomeAddress,
                BirthDate = user.BirthDate,
                AvatarUser = _context.UserAvatars.Where(a => a.UserID == user.Id).FirstOrDefault()
            };
            return View(viewUser);
        }

    }
}
