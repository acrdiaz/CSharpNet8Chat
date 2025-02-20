// Prompt the user to select to run as Server or Client
Console.WriteLine("Run as server? (Yes/No)");
var response = Console.ReadLine();


// Run as Server
if (!string.IsNullOrWhiteSpace(response)
    && response.Contains("y", StringComparison.CurrentCultureIgnoreCase))
{
    var server = new ChatServer("127.0.0.1", 5000);
    Console.WriteLine("Server is running...");
    Console.ReadLine();

    Environment.Exit(0); // Ensure the application exits cleanly
}

// Run as Client
var client = new ChatClient("127.0.0.1", 5000);
Console.WriteLine("Connected to the server.");

while (true)
{
    var message = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(message))
    {
        await client.SendMessageAsync(message);
    }
}
