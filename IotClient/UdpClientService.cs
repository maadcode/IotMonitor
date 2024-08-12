using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace IotClient;

public class UdpClientService
{
    private readonly IPEndPoint _serverEndpoint;

    public UdpClientService(IConfiguration configuration)
    {
        var serverIp = configuration["UdpSettings:ServerIp"];
        var serverPort = int.Parse(configuration["UdpSettings:ServerPort"]);
        _serverEndpoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
    }

    public void StartSendingMessages()
    {
        Console.Title = "Test Client UDP";

        using (UdpClient client = new UdpClient())
        {
            while (true)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Write your message (or type 'exit' to quit):");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                string message = Console.ReadLine();

                if (message?.ToLower() == "exit")
                    break;

                byte[] data = Encoding.UTF8.GetBytes(message);
                client.Send(data, data.Length, _serverEndpoint);
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"Message sent to UDP server: {message}");
            }
        }
    }
}