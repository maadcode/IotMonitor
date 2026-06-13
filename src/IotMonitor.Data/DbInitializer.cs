using IotMonitor.Data.Entities;
using IotMonitor.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace IotMonitor.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IotMonitorDbContext context)
    {
        if (await context.Devices.AnyAsync()) return;

        var deviceTypes = new List<DeviceTypeEntity>
        {
            new() { Code = "SENSOR_MODBUS", Name = "Sensor Industrial (Modbus)", Lifecycle = ConnectionLifecycle.AlwaysOn },
            new() { Code = "CAMARA_HTTP", Name = "Camara Seguridad (HTTP)", Lifecycle = ConnectionLifecycle.OnDemand },
            new() { Code = "BARRERA_MODBUS", Name = "Barrera Acceso (Modbus)", Lifecycle = ConnectionLifecycle.OnDemand },
            new() { Code = "CARTEL_UDP", Name = "Cartel Informativo (UDP)", Lifecycle = ConnectionLifecycle.FireAndForget }
        };

        context.DeviceTypes.AddRange(deviceTypes);
        await context.SaveChangesAsync();

        var devices = new List<DeviceEntity>
        {
            new ModbusDeviceEntity
            {
                Id = Guid.NewGuid(),
                Alias = "Sensor Temperatura Tanque",
                DeviceTypeCode = "SENSOR_MODBUS",
                IpAddress = "iotmonitor-nodered",
                Port = 1502,
                CategoryId = DeviceCategory.Lectura,
                UnitId = 1,
                PrimaryCoil = 0
            },
            new HttpDeviceEntity
            {
                Id = Guid.NewGuid(),
                Alias = "Camara Acceso Principal",
                DeviceTypeCode = "CAMARA_HTTP",
                IpAddress = "iotmonitor-nodered",
                Port = 1880,
                CategoryId = DeviceCategory.Seguridad,
                EndpointPath = "/camera/snapshot"
            },
            new ModbusDeviceEntity
            {
                Id = Guid.NewGuid(),
                Alias = "Barrera Entrada Vehiculos",
                DeviceTypeCode = "BARRERA_MODBUS",
                IpAddress = "iotmonitor-nodered",
                Port = 1502,
                CategoryId = DeviceCategory.Acceso,
                UnitId = 1,
                PrimaryCoil = 10
            },
            new UdpDeviceEntity
            {
                Id = Guid.NewGuid(),
                Alias = "Panel LED Bienvenida",
                DeviceTypeCode = "CARTEL_UDP",
                IpAddress = "iotmonitor-nodered",
                Port = 9999,
                CategoryId = DeviceCategory.Visualizacion
            }
        };

        context.Devices.AddRange(devices);
        await context.SaveChangesAsync();
    }
}
