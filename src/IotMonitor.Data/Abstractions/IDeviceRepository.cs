using IotMonitor.Data.Entities;

namespace IotMonitor.Data.Abstractions;

public interface IDeviceRepository
{
    Task<IEnumerable<DeviceTypeEntity>> GetDeviceTypesAsync(CancellationToken ct = default);
    Task<IEnumerable<DeviceEntity>> GetAllDevicesAsync(CancellationToken ct = default);
    Task<DeviceEntity?> GetDeviceByIdAsync(Guid id, CancellationToken ct = default);
    Task AddDeviceAsync(DeviceEntity device, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
