using IotMonitor.Application.Abstractions;
using IotMonitor.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace IotMonitor.Data.Repositories;

/// <summary>
/// EF Core repository for HTTP configurations.
/// </summary>
public sealed class HttpConfigRepository(IotMonitorDbContext context) : IHttpConfigRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<HttpConfig>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.HttpConfigs
            .AsNoTracking()
            .OrderBy(config => config.Alias)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task<HttpConfig?> GetByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        return context.HttpConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(config => config.Id == deviceId, cancellationToken);
    }
}
