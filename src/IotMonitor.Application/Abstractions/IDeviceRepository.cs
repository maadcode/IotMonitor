using IotMonitor.Domain.Enums;
using IotMonitor.Domain.Models;

namespace IotMonitor.Application.Abstractions;

/// <summary>
/// Query repository for persisted devices.
/// </summary>
public interface IDeviceRepository
{
    /// <summary>
    /// Gets all devices.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Collection of devices.</returns>
    Task<IReadOnlyCollection<Device>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a device by identifier.
    /// </summary>
    /// <param name="id">Device identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The matching device or null.</returns>
    Task<Device?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Gets devices by category.
    /// </summary>
    /// <param name="category">Category filter.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Collection of devices in the category.</returns>
    Task<IReadOnlyCollection<Device>> GetByCategoryAsync(DeviceCategory category, CancellationToken cancellationToken);

    /// <summary>
    /// Gets devices by device type code.
    /// </summary>
    /// <param name="deviceTypeCode">Device type code.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Collection of devices with the type code.</returns>
    Task<IReadOnlyCollection<Device>> GetByTypeCodeAsync(string deviceTypeCode, CancellationToken cancellationToken);
}
