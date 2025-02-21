using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;


namespace CSharpNet8Chat.Server;

class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync(
            "ReceiveMessage", 
            user, 
            message);
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.All.SendAsync(
            "ReceiveMessage", 
            "System", 
            $"{Context.ConnectionId} has joined the chat.");
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);
        await Clients.All.SendAsync(
            "ReceiveMessage", 
            "System", 
            $"{Context.ConnectionId} has left the chat.");
    }
}
