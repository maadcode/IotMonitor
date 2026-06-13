using IotMonitor.Domain.Enums;

namespace IotMonitor.Data.Entities;

public class DeviceTypeEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ConnectionLifecycle Lifecycle { get; set; }

    // Navigation
    public virtual ICollection<DeviceEntity> Devices { get; set; } = new HashSet<DeviceEntity>();
}
