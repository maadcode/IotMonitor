namespace IotMonitor.Domain.Interfaces;

/// <summary>
/// Capability for traffic lights with mutually-exclusive color control.
/// </summary>
public interface ILightController
{
    /// <summary>
    /// Sets the active color while turning off the other ones.
    /// </summary>
    /// <param name="color">Target color name.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A completion task.</returns>
    Task SetColorAsync(string color, CancellationToken cancellationToken);
}
