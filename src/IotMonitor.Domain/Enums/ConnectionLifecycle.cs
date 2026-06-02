namespace IotMonitor.Domain.Enums;

/// <summary>
/// Defines communication lifecycle patterns for a device.
/// </summary>
public enum ConnectionLifecycle
{
    AlwaysOn = 1,
    OnDemand = 2,
    FireAndForget = 3,
}
