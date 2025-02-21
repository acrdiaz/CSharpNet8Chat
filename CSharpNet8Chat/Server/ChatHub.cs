using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;


namespace CSharpNet8Chat.Server;

class ChatHub : Hub
{
    #region Fields & Properties

    private static Dictionary<string, string> _connections = new Dictionary<string, string>();
    //private static ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

    #endregion Fields & Properties

    #region Connections

    public override async Task OnConnectedAsync()
    {
        _connections[Context.ConnectionId] = Context.User.Identity.Name;
        await base.OnConnectedAsync();

        await Clients.All.SendAsync(
            "ReceiveMessage", 
            "System", 
            $"{Context.ConnectionId} has joined the chat.");
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        _connections.Remove(Context.ConnectionId); // AA1 delete this code
        await base.OnDisconnectedAsync(exception);

        await Clients.All.SendAsync(
            "ReceiveMessage", 
            "System", 
            $"{Context.ConnectionId} has left the chat.");
    }

    #endregion Connections

    #region Groups

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        await Clients.Caller.SendAsync(
            "ReceiveMessage", 
            "System", 
            $"You have joined the group {groupName}.");

        await Clients.Group(groupName).SendAsync(
            "ReceiveMessage",
            "System",
            $"{Context.User.Identity.Name} has joined the group {groupName}.");
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        await Clients.Caller.SendAsync(
            "ReceiveMessage", 
            "System", 
            $"You have left the group {groupName}.");

        await Clients.Group(groupName).SendAsync(
            "ReceiveMessage", 
            "System", 
            $"{Context.User.Identity.Name ?? Context.ConnectionId} has left the group {groupName}.");
    }

    #endregion Groups
    
    #region Messages
    
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync(
            "ReceiveMessage",
            user,
            message);
    }

    public async Task SendGroupMessage(string groupName, string message)
    {
        await Clients.Group(groupName).SendAsync(
            "ReceiveMessage",
            Context.User.Identity.Name ?? Context.ConnectionId, 
            message);
    }

    public async Task SendPrivateMessage(string toUser, string message)
    {
        var connectionId = _connections.FirstOrDefault(x => x.Value == toUser).Key;
        if (connectionId != null)
        {
            await Clients.Client(connectionId).SendAsync(
                "ReceiveMessage", 
                Context.User.Identity.Name, 
                message);
        }
    }

    #endregion Messages
}
