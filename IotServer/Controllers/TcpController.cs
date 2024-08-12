using IotServer.Configurations;
using IotServer.Models;
using IotServer.Models.Communication.Implementations;
using IotServer.Models.Communication.Interfaces;
using IotServer.Models.DAO.Implementations;
using IotServer.Models.DAO.Interfaces;
using IotServer.Views;
using System.Net;
using System.Net.Sockets;

namespace IotServer.Controllers;

public class TcpController
{
    private readonly TcpSettings _settings;
    private readonly IReceiveCommunication _communication;
    private readonly IDAO _dao;
    private readonly IView _view;

    public TcpController(TcpSettings settings)
    {
        _settings = settings;
        _communication = new TcpCommunication();
        _dao = DAOFactory.CreateDAO(_settings.Persistencia, _settings.FilePath, _settings.ConnectionString);
        _view = new TcpView();
    }
    public void StartListener()
    {
        TcpListener server = new TcpListener(IPAddress.Any, _settings.TcpServerPort);
        server.Start();
        _view.DisplayListen(_settings.TcpServerPort);

        while (true)
        {
            using TcpClient client = server.AcceptTcpClient();
            var data = _communication.Receive(client);
            _view.DisplayLog(data);
            SaveMessage(data);
        }
    }

    private void SaveMessage(string data)
    {
        var model = new MessageModel
        {
            Content = data,
            Timestamp = DateTime.Now
        };
        _dao.SaveMessage(model);
    }
}