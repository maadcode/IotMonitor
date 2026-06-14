using IotMonitor.Application.Abstractions;
using IotMonitor.Data.Abstractions;
using IotMonitor.Domain.Enums;
using IotMonitor.Infrastructure.Devices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IotMonitor.Infrastructure.Services;

/// <summary>
/// Sends a fire-and-forget UDP message to a device loaded from the repository.
/// Opens a socket, sends the message, then immediately closes — no response is awaited.
/// </summary>
public sealed class UdpDeviceSender : IUdpDeviceSender
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UdpDeviceSender> _logger;

    public UdpDeviceSender(IServiceScopeFactory scopeFactory, ILogger<UdpDeviceSender> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SendToDeviceAsync(Guid deviceId, string message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();

        var entity = await repository.GetDeviceByIdAsync(deviceId, cancellationToken);

        if (entity is null)
        {
            _logger.LogWarning("[UDP] Send failed — device {Id} not found in repository.", deviceId);
            return;
        }

        _logger.LogInformation(
            "[UDP] Sending message to '{Alias}' ({Id}) at {IP}:{Port} → \"{Message}\"",
            entity.Alias, entity.Id, entity.IpAddress, entity.Port, message);

        var device = new UdpDevice(
            entity.Id,
            entity.Alias,
            entity.IpAddress,
            entity.Port,
            entity.CategoryId,
            entity.DeviceType.Lifecycle);

        await device.SendMessageAsync(message, cancellationToken);

        _logger.LogInformation(
            "[UDP] Message delivered to '{Alias}' ({Id}).", entity.Alias, entity.Id);
    }
}
