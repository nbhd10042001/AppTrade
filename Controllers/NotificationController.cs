using App.Models;
using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[Authorize]
[Route("/notification/{action}")]
public class NotificationController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly NotificationService _notifService;

    public NotificationController (AppDbContext context, UserManager<AppUser> userManager, NotificationService notifService)
    {
        _context = context;
        _userManager = userManager;
        _notifService = notifService;
    }

    public ActionResult Index()
    {
        var userid = _userManager.GetUserId(User);
        var list = _notifService.GetNotificationItem(userid);
        return View(list);
    }

    public async Task<ActionResult> Detail(int id)
    {
        var notif = _context.Notification.Where(n => n.Id == id).FirstOrDefault();
        if (notif == null) return Content("Not found Id");

        if(notif.IsWatched == false)
        {
            notif.IsWatched = true;
            _context.Notification.Update(notif);
            await _context.SaveChangesAsync();
        } 

        return View(notif);
    }
}