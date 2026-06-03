using IotMonitor.Domain.Enums;

namespace IotMonitor.Domain.Models;

/// <summary>
/// Modbus-specific configuration mapped via TPT.
/// </summary>
public sealed class ModbusConfig : Device
{
    private ModbusConfig()
    {
    }

    /// <summary>
    /// Initializes a new <see cref="ModbusConfig"/> instance.
    /// </summary>
    /// <param name="id">Unique device identifier.</param>
    /// <param name="alias">Friendly name shown in the console UI.</param>
    /// <param name="deviceTypeCode">Device type code from catalog.</param>
    /// <param name="ipAddress">Target IP address.</param>
    /// <param name="port">Target network port.</param>
    /// <param name="category">Logical device category.</param>
    /// <param name="unitId">Modbus unit identifier.</param>
    /// <param name="primaryCoil">Primary coil address used for commands.</param>
    public ModbusConfig(
        Guid id,
        string alias,
        string deviceTypeCode,
        string ipAddress,
        int port,
        DeviceCategory category,
        byte unitId,
        int primaryCoil)
        : base(id, alias, deviceTypeCode, ipAddress, port, category)
    {
        UnitId = unitId;
        PrimaryCoil = primaryCoil;
    }

    /// <summary>
    /// Gets the Modbus unit identifier.
    /// </summary>
    public byte UnitId { get; private set; }

    /// <summary>
    /// Gets the primary coil address.
    /// </summary>
    public int PrimaryCoil { get; private set; }
}
