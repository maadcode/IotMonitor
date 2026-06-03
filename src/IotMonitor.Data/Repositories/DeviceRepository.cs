using IotMonitor.Application.Abstractions;
using IotMonitor.Domain.Enums;
using IotMonitor.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace IotMonitor.Data.Repositories;

/// <summary>
/// EF Core repository for device entities.
/// </summary>
public sealed class DeviceRepository(IotMonitorDbContext context) : IDeviceRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Device>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Devices
            .AsNoTracking()
            .OrderBy(device => device.Alias)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task<Device?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return context.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(device => device.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Device>> GetByCategoryAsync(
        DeviceCategory category,
        CancellationToken cancellationToken)
    {
        return await context.Devices
            .AsNoTracking()
            .Where(device => device.Category == category)
            .OrderBy(device => device.Alias)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Device>> GetByTypeCodeAsync(
        string deviceTypeCode,
        CancellationToken cancellationToken)
    {
        return await context.Devices
            .AsNoTracking()
            .Where(device => device.DeviceTypeCode == deviceTypeCode)
            .OrderBy(device => device.Alias)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
