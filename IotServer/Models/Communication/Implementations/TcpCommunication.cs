using IotServer.Models.Communication.Interfaces;
using System.Net.Sockets;
using System.Text;

namespace IotServer.Models.Communication.Implementations;

public class TcpCommunication : IReceiveCommunication
{
    public string Receive(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }
}