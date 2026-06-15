using System.Net;
using System.Net.Sockets;
using IotMonitor.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IotMonitor.Application.Services;






internal sealed class UdpPingRunner
{
    private readonly Guid _deviceId;
    private readonly string _ipAddress;
    private readonly DeviceOrchestrator _orchestrator;
    private readonly ILogger _logger;

    private static readonly TimeSpan PingInterval = TimeSpan.FromSeconds(10);

    public UdpPingRunner(
        Guid deviceId,
        string ipAddress,
        DeviceOrchestrator orchestrator,
        ILogger logger)
    {
        _deviceId = deviceId;
        _ipAddress = ipAddress;
        _orchestrator = orchestrator;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken ct)
    {
        _logger.LogInformation(
            "[UDP] Ping runner started for device {Id} ({IP})", _deviceId, _ipAddress);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var addresses = await Dns.GetHostAddressesAsync(
                    _ipAddress, AddressFamily.InterNetwork, ct);

                var isReachable = addresses.Length > 0;
                var newStatus = isReachable ? DeviceStatus.Connected : DeviceStatus.Disconnected;

                _logger.LogInformation(
                    "[UDP] Device {Id} ({IP}) → DNS {Result} ({Count} address(es))",
                    _deviceId, _ipAddress,
                    isReachable ? "resolved" : "unresolved",
                    addresses.Length);

                await _orchestrator.UpdateDeviceStatusInternalAsync(_deviceId, newStatus, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    "[UDP] Device {Id} ({IP}) → {ExType}: {Message}. Marking as Disconnected.",
                    _deviceId, _ipAddress, ex.GetType().Name, ex.Message);

                await _orchestrator.UpdateDeviceStatusInternalAsync(
                    _deviceId, DeviceStatus.Disconnected, CancellationToken.None);
            }

            try
            {
                await Task.Delay(PingInterval, ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("[UDP] Ping runner stopped for device {Id}.", _deviceId);
    }
}
