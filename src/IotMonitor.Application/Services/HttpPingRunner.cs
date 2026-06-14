using IotMonitor.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IotMonitor.Application.Services;

/// <summary>
/// Periodically checks reachability of an HTTP device by sending a HEAD request.
/// Updates the orchestrator status so the dashboard reflects the current camera state.
/// </summary>
internal sealed class HttpPingRunner
{
    private readonly Guid _deviceId;
    private readonly string _ipAddress;
    private readonly int _port;
    private readonly DeviceOrchestrator _orchestrator;
    private readonly ILogger _logger;

    private static readonly TimeSpan PingInterval = TimeSpan.FromSeconds(15);
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(5) };

    public HttpPingRunner(
        Guid deviceId,
        string ipAddress,
        int port,
        DeviceOrchestrator orchestrator,
        ILogger logger)
    {
        _deviceId = deviceId;
        _ipAddress = ipAddress;
        _port = port;
        _orchestrator = orchestrator;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken ct)
    {
        _logger.LogInformation(
            "[HTTP] Ping runner started for device {Id} ({IP}:{Port})", _deviceId, _ipAddress, _port);

        while (!ct.IsCancellationRequested)
        {
            var newStatus = DeviceStatus.Disconnected;
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                cts.CancelAfter(TimeSpan.FromSeconds(5));

                var request = new HttpRequestMessage(HttpMethod.Head, $"http://{_ipAddress}:{_port}");
                var response = await _httpClient.SendAsync(request, cts.Token);
                newStatus = response.IsSuccessStatusCode ? DeviceStatus.Ready : DeviceStatus.Disconnected;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(
                    "[HTTP] Ping failed for device {Id}: {Message}", _deviceId, ex.Message);
            }

            _logger.LogInformation(
                "[HTTP] Device {Id} ({IP}:{Port}) → {Status}",
                _deviceId, _ipAddress, _port, newStatus);

            await _orchestrator.UpdateDeviceStatusInternalAsync(_deviceId, newStatus, ct);

            await Task.Delay(PingInterval, ct).ConfigureAwait(false);
        }

        _logger.LogInformation("[HTTP] Ping runner stopped for device {Id}.", _deviceId);
    }
}

