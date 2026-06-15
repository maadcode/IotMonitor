using IotMonitor.Application.Abstractions;
using IotMonitor.Data.Abstractions;
using IotMonitor.Data.Entities;
using IotMonitor.Infrastructure.Devices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IotMonitor.Infrastructure.Services;

public sealed class LightControlService(IServiceScopeFactory scopeFactory, ILogger<LightControlService> logger) 
    : ILightControlService
{
    public async Task SetColorAsync(Guid deviceId, string color, CancellationToken cancellationToken)
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

        logger.LogInformation("[Modbus] Setting color for '{Alias}' ({Id}) to {Color}", 
            modbusEntity.Alias, deviceId, color);

        await device.SetColorAsync(color, cancellationToken).ConfigureAwait(false);
    }
}
