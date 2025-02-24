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

    private static readonly ConcurrentDictionary<string, byte> _groups = new ConcurrentDictionary<string, byte>();
    private static readonly ConcurrentDictionary<string, string> _clients = new ConcurrentDictionary<string, string>();

    #endregion Fields & Properties

    #region Connections

    public override async Task OnConnectedAsync()
    {
        string username = HelperGetUserID();
        await base.OnConnectedAsync();

        AddClient(username, Context.ConnectionId);

        await Clients.All.SendAsync(
            "ReceiveMessage",
            "System",
            $"{username} has joined the chat.");
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        string username = HelperGetUserID();
        await base.OnDisconnectedAsync(exception);

        RemoveClient(username);

        // AA1 is this working?
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

        if (!_groups.ContainsKey(groupName))
        {
            _groups[groupName] = 1;
        }

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

        // AA1 handle group members
        // code goes here

        await Clients.Caller.SendAsync(
            "ReceiveMessage", 
            "System", 
            $"You have left the group {groupName}.");

        await Clients.Group(groupName).SendAsync(
            "ReceiveMessage", 
            "System", 
            $"{HelperGetUserID()} has left the group {groupName}.");
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
            HelperGetUserID(), 
            message);
    }

    public async Task SendPrivateMessage(string toUser, string message)
    {
        //var connectionId = _connections.FirstOrDefault(x => x.Value == toUser).Key;
        if (!_clients.ContainsKey(toUser))
        {
            await Clients.Client(Context.User.Identity.Name).SendAsync(
                "ReceiveMessage",
                //Context.User.Identity.Name,
                Context.ConnectionId,
                $"System: toUser not found -- {toUser}");

            return;
        }

        var connectionId = toUser;
        await Clients.Client(connectionId).SendAsync(
            "ReceiveMessage",
            //Context.User.Identity.Name,
            Context.ConnectionId,
            message);

        // --
        //if (_connections.TryGetValue(toUser, out var connectionId))
        //{
        //    await Clients.Client(connectionId).SendAsync(
        //        "ReceiveMessage", 
        //        HelperGetUserID(), 
        //        message);
        //}
    }

    #endregion Messages

    #region User/Identity helper

    private string HelperGetUserID()
    {
        return Context.User.Identity.Name ?? Context.ConnectionId;
    }

    #endregion User/Identity helper

    #region Group helper

    // code addgroup, removegroup, etc
    public void AddGroup(string groupName)
    {
        if (!_groups.ContainsKey(groupName))
        {
            _groups[groupName] = 0;
        }
    }

    public void RemoveGroup(string groupName)
    {
        if (_groups.ContainsKey(groupName))
        {
            _groups.TryRemove(groupName, out _);
        }
    }

    #endregion Group helper

    #region Client helper

    public void AddClient(string username, string connectionId)
    {
        _clients[username] = connectionId;
        // AA1 wich to use Add? _clients.TryAdd(connectionId, username)
    }

    public void RemoveClient(string username)
    {
        if (_clients.ContainsKey(username))
        {
            _clients.TryRemove(username, out _);
        }
    }

    // AA1 is this necessary?
    public void AddClientToGroup(string username, string groupName)
    {
        if (_groups.ContainsKey(groupName))
        {
            _groups[groupName]++;
        }
    }

    public IEnumerable<string> GetAllClients()
    {
        //return _clients.Keys;
        //return _clients.Select(x => x.Key);
        return _clients.Values.ToList();
    }

    #endregion Client helper
}
