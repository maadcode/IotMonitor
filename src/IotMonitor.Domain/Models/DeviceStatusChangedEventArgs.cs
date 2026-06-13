using IotMonitor.Domain.Enums;

namespace IotMonitor.Domain.Models;

/// <summary>
/// Event arguments for device status change events.
/// </summary>
/// <param name="DeviceId">The unique identifier of the device.</param>
/// <param name="OldStatus">The previous status of the device.</param>
/// <param name="NewStatus">The new status of the device.</param>
public record DeviceStatusChangedEventArgs(Guid DeviceId, DeviceStatus OldStatus, DeviceStatus NewStatus);
