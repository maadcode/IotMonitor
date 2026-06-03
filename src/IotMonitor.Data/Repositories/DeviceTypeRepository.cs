using IotMonitor.Application.Abstractions;
using IotMonitor.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace IotMonitor.Data.Repositories;

/// <summary>
/// EF Core repository for device types.
/// </summary>
public sealed class DeviceTypeRepository(IotMonitorDbContext context) : IDeviceTypeRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DeviceType>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.DeviceTypes
            .AsNoTracking()
            .OrderBy(type => type.Code)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task<DeviceType?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return context.DeviceTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(type => type.Code == code, cancellationToken);
    }
}
