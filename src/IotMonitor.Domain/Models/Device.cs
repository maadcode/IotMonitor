using IotMonitor.Domain.Enums;

namespace IotMonitor.Domain.Models;

/// <summary>
/// Base entity for device persistence.
/// </summary>
public class Device
{
    protected Device()
    {
    }

    /// <summary>
    /// Initializes a new <see cref="Device"/> instance.
    /// </summary>
    /// <param name="id">Unique device identifier.</param>
    /// <param name="alias">Friendly name shown in the console UI.</param>
    /// <param name="deviceTypeCode">Device type code from catalog.</param>
    /// <param name="ipAddress">Target IP address.</param>
    /// <param name="port">Target network port.</param>
    /// <param name="category">Logical device category.</param>
    public Device(
        Guid id,
        string alias,
        string deviceTypeCode,
        string ipAddress,
        int port,
        DeviceCategory category)
    {
        Id = id;
        Alias = alias;
        DeviceTypeCode = deviceTypeCode;
        IpAddress = ipAddress;
        Port = port;
        Category = category;
    }

    /// <summary>
    /// Gets the unique identifier for the device.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the user-friendly alias.
    /// </summary>
    public string Alias { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the device type code.
    /// </summary>
    public string DeviceTypeCode { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the navigation to the device type.
    /// </summary>
    public DeviceType? DeviceType { get; private set; }

    /// <summary>
    /// Gets the configured device IP address.
    /// </summary>
    public string IpAddress { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the configured device port.
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// Gets the logical category used by the menu hierarchy.
    /// </summary>
    public DeviceCategory Category { get; private set; }
}
