using System.Collections;
using System.Data.Entity;
using System.Text.Json;
using App.Areas.Community.Models;
using App.Models;
using Azure.Identity;
using Hubs.SignalRChatMVC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Community.Controllers;

[Area("Community")]
[Authorize]
public class ChatUserController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IDictionary<string, UserConnection> _connections;

    public ChatUserController ( AppDbContext context, 
                                UserManager<AppUser> userManager,
                                IDictionary<string, UserConnection> connections
                                )
    {
        _context = context;
        _userManager = userManager;
        _connections = connections;
    }

    [Route("chat")]
    public async Task<IActionResult> Index ([Bind("UserId")] ViewUserModel? model)
    {
        var curr_user = await _userManager.GetUserAsync(User);
        var toId = "";

        if(model.UserId == null) {
            toId = _context.Users.Where(u => u.Id != curr_user.Id).FirstOrDefault().Id;
        }
        else{
            toId = model.UserId; 
        }

        if(curr_user.Id == toId) return Content("User is same");

        var userReceiveName = _context.Users.Where(user => user.Id == toId).FirstOrDefault().UserName.ToString();
        
        ViewBag.FromId = curr_user.Id;
        ViewBag.ToId = toId;
        ViewBag.CurrUserName = curr_user.UserName;
        ViewBag.UserReceiveName = userReceiveName;

        return View();
    }

    [HttpGet("/api/get-current-user-login")]
    public async Task<ActionResult> GetCurrentUserLogin_API()
    {

        var user = await _userManager.GetUserAsync(User);
        if(user == null){
            return Json(new{
                success = false
            });
        }
        return Json(new {
            success = true,
            name = user.UserName,
            id = user.Id
        });
    }

    [HttpPost("/api/get-all-users")]
    public async Task<IActionResult> GetListAllUsers_API()
    {
        var currUserId = _userManager.GetUserId(User);
        var list_users = _context.Users.Where(u => u.Id != currUserId).ToList(); 

        var filename = Path.Combine("Uploads", $"Chat/{currUserId}.json");
        List<DataChatUserModel> datas = new List<DataChatUserModel>();
        List<string> listUserHaveMessage = new List<string>();
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
                    listUserHaveMessage.Add(data.ToUser);
                }
            }
        }

        return Json(new {
            users = list_users,
            userHaveMessages = listUserHaveMessage
        });
    }

    [HttpPost("/api/get-user-connect-chat")]
    public ActionResult GetUserConnectionChat_API()
    {  
        var curUserId = _userManager.GetUserId(User);
        var userOffs = _context.Users.Where(u => u.Id != curUserId).Select(u => new UserConnection{
            UserName = u.UserName,
            UserId = u.Id,
        }).ToList();

        List<UserConnection> userConnects = new List<UserConnection>();
        foreach(var value in _connections.Values)
        {
            if(value.UserId == curUserId) continue;
            userConnects.Add(new UserConnection(){
                UserName = value.UserName,
                UserId = value.UserId,
            });
        }

        foreach(var userOnl in userConnects)
        {
            var user = userOffs.Find(u => u.UserId == userOnl.UserId);
            if(user != null){
                userOffs.Remove(user);
            }
        }

        return Json(new {
            listOnl = userConnects,
            listOff = userOffs
        });
    }

    [HttpPost("/api/get-message")]
    public async Task<IActionResult> GetMessageUser_API(string idFrom, string idTo)
    {
        DataChatUserModel messages_userFrom = null;
        DataChatUserModel messages_userTo = null;

        // get list message from user idFrom to user idTo
        var filename = Path.Combine("Uploads", $"Chat/{idFrom}.json");
        if(System.IO.File.Exists(filename))
        {
            List<DataChatUserModel> datas = new List<DataChatUserModel>();
            using (StreamReader r = new StreamReader(filename))
            {
                string json = r.ReadToEnd();
                datas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataChatUserModel>>(json);
            }
            var index = datas.FindIndex(data => data.ToId == idTo);
            if(index != -1){ // if index == -1, not found id
                messages_userFrom = datas[index];
                datas[index].HaveMessage = false;
            }
            await using FileStream createStream = System.IO.File.Create(filename);
            await JsonSerializer.SerializeAsync(createStream, datas);
        }

        // get list message from user idTo to user idFrom
        filename = Path.Combine("Uploads", $"Chat/{idTo}.json");
        if(System.IO.File.Exists(filename))
        {
            List<DataChatUserModel> datas = new List<DataChatUserModel>();
            using (StreamReader r = new StreamReader(filename))
            {
                string json = r.ReadToEnd();
                datas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataChatUserModel>>(json);
            }
            var index = datas.FindIndex(data => data.ToId == idFrom);
            if(index != -1){ // if index == -1, not found id
                messages_userTo= datas[index];
            }
        }

        // check if mess_uFrom not null and mess_uTo not null, then move items mess_uTo to mess_uFrom.
        // If mess_uFrom is null and mess_uTo not null, then mess_uFrom = mess_uTo.
        // Else all null, return json success = false.
        List<MessageViewModel> listMessUserFrom = new List<MessageViewModel>();
        List<MessageViewModel> listMessUserTo= new List<MessageViewModel>();
        if(messages_userFrom != null){
            listMessUserFrom = messages_userFrom.listRM.Select(mess => new MessageViewModel(){
                UserSend = messages_userFrom.ToUser,
                Message = mess.Message,
                DateCreate = mess.DateCreate,
                DateShow = mess.DateShow,
            }).ToList();
        }

        if(messages_userTo != null){
            listMessUserTo = messages_userTo.listRM.Select(mess => new MessageViewModel(){
                UserSend = messages_userTo.ToUser,
                Message = mess.Message,
                DateCreate = mess.DateCreate,
                DateShow = mess.DateShow,
            }).ToList();
        }

        // gop tin nhan tu user 2 vao user 1
        if(listMessUserFrom != null && listMessUserTo != null){
            foreach(var mess in listMessUserTo){
                listMessUserFrom.Add(mess);
            }
        }
        else if (listMessUserFrom == null && listMessUserTo != null){
            listMessUserFrom = listMessUserTo;
        }

        // return json if mess not null
        if(listMessUserFrom != null){
            var listmess = listMessUserFrom.OrderBy(m => m.DateCreate).ToList();

            return Json(new {
                success = true,
                messages = listmess,
            });
        }
        else{
            return Json(new {
                success = false,
            });
        }
    }
}
