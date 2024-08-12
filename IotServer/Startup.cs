using IotServer.Configurations;
using IotServer.Controllers;
using Microsoft.Extensions.Configuration;

namespace IotServer;

public class Startup
{
    public async Task Init()
    {
        var configuration = LoadConfiguration();
        var tcpController = CreateTcpController(configuration);
        var udpController = CreateUdpController(configuration);

        var tcpTask = Task.Run(() => tcpController.StartListener());
        var udpTask = Task.Run(() => udpController.StartListener());

        await Task.WhenAll(tcpTask, udpTask);
    }

    private IConfiguration LoadConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    private TcpController CreateTcpController(IConfiguration configuration) => new TcpController(configuration.GetSection("TcpSettings").Get<TcpSettings>());
    private UdpController CreateUdpController(IConfiguration configuration) => new UdpController(configuration.GetSection("UdpSettings").Get<UdpSettings>());
}