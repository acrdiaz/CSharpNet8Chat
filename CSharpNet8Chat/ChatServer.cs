using System.Net.Sockets;
using System.Net;
using System.Text;

public class ChatServer
{
    private TcpListener _server;
    private bool _isRunning;
    private List<TcpClient> _clients;

    public ChatServer(string ipAddress, int port)
    {
        _server = new TcpListener(IPAddress.Parse(ipAddress), port);
        _server.Start();

        _isRunning = true;
        _clients = new List<TcpClient>();
        Task.Run(() => AcceptClientsAsync());
    }

    private async Task AcceptClientsAsync()
    {
        while (_isRunning)
        {
            var client = await _server.AcceptTcpClientAsync();
            _clients.Add(client);
            BroadcastMessage($"Client connected: {client.Client.RemoteEndPoint}");
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
            BroadcastMessage(message, client);
        }

        _clients.Remove(client);
        client.Close();
        BroadcastMessage($"Client disconnected: {client.Client.RemoteEndPoint}");
    }

    private void BroadcastMessage(string message, TcpClient excludeClient = null)
    {
        var buffer = Encoding.UTF8.GetBytes(message);

        foreach (var client in _clients)
        {
            if (client == excludeClient) continue;

            var stream = client.GetStream();
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
