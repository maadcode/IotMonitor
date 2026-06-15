using System.Net.Sockets;
using IotMonitor.Domain.Abstractions;
using IotMonitor.Domain.Enums;
using IotMonitor.Domain.Interfaces;
using NModbus;

namespace IotMonitor.Infrastructure.Devices;




public sealed class ModbusDevice : DeviceBase, IAccessController, ILightController
{
    private readonly byte _unitId;
    private readonly ushort _primaryCoil;

    public ModbusDevice(
        Guid id,
        string alias,
        string ipAddress,
        int port,
        DeviceCategory category,
        ConnectionLifecycle lifecycle,
        byte unitId,
        ushort primaryCoil)
        : base(id, alias, ipAddress, port, category, lifecycle)
    {
        _unitId = unitId;
        _primaryCoil = primaryCoil;
    }

    
    public override async Task<bool> TestConnectionAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(IpAddress, Port, cancellationToken);
            
            
            if (await Task.WhenAny(connectTask.AsTask(), Task.Delay(2000, cancellationToken)) != connectTask.AsTask())
            {
                return false;
            }

            var factory = new ModbusFactory();
            var master = factory.CreateMaster(client);
            master.Transport.ReadTimeout = 1000;

            
            await master.ReadCoilsAsync(_unitId, _primaryCoil, 1);
            return true;
        }
        catch
        {
            return false;
        }
    }

    
    public async Task SetAccessStateAsync(bool active, CancellationToken cancellationToken)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(IpAddress, Port, cancellationToken);

        var factory = new ModbusFactory();
        var master = factory.CreateMaster(client);
        
        await master.WriteSingleCoilAsync(_unitId, _primaryCoil, active);
    }

    
    public async Task SetColorAsync(string color, CancellationToken cancellationToken)
    {
        
        
        ushort offset = color.ToLowerInvariant() switch
        {
            "red" or "rojo" => 0,
            "yellow" or "amarillo" => 1,
            "green" or "verde" => 2,
            _ => 0
        };

        using var client = new TcpClient();
        await client.ConnectAsync(IpAddress, Port, cancellationToken);

        var factory = new ModbusFactory();
        var master = factory.CreateMaster(client);

        
        await master.WriteMultipleCoilsAsync(_unitId, _primaryCoil, [false, false, false]);
        
        
        await master.WriteSingleCoilAsync(_unitId, (ushort)(_primaryCoil + offset), true);
    }
}
