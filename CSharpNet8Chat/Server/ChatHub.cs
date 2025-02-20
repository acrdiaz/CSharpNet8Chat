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
        // AA1 this is from copilot -- i do not trust copilot
        //await Clients.All.SendAsync(
        //    "ReceiveMessage", 
        //    "Server", 
        //    $"{Context.ConnectionId} joined the chat.");

        // AA1 to test this
        await base.OnConnectedAsync();
        await Clients.All.SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} has joined the chat.");
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        //await Clients.All.SendAsync(
        //    "ReceiveMessage", 
        //    "Server", 
        //    $"{Context.ConnectionId} left the chat.");

        // AA1 to test this
        await base.OnDisconnectedAsync(exception);
        await Clients.All.SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} has left the chat.");
    }
}
