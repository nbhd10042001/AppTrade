
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Data;
using App.Data.Interface;

namespace App.Models;

public class NotificationModel
{
    [Key]
    public int Id {set; get;}

    public string UserId {set; get;}

    public string UserName {set; get;}

    public string TypeNotif {set; get;}

    public DateTime DateCreated {set; get;}

    public bool IsWatched {set; get;}

    public string Title {set; get;}

    public string Content {set; get;}

    public void AddTitleNotif (string typeNotif, string message)
    {
        if(typeNotif == TypeNotification.Order)
        {
            this.TypeNotif = typeNotif;
            this.Title = $"You have successfully confirmed your order!";
            this.Content = $"{message}";
        }
        else if(typeNotif == TypeNotification.Account)
        {
            this.TypeNotif = typeNotif;
            this.Title = $"You have updated your account!";
            this.Content = $"{message}";
        }
        else if(typeNotif == TypeNotification.FromAdmin)
        {
            this.TypeNotif = typeNotif;
            this.Title = $"You have a notification coming from the admin!";
            this.Content = $"{message}";
        }
    }
}