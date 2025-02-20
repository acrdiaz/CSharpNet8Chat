using System.Net.Sockets;
using System.Net;
using System.Text;

namespace CSharpNet8Chat.Server;

public class ChatServer
{
    private TcpListener _server;
    private bool _isRunning;

    public ChatServer(string ipAddress, int port)
    {
        _server = new TcpListener(IPAddress.Parse(ipAddress), port);
        _server.Start();

        _isRunning = true;
        Task.Run(() => AcceptClientsAsync());
    }

    private async Task AcceptClientsAsync()
    {
        while (_isRunning)
        {
            var client = await _server.AcceptTcpClientAsync();
            await HandleClientAsync(client);
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        var buffer = new byte[1024];
        var stream = client.GetStream();

        while (client.Connected)
        {
            var byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (byteCount <= 0) continue;

            var message = Encoding.UTF8.GetString(buffer, 0, byteCount);
            Console.WriteLine($"Received: {message}");

            var response = Encoding.UTF8.GetBytes($"[{DateTime.UtcNow}] Received: {message}");
            await stream.WriteAsync(response, 0, response.Length);
        }

        client.Close();
    }
}
