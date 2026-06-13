namespace IotMonitor.Data.Entities;

public class HttpDeviceEntity : DeviceEntity
{
    public string EndpointPath { get; set; } = null!;
}
