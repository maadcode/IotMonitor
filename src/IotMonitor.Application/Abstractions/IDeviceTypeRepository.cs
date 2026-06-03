using IotMonitor.Domain.Models;

namespace IotMonitor.Application.Abstractions;

/// <summary>
/// Query repository for immutable device types.
/// </summary>
public interface IDeviceTypeRepository
{
    /// <summary>
    /// Gets all device types in the catalog.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Collection of device types.</returns>
    Task<IReadOnlyCollection<DeviceType>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a device type by its code.
    /// </summary>
    /// <param name="code">Device type code.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The matching device type or null.</returns>
    Task<DeviceType?> GetByCodeAsync(string code, CancellationToken cancellationToken);
}
