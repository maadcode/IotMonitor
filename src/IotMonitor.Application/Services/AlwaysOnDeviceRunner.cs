using System.Net;
using System.Net.Sockets;
using IotMonitor.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IotMonitor.Application.Services;

/// <summary>
/// Maintains a persistent TCP connection for a single AlwaysOn device,
/// reads incoming bytes and maps them to <see cref="DeviceStatus"/>.
/// </summary>
internal sealed class AlwaysOnDeviceRunner
{
    private readonly Guid _deviceId;
    private readonly string _ipAddress;
    private readonly int _port;
    private readonly DeviceOrchestrator _orchestrator;
    private readonly ILogger _logger;

    private static readonly TimeSpan ReconnectDelay = TimeSpan.FromSeconds(5);

    public AlwaysOnDeviceRunner(
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

    /// <summary>
    /// Connects, reads bytes in a loop, and reconnects automatically on failure.
    /// Runs until <paramref name="ct"/> is cancelled.
    /// <para>
    /// Byte protocol (matches Node-RED broadcast-func):
    /// <c>0x01</c> = sensor ON  → <see cref="DeviceStatus.Connected"/><br/>
    /// <c>0x00</c> = sensor OFF → <see cref="DeviceStatus.Disconnected"/>
    /// </para>
    /// </summary>
    public async Task RunAsync(CancellationToken ct)
    {
        _logger.LogInformation(
            "[TCP] Runner started for device {Id} ({IP}:{Port})", _deviceId, _ipAddress, _port);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                // Resolve the hostname to an IPv4 address explicitly.
                // Using the default TcpClient() (dual-stack) on Linux throws
                // PlatformNotSupportedException after any failed connect attempt.
                var addresses = await Dns.GetHostAddressesAsync(
                    _ipAddress, AddressFamily.InterNetwork, ct);

                if (addresses.Length == 0)
                {
                    _logger.LogWarning(
                        "[TCP] Device {Id} → DNS lookup for '{Host}' returned no IPv4 addresses. Retrying in {Delay}s…",
                        _deviceId, _ipAddress, ReconnectDelay.TotalSeconds);
                    await Task.Delay(ReconnectDelay, ct);
                    continue;
                }

                var resolvedIp = addresses[0];
                _logger.LogInformation(
                    "[TCP] Device {Id} → resolved '{Host}' to {IP}", _deviceId, _ipAddress, resolvedIp);

                // Force IPv4 to avoid dual-stack issues on Linux
                using var client = new TcpClient(AddressFamily.InterNetwork);

                // Enable OS-level keep-alive to detect silent network drops
                client.Client.SetSocketOption(
                    SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

                _logger.LogInformation(
                    "[TCP] Device {Id} → attempting ConnectAsync to {IP}:{Port}", _deviceId, resolvedIp, _port);

                await client.ConnectAsync(resolvedIp, _port, ct);

                _logger.LogInformation(
                    "[TCP] Device {Id} → CONNECTED to {IP}:{Port} (LocalEndPoint: {Local})",
                    _deviceId, _ipAddress, _port, client.Client.LocalEndPoint);

                // TCP handshake done — sensor state is unknown until Node-RED broadcasts.
                await _orchestrator.UpdateDeviceStatusInternalAsync(
                    _deviceId, DeviceStatus.Ready, ct);

                using var stream = client.GetStream();

                // Node-RED TCP In only captures _session when the CLIENT sends data.
                // Send a single registration byte (0x00) so Node-RED stores our session
                // in the tcp_clients pool and future broadcasts reach us.
                await stream.WriteAsync(new byte[] { 0x00 }, ct);
                _logger.LogInformation(
                    "[TCP] Device {Id} → sent registration byte — waiting for sensor state broadcasts…", _deviceId);

                var buffer = new byte[64];
                var totalBytesReceived = 0;

                while (!ct.IsCancellationRequested)
                {
                    _logger.LogDebug(
                        "[TCP] Device {Id} → waiting for data…", _deviceId);

                    var bytesRead = await stream.ReadAsync(buffer, ct);

                    if (bytesRead == 0)
                    {
                        _logger.LogInformation(
                            "[TCP] Device {Id} → server closed connection cleanly (total bytes received: {Total})",
                            _deviceId, totalBytesReceived);
                        break;
                    }

                    totalBytesReceived += bytesRead;

                    var rawHex = Convert.ToHexString(buffer, 0, bytesRead);
                    var lastByte = buffer[bytesRead - 1];
                    var newStatus = lastByte == 0x01
                        ? DeviceStatus.Connected
                        : DeviceStatus.Disconnected;

                    _logger.LogInformation(
                        "[TCP] Device {Id} → received {Count} byte(s) [{Hex}], last=0x{Last:X2} → {Status}",
                        _deviceId, bytesRead, rawHex, lastByte, newStatus);

                    await _orchestrator.UpdateDeviceStatusInternalAsync(_deviceId, newStatus, ct);
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                break;
            }
            catch (SocketException sex)
            {
                _logger.LogWarning(
                    "[TCP] Device {Id} → SocketException {Code} ({Error}) connecting to {IP}:{Port}. Retrying in {Delay}s…",
                    _deviceId, sex.SocketErrorCode, sex.Message, _ipAddress, _port, ReconnectDelay.TotalSeconds);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    "[TCP] Device {Id} → {ExType}: {Message}. Retrying in {Delay}s…",
                    _deviceId, ex.GetType().Name, ex.Message, ReconnectDelay.TotalSeconds);
            }

            // Mark as unknown while waiting to reconnect
            await _orchestrator.UpdateDeviceStatusInternalAsync(
                _deviceId, DeviceStatus.Unknown, CancellationToken.None);

            if (!ct.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(ReconnectDelay, ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        _logger.LogInformation("Device {Id} runner stopped.", _deviceId);
    }
}
