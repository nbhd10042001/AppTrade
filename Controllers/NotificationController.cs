using System.Data.Entity;
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

    public async Task<ActionResult> Index()
    {
        var userid = _userManager.GetUserId(User);
        var list = _notifService.GetNotificationItem(userid);
        var user = await _userManager.GetUserAsync(this.User);
        ViewBag.userid = user.Id;

        return View(list);
    }

    [HttpPost("/api/get-noti")]
    public ActionResult GetNotificationItem_API (string message)
    {
        if(string.IsNullOrEmpty(message)) return BadRequest();

        string[] words = message.Split('+');
        var userid = "";
        var _currCall = "";

        for (int i = 0; i < words.Length; i++)
        {
            if (i == 0) userid = words[i];
            else if (i == 1) _currCall = words[i];
        }
        int currCall = Int32.Parse(_currCall);

        var list = _context.Notification.Where(n => n.UserId == userid)
                                        .OrderByDescending(item => item.DateCreated).Skip(5*currCall)
                                        .Take(5)
                                        .ToList();

        var listNoti = list.Select(noti => new {
                iswatch = noti.IsWatched,
                id = noti.Id,
                title = noti.Title,
                content = noti.Content,
                date = noti.DateCreated
            });

        return Json(new {
            success = 1,
            list = listNoti,
        });
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

    [HttpPost]
    public async Task<ActionResult> DeleteAPI(int id)
    {
        //  return Json(new {
        //     success = 1,
        // });
        
        var noti = _context.Notification.Where(n => n.Id == id).FirstOrDefault();
        if(noti == null) return BadRequest();

        _context.Notification.Remove(noti);
        await _context.SaveChangesAsync();

        return Json(new {
            success = 1,
        });
    }
}