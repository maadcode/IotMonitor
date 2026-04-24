using IotMonitor.Domain.Enums;

namespace IotMonitor.Domain.Abstractions;

/// <summary>
/// Base abstraction for all device types in the orchestrator.
/// </summary>
public abstract class DeviceBase
{
    /// <summary>
    /// Initializes a new <see cref="DeviceBase"/> instance.
    /// </summary>
    /// <param name="id">Unique device identifier.</param>
    /// <param name="alias">Friendly name shown in the console UI.</param>
    /// <param name="ipAddress">Target IP address.</param>
    /// <param name="port">Target network port.</param>
    /// <param name="category">Logical device category.</param>
    /// <param name="lifecycle">Communication lifecycle behavior.</param>
    protected DeviceBase(
        Guid id,
        string alias,
        string ipAddress,
        int port,
        DeviceCategory category,
        ConnectionLifecycle lifecycle)
    {
        Id = id;
        Alias = string.IsNullOrWhiteSpace(alias)
            ? throw new ArgumentException("Alias is required.", nameof(alias))
            : alias;
        IpAddress = string.IsNullOrWhiteSpace(ipAddress)
            ? throw new ArgumentException("IP address is required.", nameof(ipAddress))
            : ipAddress;
        Port = port;
        Category = category;
        Lifecycle = lifecycle;
    }

    /// <summary>
    /// Gets the unique identifier for this device.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the user-friendly alias.
    /// </summary>
    public string Alias { get; }

    /// <summary>
    /// Gets the configured device IP address.
    /// </summary>
    public string IpAddress { get; }

    /// <summary>
    /// Gets the configured device port.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// Gets the logical category used by the menu hierarchy.
    /// </summary>
    public DeviceCategory Category { get; }

    /// <summary>
    /// Gets the communication lifecycle pattern.
    /// </summary>
    public ConnectionLifecycle Lifecycle { get; }

    /// <summary>
    /// Performs a protocol-specific connectivity test.
    /// </summary>
    /// <param name="cancellationToken">Cancellation signal for the operation.</param>
    /// <returns>True if the connectivity test succeeded; otherwise false.</returns>
    public abstract Task<bool> TestConnectionAsync(CancellationToken cancellationToken);
}
