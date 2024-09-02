
using System.Collections;
using System.Configuration;
using System.Data.Entity;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using App.Areas.Community.Models;
using App.Models;
using Bogus.DataSets;
using elFinder.NetCore.Models.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI;
using NuGet.Common;

namespace Hubs.SignalRChatMVC;

[Authorize]
public class ChatHub : Hub
{
    private readonly IDictionary<string, UserConnection> _connections;

    public ChatHub( IDictionary<string, UserConnection> connections)
    {
        _connections = connections;
    }

    // update user online in _connectionsService
    public async Task UserConnectionChat (string id, string name)
    {
        if(id == null || name == null) return;

        UserConnection uc = new UserConnection(){
            UserName = name,
            UserId = id
        };
        _connections[Context.ConnectionId] = uc;
    }
    
    public async Task UserHaveReadMessage (string curId, string toId)
    {
        var filename = Path.Combine("Uploads", $"Chat/{curId}.json");
        List<DataChatUserModel> datas = new List<DataChatUserModel>();
        // if have exist file chat between user1 and user2
        if (File.Exists(filename))
        {
            using (StreamReader r = new StreamReader(filename))
            {
                string json = r.ReadToEnd();
                datas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataChatUserModel>>(json);
            }
            var result = datas.FindIndex(data => data.ToId == toId);
            if (result != -1){
                datas[result].HaveMessage = false;
            }
        }
        await using FileStream createStream = File.Create(filename);
        await JsonSerializer.SerializeAsync(createStream, datas);
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        // remove user in _connectionService when current user logout or close browser
        if(_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
        {
            _connections.Remove(Context.ConnectionId);
        }
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string stringids, string message, AppDbContext _dbContext)
    {
        var strIds = stringids.Split("&&");
        var fromId = strIds[0];
        var toId = strIds[1];

        var userFrom = _dbContext.Users.Where(user => user.Id == fromId).FirstOrDefault();
        var userTo = _dbContext.Users.Where(user => user.Id == toId).FirstOrDefault();

        if(userFrom == null || userTo == null) return;

        Hashtable myHash = new Hashtable();
        var datetime = DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToShortTimeString();
        myHash.Add("DateCreate",  datetime);

        var messModel = new MessageModel(){
            Message = message,
            DateCreate = DateTime.Now.ToString(),
            DateShow = myHash["DateCreate"].ToString(),
        };

        // save file json
        await WriteAllTextToJson(fromId, toId, userFrom.UserName, userTo.UserName, messModel);
        // send message to user with id's user
        await Clients.User(toId).SendAsync("ReceiveMessage", userFrom.UserName, userFrom.Id, message, myHash);
    }

    public async Task WriteAllTextToJson(string fromid, string toid, string fromuser, string touser, MessageModel model)
    {
        var filename = Path.Combine("Uploads", $"Chat/{toid}.json");
        List<DataChatUserModel> datas = new List<DataChatUserModel>();
        // if have exist file chat between user1 and user2
        if (File.Exists(filename))
        {
            using (StreamReader r = new StreamReader(filename))
            {
                string json = r.ReadToEnd();
                datas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataChatUserModel>>(json);
            }

            var result = datas.FindIndex(data => data.ToId == fromid);

            //if there are no messages with user 2
            if (result == -1)
            {
                // create new data message with user 2
                var data = new DataChatUserModel()
                {
                    FromId = fromid,
                    ToId = toid,
                    FromUser = fromuser,
                    ToUser = touser
                };
                data.listRM.Add(model);
                data.HaveMessage = true;
                datas.Add(data);
            }
            else { 
                datas[result].listRM.Add(model);
                datas[result].HaveMessage = true;
            }
        }
        // if not exist file messages between user1 and user2
        else
        {
            var data = new DataChatUserModel()
            {
                FromId = fromid,
                ToId = toid,
                FromUser = fromuser,
                ToUser = touser
            };
            data.listRM.Add(model);
            data.HaveMessage = true;
            datas.Add(data);
        }
        await using FileStream createStream = File.Create(filename);
        await JsonSerializer.SerializeAsync(createStream, datas);
    }
}