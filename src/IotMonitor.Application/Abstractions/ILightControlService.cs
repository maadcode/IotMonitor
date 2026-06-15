namespace IotMonitor.Application.Abstractions;

/// <summary>
/// Application service for controlling signaling lights (traffic lights).
/// </summary>
public interface ILightControlService
{
    /// <summary>
    /// Sets the active color of a light controller.
    /// </summary>
    /// <param name="deviceId">Unique identifier of the device.</param>
    /// <param name="color">The color name to activate.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A completion task.</returns>
    Task SetColorAsync(Guid deviceId, string color, CancellationToken cancellationToken);
}
