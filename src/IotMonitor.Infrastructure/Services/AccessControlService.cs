using IotMonitor.Application.Abstractions;
using IotMonitor.Data.Abstractions;
using IotMonitor.Data.Entities;
using IotMonitor.Infrastructure.Devices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IotMonitor.Infrastructure.Services;

public sealed class AccessControlService(IServiceScopeFactory scopeFactory, ILogger<AccessControlService> logger) 
    : IAccessControlService
{
    public async Task SetAccessStateAsync(Guid deviceId, bool active, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();

        var entity = await repository.GetDeviceByIdAsync(deviceId, cancellationToken);
        if (entity is not ModbusDeviceEntity modbusEntity)
        {
            throw new InvalidOperationException($"Device {deviceId} is not a Modbus device or was not found.");
        }

        var device = new ModbusDevice(
            modbusEntity.Id,
            modbusEntity.Alias,
            modbusEntity.IpAddress,
            modbusEntity.Port,
            modbusEntity.CategoryId,
            modbusEntity.DeviceType.Lifecycle,
            (byte)modbusEntity.UnitId,
            (ushort)modbusEntity.PrimaryCoil);

        logger.LogInformation("[Modbus] Setting access state for '{Alias}' ({Id}) to {State}", 
            modbusEntity.Alias, deviceId, active ? "Active/Open" : "Inactive/Closed");

        await device.SetAccessStateAsync(active, cancellationToken).ConfigureAwait(false);
    }
}
