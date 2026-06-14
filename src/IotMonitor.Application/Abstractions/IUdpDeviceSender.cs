namespace IotMonitor.Application.Abstractions;

/// <summary>
/// Sends a fire-and-forget message to a UDP device identified by its ID.
/// </summary>
public interface IUdpDeviceSender
{
    /// <summary>
    /// Opens a UDP socket, sends <paramref name="message"/>, then immediately closes the socket.
    /// No response is expected or waited for.
    /// </summary>
    Task SendToDeviceAsync(Guid deviceId, string message, CancellationToken cancellationToken);
}
