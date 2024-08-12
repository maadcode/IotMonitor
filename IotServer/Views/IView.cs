namespace IotServer.Views;

public interface IView
{
    void DisplayLog(string message);
    void DisplayListen(int port);
}