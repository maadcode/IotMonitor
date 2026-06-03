using IotMonitor.Application.Abstractions;
using IotMonitor.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace IotMonitor.Data.Repositories;

/// <summary>
/// EF Core repository for Modbus configurations.
/// </summary>
public sealed class ModbusConfigRepository(IotMonitorDbContext context) : IModbusConfigRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ModbusConfig>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.ModbusConfigs
            .AsNoTracking()
            .OrderBy(config => config.Alias)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task<ModbusConfig?> GetByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        return context.ModbusConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(config => config.Id == deviceId, cancellationToken);
    }
}
