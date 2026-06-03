using IotMonitor.Domain.Enums;
using IotMonitor.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace IotMonitor.Data;

/// <summary>
/// EF Core DbContext for the IoT Monitor persistence store.
/// </summary>
public sealed class IotMonitorDbContext(DbContextOptions<IotMonitorDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets the device types catalog.
    /// </summary>
    public DbSet<DeviceType> DeviceTypes => Set<DeviceType>();

    /// <summary>
    /// Gets the base device entities.
    /// </summary>
    public DbSet<Device> Devices => Set<Device>();

    /// <summary>
    /// Gets the Modbus configuration entities.
    /// </summary>
    public DbSet<ModbusConfig> ModbusConfigs => Set<ModbusConfig>();

    /// <summary>
    /// Gets the HTTP configuration entities.
    /// </summary>
    public DbSet<HttpConfig> HttpConfigs => Set<HttpConfig>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeviceType>(entity =>
        {
            entity.ToTable("DeviceTypes");
            entity.HasKey(type => type.Code);
            entity.Property(type => type.Code).HasMaxLength(64).IsRequired();
            entity.Property(type => type.Name).HasMaxLength(200).IsRequired();
            entity.Property(type => type.Protocol).HasMaxLength(100).IsRequired();
            entity.Property(type => type.Lifecycle).HasConversion<int>().IsRequired();
            entity.HasMany(type => type.Devices)
                .WithOne(device => device.DeviceType)
                .HasForeignKey(device => device.DeviceTypeCode)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.UseTptMappingStrategy();
            entity.ToTable("Devices");
            entity.HasKey(device => device.Id);
            entity.Property(device => device.Alias).HasMaxLength(200).IsRequired();
            entity.Property(device => device.DeviceTypeCode).HasMaxLength(64).IsRequired();
            entity.Property(device => device.IpAddress).HasMaxLength(64).IsRequired();
            entity.Property(device => device.Port).IsRequired();
            entity.Property(device => device.Category).HasConversion<int>().IsRequired();
        });

        modelBuilder.Entity<ModbusConfig>(entity =>
        {
            entity.ToTable("ModbusConfigs");
            entity.Property(config => config.UnitId).IsRequired();
            entity.Property(config => config.PrimaryCoil).IsRequired();
        });

        modelBuilder.Entity<HttpConfig>(entity =>
        {
            entity.ToTable("HttpConfigs");
            entity.Property(config => config.EndpointPath).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<DeviceType>().HasData(
            new
            {
                Code = "SENSOR_MODBUS",
                Name = "Sensor Modbus",
                Protocol = "Modbus TCP",
                Lifecycle = ConnectionLifecycle.AlwaysOn,
            },
            new
            {
                Code = "BARRERA_MODBUS",
                Name = "Barrera Modbus",
                Protocol = "Modbus TCP",
                Lifecycle = ConnectionLifecycle.OnDemand,
            },
            new
            {
                Code = "SEMAFORO_MODBUS",
                Name = "Semaforo Modbus",
                Protocol = "Modbus TCP",
                Lifecycle = ConnectionLifecycle.OnDemand,
            },
            new
            {
                Code = "CAMARA_HTTP",
                Name = "Camara HTTP",
                Protocol = "HTTP",
                Lifecycle = ConnectionLifecycle.OnDemand,
            },
            new
            {
                Code = "CARTEL_UDP",
                Name = "Cartel UDP",
                Protocol = "UDP",
                Lifecycle = ConnectionLifecycle.FireAndForget,
            });
    }
}
