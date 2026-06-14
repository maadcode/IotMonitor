namespace IotMonitor.Application.Abstractions;

/// <summary>
/// Captures a photo from an HTTP-based photographic device and persists it to local storage.
/// </summary>
public interface IPhotoCaptureService
{
    /// <summary>
    /// Captures a photo from the specified device and saves it to the configured path.
    /// </summary>
    /// <param name="deviceId">The unique identifier of the photographic device.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The absolute path where the image was saved.</returns>
    Task<string> CapturePhotoAsync(Guid deviceId, CancellationToken cancellationToken);
}
