using IotMonitor.Domain.Models;

namespace IotMonitor.Application.Abstractions;

/// <summary>
/// Exposes stateful orchestration operations used by the console interface.
/// </summary>
public interface IDeviceOrchestrator
{
    /// <summary>
    /// Event fired when a device status changes.
    /// </summary>
    event EventHandler<DeviceStatusChangedEventArgs> StatusChanged;

    /// <summary>
    /// Gets the current dashboard snapshot.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Collection of device snapshots.</returns>
    Task<IReadOnlyCollection<DeviceSnapshot>> GetDashboardSnapshotAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Performs a connectivity test for all devices and updates runtime state.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Updated collection of device snapshots.</returns>
    Task<IReadOnlyCollection<DeviceSnapshot>> TestAllConnectionsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Triggers a manual reconnection for a specific device.
    /// </summary>
    /// <param name="deviceId">The unique identifier of the device.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A completion task.</returns>
    Task ReconnectDeviceAsync(Guid deviceId, CancellationToken cancellationToken);
}
