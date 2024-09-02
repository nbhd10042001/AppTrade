namespace App.Areas.Community.Models;

public class DataChatUserModel
{
    public string FromId { set; get; }
    public string ToId { set; get; }
    public string FromUser {set; get;}
    public string ToUser {set; get;}
    public bool HaveMessage { set; get; }
    public List<MessageModel> listRM { set; get; } = new List<MessageModel>();
}