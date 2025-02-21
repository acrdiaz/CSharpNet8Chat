// Prompt the user to select to run as Server or Client
using CSharpNet8Chat.Client;
using CSharpNet8Chat.Server;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting; // Add this using directive
using Microsoft.AspNetCore.Http; // Add this using directive


using Microsoft.AspNetCore.SignalR.Client;


Console.WriteLine("Run as server? (Yes/No)");
var response = Console.ReadLine();

// Run as Server
if (!string.IsNullOrWhiteSpace(response)
    && response.Contains("y", StringComparison.CurrentCultureIgnoreCase))
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddSignalR();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHub<ChatHub>("/chatHub");
    });

    app.Run();

    Environment.Exit(0); // Ensure the application exits cleanly
}


// Run as Client
string url = "http://localhost:5000/chatHub";

var connection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();

connection.On<string, string>("ReceiveMessage", (user, message) =>
{
    Console.WriteLine($"{user}: {message}");
});

await connection.StartAsync();
Console.WriteLine("Connected to the server.");

while (true)
{
    Console.WriteLine("Enter command (private/group/join/leave):");
    var command = Console.ReadLine();

    if (command.StartsWith("private"))
    {
        var parts = command.Split(' ', 3);
        if (parts.Length == 3)
        {
            var toUser = parts[1];
            var message = parts[2];
            await connection.InvokeAsync("SendPrivateMessage", toUser, message);
        }
        else
        {
            Console.WriteLine("Invalid command format. Use: private <username> <message>");
        }
    }
    else if (command.StartsWith("group"))
    {
        var parts = command.Split(' ', 3);
        if (parts.Length == 3)
        {
            var groupName = parts[1];
            var message = parts[2];
            await connection.InvokeAsync("SendGroupMessage", groupName, message);
        }
        else
        {
            Console.WriteLine("Invalid command format. Use: group <groupname> <message>");
        }
    }
    else if (command.StartsWith("join"))
    {
        var parts = command.Split(' ', 2);
        if (parts.Length == 2)
        {
            var groupName = parts[1];
            await connection.InvokeAsync("JoinGroup", groupName);
        }
        else
        {
            Console.WriteLine("Invalid command format. Use: join <groupname>");
        }
    }
    else if (command.StartsWith("leave"))
    {
        var parts = command.Split(' ', 2);
        if (parts.Length == 2)
        {
            var groupName = parts[1];
            await connection.InvokeAsync("LeaveGroup", groupName);
        }
        else
        {
            Console.WriteLine("Invalid command format. Use: leave <groupname>");
        }
    }
    else
    {
        Console.WriteLine("Unknown command. Use: private/group/join/leave");
    }
}
