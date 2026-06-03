using IotMonitor.Domain.Models;

namespace IotMonitor.Application.Abstractions;

/// <summary>
/// Query repository for Modbus configurations.
/// </summary>
public interface IModbusConfigRepository
{
    /// <summary>
    /// Gets all Modbus configurations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Collection of Modbus configurations.</returns>
    Task<IReadOnlyCollection<ModbusConfig>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a Modbus configuration by device identifier.
    /// </summary>
    /// <param name="deviceId">Device identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The matching configuration or null.</returns>
    Task<ModbusConfig?> GetByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken);
}
