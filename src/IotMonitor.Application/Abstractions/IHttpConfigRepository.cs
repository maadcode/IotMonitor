using IotMonitor.Domain.Models;

namespace IotMonitor.Application.Abstractions;

/// <summary>
/// Query repository for HTTP configurations.
/// </summary>
public interface IHttpConfigRepository
{
    /// <summary>
    /// Gets all HTTP configurations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Collection of HTTP configurations.</returns>
    Task<IReadOnlyCollection<HttpConfig>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets an HTTP configuration by device identifier.
    /// </summary>
    /// <param name="deviceId">Device identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The matching configuration or null.</returns>
    Task<HttpConfig?> GetByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken);
}
