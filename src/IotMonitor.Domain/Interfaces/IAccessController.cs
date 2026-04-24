namespace IotMonitor.Domain.Interfaces;

/// <summary>
/// Capability for access control devices like barriers.
/// </summary>
public interface IAccessController
{
    /// <summary>
    /// Activates or deactivates the access mechanism.
    /// </summary>
    /// <param name="active">True to activate; otherwise false.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A completion task.</returns>
    Task SetAccessStateAsync(bool active, CancellationToken cancellationToken);
}
