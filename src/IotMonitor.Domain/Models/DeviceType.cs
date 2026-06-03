using IotMonitor.Domain.Enums;

namespace IotMonitor.Domain.Models;

/// <summary>
/// Immutable catalog definition for supported device types.
/// </summary>
public sealed class DeviceType
{
    private DeviceType()
    {
    }

    /// <summary>
    /// Initializes a new <see cref="DeviceType"/> instance.
    /// </summary>
    /// <param name="code">Unique alphanumeric device type code.</param>
    /// <param name="name">Friendly name for UI.</param>
    /// <param name="protocol">Underlying protocol name.</param>
    /// <param name="lifecycle">Communication lifecycle for the type.</param>
    public DeviceType(string code, string name, string protocol, ConnectionLifecycle lifecycle)
    {
        Code = code;
        Name = name;
        Protocol = protocol;
        Lifecycle = lifecycle;
    }

    /// <summary>
    /// Gets the unique device type code.
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the friendly display name.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the protocol name.
    /// </summary>
    public string Protocol { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the lifecycle classification.
    /// </summary>
    public ConnectionLifecycle Lifecycle { get; private set; }

    /// <summary>
    /// Gets the devices that use this type.
    /// </summary>
    public ICollection<Device> Devices { get; private set; } = new List<Device>();
}
