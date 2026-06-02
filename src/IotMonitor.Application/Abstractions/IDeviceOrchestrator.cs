using IotMonitor.Domain.Models;

namespace IotMonitor.Application.Abstractions;

/// <summary>
/// Exposes stateful orchestration operations used by the console interface.
/// </summary>
public interface IDeviceOrchestrator
{
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
}
