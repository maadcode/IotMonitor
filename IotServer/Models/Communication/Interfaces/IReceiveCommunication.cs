using System.Net.Sockets;

namespace IotServer.Models.Communication.Interfaces;

public interface IReceiveCommunication
{
    string Receive(TcpClient client);
}