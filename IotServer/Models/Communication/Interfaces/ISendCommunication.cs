namespace IotServer.Models.Communication.Interfaces;

public interface ISendCommunication
{
    void Send(string ipAddress, int port, string message);
}