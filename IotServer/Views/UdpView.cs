using System.Runtime;

namespace IotServer.Views;

public class UdpView : IView
{
    public void DisplayLog(string message)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"UDP Message Received: {message}");
    }
    public void DisplayListen(int port)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"UDP Server listening on port {port}...");
    }
}