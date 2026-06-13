using IotMonitor.Data.Abstractions;
using IotMonitor.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IotMonitor.Data.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly IotMonitorDbContext _context;

    public DeviceRepository(IotMonitorDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DeviceTypeEntity>> GetDeviceTypesAsync(CancellationToken ct = default)
    {
        return await _context.DeviceTypes.ToListAsync(ct);
    }

    public async Task<IEnumerable<DeviceEntity>> GetAllDevicesAsync(CancellationToken ct = default)
    {
        return await _context.Devices
            .Include(d => d.DeviceType)
            .ToListAsync(ct);
    }

    public async Task<DeviceEntity?> GetDeviceByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Devices
            .Include(d => d.DeviceType)
            .FirstOrDefaultAsync(d => d.Id == id, ct);
    }

    public async Task AddDeviceAsync(DeviceEntity device, CancellationToken ct = default)
    {
        await _context.Devices.AddAsync(device, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}
