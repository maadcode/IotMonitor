namespace IotMonitor.Domain.Interfaces;

/// <summary>
/// Capability for one-way UDP devices.
/// </summary>
public interface IUdpMessenger
{
    /// <summary>
    /// Sends a fire-and-forget message.
    /// </summary>
    /// <param name="message">Text payload to transmit.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A completion task.</returns>
    Task SendMessageAsync(string message, CancellationToken cancellationToken);
}
