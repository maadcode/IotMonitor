namespace IotMonitor.Domain.Interfaces;

/// <summary>
/// Capability for devices able to capture an image.
/// </summary>
public interface IPhotographic
{
    /// <summary>
    /// Captures an image and stores it in the provided file path.
    /// </summary>
    /// <param name="targetFilePath">Destination path for the captured image file.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The resulting file path.</returns>
    Task<string> CapturePhotoAsync(string targetFilePath, CancellationToken cancellationToken);
}
