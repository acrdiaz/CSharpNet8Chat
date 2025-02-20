// Prompt the user to select to run as Server or Client
using CSharpNet8Chat.Client;
using CSharpNet8Chat.Server;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting; // Add this using directive
using Microsoft.AspNetCore.Http; // Add this using directive

//using Microsoft.AspNetCore.Builder;

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
//var client = new ChatClient("127.0.0.1", 5000);
//Console.WriteLine("Connected to the server.");

//while (true)
//{
//    var message = Console.ReadLine();
//    if (!string.IsNullOrWhiteSpace(message))
//    {
//        await client.SendMessageAsync(message);
//    }
//}


var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/chatHub")
            .Build();

connection.On<string, string>("ReceiveMessage", (user, message) =>
{
    Console.WriteLine($"{user}: {message}");
});

await connection.StartAsync();
Console.WriteLine("Connected to the server.");

while (true)
{
    var message = Console.ReadLine();
    await connection.InvokeAsync("SendMessage", "ConsoleClient", message);
}




/*

│36: dotnet add package Microsoft.AspNetCore.SignalR             │
│37:  dotnet add package Microsoft.Extensions.Hosting            │
│38: dotnet add package Microsoft.AspNetCore.SignalR.Client      │
│39: dotnet add package Microsoft.AspNetCore.SignalR             │
│40: dotnet add package Microsoft.Extensions.Hosting             │
│41: using Microsoft.AspNetCore.Builder;                         │
│42: dotnet --version                                            │
│43: dotnet add package Microsoft.Extensions.Configuration.Binder│
│44: dotnet add package Microsoft.Extensions.Hosting             │
│45: code .                                                      │
│46: dotnet add package Microsoft.Extensions.DependencyInjection │
│47: dotnet add package Microsoft.AspNetCore                     │
│48: dotnet add package Microsoft.AspNetCore.App

*/