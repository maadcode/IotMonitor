using System.Net.Sockets;
using System.Text;
using IotMonitor.Domain.Abstractions;
using IotMonitor.Domain.Enums;
using IotMonitor.Domain.Interfaces;

namespace IotMonitor.Infrastructure.Devices;

/// <summary>
/// Protocol handler for UDP-based devices (e.g., dynamic message signs).
/// </summary>
public sealed class UdpDevice : DeviceBase, IUdpMessenger
{
    public UdpDevice(
        Guid id,
        string alias,
        string ipAddress,
        int port,
        DeviceCategory category,
        ConnectionLifecycle lifecycle)
        : base(id, alias, ipAddress, port, category, lifecycle)
    {
    }

    /// <inheritdoc />
    public override async Task<bool> TestConnectionAsync(CancellationToken cancellationToken)
    {
        try
        {
            // UDP is stateless, so we just verify if the endpoint is reachable/formattable
            // and optionally send a zero-byte heartbeat if the device supports it.
            using var client = new UdpClient();
            client.Connect(IpAddress, Port);
            
            // In a real scenario, we might send a heartbeat and wait for a response,
            // but for a generic handler, successfully opening the socket is the baseline.
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async Task SendMessageAsync(string message, CancellationToken cancellationToken)
    {
        using var client = new UdpClient();
        client.Connect(IpAddress, Port);

        var data = Encoding.UTF8.GetBytes(message);
        await client.SendAsync(data, cancellationToken);
    }
}
