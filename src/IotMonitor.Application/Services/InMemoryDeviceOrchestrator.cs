using IotMonitor.Application.Abstractions;
using IotMonitor.Domain.Enums;
using IotMonitor.Domain.Models;

namespace IotMonitor.Application.Services;

/// <summary>
/// Temporary in-memory orchestrator used while protocol handlers are being implemented.
/// </summary>
public sealed class InMemoryDeviceOrchestrator : IDeviceOrchestrator
{
    private readonly List<DeviceSnapshot> _devices =
    [
        new(Guid.NewGuid(), "Sensor Patio Norte", DeviceCategory.Lectura, ConnectionLifecycle.AlwaysOn, DeviceStatus.Unknown, null),
        new(Guid.NewGuid(), "Camara Acceso 1", DeviceCategory.Seguridad, ConnectionLifecycle.OnDemand, DeviceStatus.Ready, DateTimeOffset.UtcNow.AddMinutes(-30)),
        new(Guid.NewGuid(), "Barrera Principal", DeviceCategory.Acceso, ConnectionLifecycle.OnDemand, DeviceStatus.Ready, DateTimeOffset.UtcNow.AddMinutes(-15)),
        new(Guid.NewGuid(), "Semaforo Muelle", DeviceCategory.Senalizacion, ConnectionLifecycle.OnDemand, DeviceStatus.Disconnected, DateTimeOffset.UtcNow.AddHours(-2)),
        new(Guid.NewGuid(), "Cartel Recepcion", DeviceCategory.Visualizacion, ConnectionLifecycle.FireAndForget, DeviceStatus.Ready, DateTimeOffset.UtcNow.AddMinutes(-5)),
    ];

    /// <inheritdoc />
    public Task<IReadOnlyCollection<DeviceSnapshot>> GetDashboardSnapshotAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyCollection<DeviceSnapshot>>(_devices.AsReadOnly());
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<DeviceSnapshot>> TestAllConnectionsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        for (var index = 0; index < _devices.Count; index++)
        {
            var device = _devices[index];
            var nextStatus = device.Status switch
            {
                DeviceStatus.Unknown => DeviceStatus.Connected,
                DeviceStatus.Connected => DeviceStatus.Ready,
                DeviceStatus.Ready => DeviceStatus.Connected,
                DeviceStatus.Disconnected => DeviceStatus.Connected,
                _ => DeviceStatus.Unknown,
            };

            _devices[index] = device with
            {
                Status = nextStatus,
                LastConnectionUtc = nextStatus is DeviceStatus.Connected or DeviceStatus.Ready
                    ? DateTimeOffset.UtcNow
                    : device.LastConnectionUtc,
            };
        }

        return Task.FromResult<IReadOnlyCollection<DeviceSnapshot>>(_devices.AsReadOnly());
    }
}
