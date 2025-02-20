using System.Net.Sockets;
using System.Text;

namespace CSharpNet8Chat.Client;

public class ChatClient
{
    private TcpClient _client;
    private NetworkStream _stream;

    public ChatClient(string ipAddress, int port)
    {
        _client = new TcpClient(ipAddress, port);
        _stream = _client.GetStream();

        Task.Run(() => ListenForMessagesAsync());
    }

    private async Task ListenForMessagesAsync()
    {
        var buffer = new byte[1024];

        while (_client.Connected)
        {
            var byteCount = await _stream.ReadAsync(buffer, 0, buffer.Length);
            if (byteCount <= 0) continue;

            var message = Encoding.UTF8.GetString(buffer, 0, byteCount);
            Console.WriteLine(message);
        }
    }

    public async Task SendMessageAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        await _stream.WriteAsync(buffer, 0, buffer.Length);
    }
}