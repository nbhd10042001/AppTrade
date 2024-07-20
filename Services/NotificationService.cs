using System.Data.Entity;
using App.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using SixLabors.ImageSharp;

namespace App.Services;

public class NotificationService
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    private const int ITEMS_PER_NOTIF = 5;

    public NotificationService (AppDbContext context, UserManager<AppUser> userManager) 
    {
        _context = context;
        _userManager = userManager;
    }

    public void CreateNotification (string typeNotif, string message, string userid, string username)
    {
        var model = new NotificationModel(){
            UserId = userid,
            UserName = username,
            DateCreated = DateTime.Now,
            IsWatched = false,
        };
        model.AddTitleNotif(typeNotif, message);

        _context.Notification.Add(model);
        _context.SaveChanges();
    }

    public List<NotificationModel> GetNotificationItem (string userid)
    {
        var list = _context.Notification.Where(n => n.UserId == userid)
                                        .OrderByDescending(item => item.DateCreated)
                                        .ToList();
        return list;
    }

    public List<NotificationModel> GetOtherNotificationNotSeen (string userid)
    {
        var list = _context.Notification.Where(n => n.UserId == userid)
                                        .OrderByDescending(item => item.DateCreated)
                                        .Skip(ITEMS_PER_NOTIF)
                                        .Where(n => n.IsWatched == false).ToList();
        return list;
    }

    public List<NotificationModel> GetAllNotificationNotSeen (string userid)
    {
        var list = _context.Notification.Where(n => n.UserId == userid)
                                        .OrderByDescending(item => item.DateCreated)
                                        .Where(n => n.IsWatched == false).ToList();
        return list;
    }

    public List<NotificationModel> GetRecentNotificationItem (string userid)
    {
        var list = _context.Notification.Where(n => n.UserId == userid)
                                        .OrderByDescending(item => item.DateCreated)
                                        .Take(ITEMS_PER_NOTIF).ToList();
        return list;
    }
}