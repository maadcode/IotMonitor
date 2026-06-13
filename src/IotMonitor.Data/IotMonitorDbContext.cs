using System.Reflection;
using IotMonitor.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IotMonitor.Data;

public class IotMonitorDbContext : DbContext
{
    public IotMonitorDbContext(DbContextOptions<IotMonitorDbContext> options) : base(options)
    {
    }

    public DbSet<DeviceTypeEntity> DeviceTypes => Set<DeviceTypeEntity>();
    public DbSet<DeviceEntity> Devices => Set<DeviceEntity>();
    public DbSet<ModbusDeviceEntity> ModbusDevices => Set<ModbusDeviceEntity>();
    public DbSet<HttpDeviceEntity> HttpDevices => Set<HttpDeviceEntity>();
    public DbSet<UdpDeviceEntity> UdpDevices => Set<UdpDeviceEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
