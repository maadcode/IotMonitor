using IotMonitor.Domain.Enums;

namespace IotMonitor.Domain.Models;







public record DeviceStatusChangedEventArgs(Guid DeviceId, DeviceStatus OldStatus, DeviceStatus NewStatus);
