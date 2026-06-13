using IotMonitor.Domain.Enums;

namespace IotMonitor.Data.Entities;

public abstract class DeviceEntity
{
    public Guid Id { get; set; }
    public string Alias { get; set; } = null!;
    public string DeviceTypeCode { get; set; } = null!;
    public string IpAddress { get; set; } = null!;
    public int Port { get; set; }
    public DeviceCategory CategoryId { get; set; }

    // Navigation
    public virtual DeviceTypeEntity DeviceType { get; set; } = null!;
}
