using System.Runtime;

namespace IotServer.Views;

public class TcpView : IView
{
    public void DisplayLog(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"TCP Message Received: {message}");
    }
    public void DisplayListen(int port)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"TCP Server listening on port {port}...");
    }
}