using IotMonitor.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IotMonitor.Data.Configurations;

public class DeviceTypeConfiguration : IEntityTypeConfiguration<DeviceTypeEntity>
{
    public void Configure(EntityTypeBuilder<DeviceTypeEntity> builder)
    {
        builder.ToTable("DeviceTypes");

        builder.HasKey(e => e.Code);
        builder.Property(e => e.Code).HasMaxLength(50);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);

        
        builder.HasData(
            new DeviceTypeEntity { Code = "SENSOR_MODBUS", Name = "Sensor Modbus TCP", Lifecycle = Domain.Enums.ConnectionLifecycle.AlwaysOn },
            new DeviceTypeEntity { Code = "CAMARA_HTTP", Name = "Cámara HTTP", Lifecycle = Domain.Enums.ConnectionLifecycle.OnDemand },
            new DeviceTypeEntity { Code = "BARRERA_MODBUS", Name = "Barrera Modbus TCP", Lifecycle = Domain.Enums.ConnectionLifecycle.OnDemand },
            new DeviceTypeEntity { Code = "SEMAFORO_MODBUS", Name = "Semáforo Modbus TCP", Lifecycle = Domain.Enums.ConnectionLifecycle.OnDemand },
            new DeviceTypeEntity { Code = "CARTEL_UDP", Name = "Cartel LED UDP", Lifecycle = Domain.Enums.ConnectionLifecycle.FireAndForget }
        );
    }
}

public class DeviceConfiguration : IEntityTypeConfiguration<DeviceEntity>
{
    public void Configure(EntityTypeBuilder<DeviceEntity> builder)
    {
        builder.ToTable("Devices");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Alias).IsRequired().HasMaxLength(200);
        builder.Property(e => e.IpAddress).IsRequired().HasMaxLength(50);
        builder.Property(e => e.DeviceTypeCode).IsRequired().HasMaxLength(50);

        builder.HasOne(d => d.DeviceType)
            .WithMany(p => p.Devices)
            .HasForeignKey(d => d.DeviceTypeCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ModbusDeviceConfiguration : IEntityTypeConfiguration<ModbusDeviceEntity>
{
    public void Configure(EntityTypeBuilder<ModbusDeviceEntity> builder)
    {
        builder.ToTable("ModbusConfigs");
    }
}

public class HttpDeviceConfiguration : IEntityTypeConfiguration<HttpDeviceEntity>
{
    public void Configure(EntityTypeBuilder<HttpDeviceEntity> builder)
    {
        builder.ToTable("HttpConfigs");
    }
}
