using System.Data.Entity;
using App.Areas.Community.Models;
using App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;


namespace App.Areas.Community.Controllers
{
    [Area("Community")]
    [Authorize]
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
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
                PhoneNumber = user.PhoneNumber,
                HomeAdress = user.HomeAddress,
                BirthDate = user.BirthDate,
                AvatarUser = _context.UserAvatars.Where(a => a.UserID == user.Id).FirstOrDefault()
            };
            return View(viewUser);
        }

        [Route("viewuser/users")]
        public ActionResult ViewAllUsers()
        {
            var currUserId = _userManager.GetUserId(User);
            var users = _context.Users.Select(user => new {
                Id = user.Id,
                UserName = user.UserName,
                AvatarName = _context.UserAvatars.Where(a => a.UserID == user.Id).FirstOrDefault().AvatarName,
            }).ToList();

            var result = users.Find(u => u.Id == currUserId);
            if(result != null) users.Remove(result);
            
            ViewBag.users = users;
            return View();
        }
    }
}
