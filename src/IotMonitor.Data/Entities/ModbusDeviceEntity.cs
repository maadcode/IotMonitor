namespace IotMonitor.Data.Entities;

public class ModbusDeviceEntity : DeviceEntity
{
    public int UnitId { get; set; }
    public int PrimaryCoil { get; set; }
}
