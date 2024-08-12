using IotServer.Models.Communication.Interfaces;
using System.Net.Sockets;
using System.Text;

namespace IotServer.Models.Communication.Implementations;

public class UdpCommunication : ISendCommunication
{
    public void Send(string ipAddress, int port, string message)
    {
        using (UdpClient udpClient = new UdpClient())
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            udpClient.Send(data, data.Length, ipAddress, port);
        }
    }
}