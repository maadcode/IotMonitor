using System.Net;
using System.Net.Sockets;
using System.Text;
using IotMonitor.Domain.Abstractions;
using IotMonitor.Domain.Enums;
using IotMonitor.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace IotMonitor.Infrastructure.Devices;




public sealed class UdpDevice : DeviceBase, IUdpMessenger
{
    private readonly ILogger<UdpDevice> _logger;

    public UdpDevice(
        Guid id,
        string alias,
        string ipAddress,
        int port,
        DeviceCategory category,
        ConnectionLifecycle lifecycle,
        ILogger<UdpDevice>? logger = null)
        : base(id, alias, ipAddress, port, category, lifecycle)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<UdpDevice>.Instance;
    }

    
    
    
    
    
    public override async Task<bool> TestConnectionAsync(CancellationToken cancellationToken)
    {
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(
                IpAddress, AddressFamily.InterNetwork, cancellationToken);

            var reachable = addresses.Length > 0;

            _logger.LogInformation(
                "[UDP] TestConnection {Alias} ({IP}) → DNS {Result} ({Count} address(es))",
                Alias, IpAddress,
                reachable ? "resolved" : "unresolved",
                addresses.Length);

            return reachable;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                "[UDP] TestConnection {Alias} ({IP}) → {ExType}: {Message}",
                Alias, IpAddress, ex.GetType().Name, ex.Message);
            return false;
        }
    }

    
    
    
    
    
    public async Task SendMessageAsync(string message, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[UDP] Sending datagram to {Alias} ({IP}:{Port}) → \"{Message}\"",
            Alias, IpAddress, Port, message);

        using var client = new UdpClient();
        client.Connect(IpAddress, Port);

        var data = Encoding.UTF8.GetBytes(message);
        await client.SendAsync(data, cancellationToken);

        _logger.LogInformation(
            "[UDP] Datagram sent to {Alias} ({IP}:{Port}), {Bytes} byte(s).",
            Alias, IpAddress, Port, data.Length);
    }
}
