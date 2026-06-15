using IotMonitor.Domain.Enums;
using IotMonitor.Infrastructure.Devices;

namespace IotMonitor.Tests;

[TestClass]
public sealed class ProtocolHandlersTests
{
    [TestMethod]
    public async Task HttpDevice_TestConnection_ShouldReturnFalse_WhenEndpointIsUnreachable()
    {
        
        var device = new HttpDevice(
            Guid.NewGuid(),
            "Test Cam",
            "192.0.2.1", 
            8080,
            DeviceCategory.Seguridad,
            ConnectionLifecycle.OnDemand,
            "/test");

        
        var result = await device.TestConnectionAsync(CancellationToken.None);

        
        Assert.IsFalse(result, "Connection should fail for unreachable IP.");
    }

    [TestMethod]
    public async Task ModbusDevice_TestConnection_ShouldReturnFalse_WhenEndpointIsUnreachable()
    {
        
        var device = new ModbusDevice(
            Guid.NewGuid(),
            "Test PLC",
            "192.0.2.1",
            502,
            DeviceCategory.Acceso,
            ConnectionLifecycle.OnDemand,
            1,
            0);

        
        var result = await device.TestConnectionAsync(CancellationToken.None);

        
        Assert.IsFalse(result, "Connection should fail for unreachable IP.");
    }

    [TestMethod]
    public async Task UdpDevice_TestConnection_ShouldReturnTrue_WhenEndpointIsValidlyFormatted()
    {
        
        
        var device = new UdpDevice(
            Guid.NewGuid(),
            "Test Sign",
            "127.0.0.1",
            9999,
            DeviceCategory.Visualizacion,
            ConnectionLifecycle.FireAndForget);

        
        var result = await device.TestConnectionAsync(CancellationToken.None);

        
        Assert.IsTrue(result, "UDP 'Connection' (socket open) should succeed for local loopback.");
    }
}
