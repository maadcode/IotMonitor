namespace IotMonitor.Domain.Enums;

/// <summary>
/// Represents the runtime connectivity status displayed in the dashboard.
/// </summary>
public enum DeviceStatus
{
    Unknown = 0,
    Connected = 1,
    Disconnected = 2,
    Ready = 3,
}
