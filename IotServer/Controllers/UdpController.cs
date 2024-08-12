using IotServer.Configurations;
using IotServer.Models;
using IotServer.Models.Communication.Implementations;
using IotServer.Models.Communication.Interfaces;
using IotServer.Models.DAO.Implementations;
using IotServer.Models.DAO.Interfaces;
using IotServer.Views;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace IotServer.Controllers;

public class UdpController
{
    private readonly UdpSettings _settings;
    private readonly ISendCommunication _communication;
    private readonly IDAO _dao;
    private readonly IView _view;

    public UdpController(UdpSettings settings)
    {
        _settings = settings;
        _communication = new UdpCommunication();
        _dao = DAOFactory.CreateDAO(_settings.Persistencia, _settings.FilePath, _settings.ConnectionString);
        _view = new UdpView();
    }

    public void StartListener()
    {
        IPEndPoint _endPoint = new IPEndPoint(IPAddress.Any, _settings.UdpServerPort);
        using UdpClient _udpServer = new UdpClient(_endPoint);
        _view.DisplayListen(_settings.UdpServerPort);

        while (true)
        {
            var receivedResults = _udpServer.Receive(ref _endPoint);
            string data = Encoding.UTF8.GetString(receivedResults);
            _view.DisplayLog(data);
            SaveMessage(data);
            _communication.Send(_settings.NodeRedServerIp, _settings.NodeRedServerPort, data);
        }
    }

    private void SaveMessage(string data)
    {
        var model = new MessageModel
        {
            Content = data,
            Timestamp = DateTime.Now,
            Protocol = Protocol.UDP
        };
        _dao.SaveMessage(model);
    }
}