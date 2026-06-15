namespace IotMonitor.Application.Abstractions;

/// <summary>
/// Application service for controlling access mechanisms (barriers, gates).
/// </summary>
public interface IAccessControlService
{
    /// <summary>
    /// Changes the state of an access control device.
    /// </summary>
    /// <param name="deviceId">Unique identifier of the device.</param>
    /// <param name="active">True to open/activate; false to close/deactivate.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A completion task.</returns>
    Task SetAccessStateAsync(Guid deviceId, bool active, CancellationToken cancellationToken);
}
