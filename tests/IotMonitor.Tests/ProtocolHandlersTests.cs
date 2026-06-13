using IotMonitor.Domain.Enums;
using IotMonitor.Infrastructure.Devices;

namespace IotMonitor.Tests;

[TestClass]
public sealed class ProtocolHandlersTests
{
    [TestMethod]
    public async Task HttpDevice_TestConnection_ShouldReturnFalse_WhenEndpointIsUnreachable()
    {
        // Arrange
        var device = new HttpDevice(
            Guid.NewGuid(),
            "Test Cam",
            "192.0.2.1", // Test-Net-1 (non-routable)
            8080,
            DeviceCategory.Seguridad,
            ConnectionLifecycle.OnDemand,
            "/test");

        // Act
        var result = await device.TestConnectionAsync(CancellationToken.None);

        // Assert
        Assert.IsFalse(result, "Connection should fail for unreachable IP.");
    }

    [TestMethod]
    public async Task ModbusDevice_TestConnection_ShouldReturnFalse_WhenEndpointIsUnreachable()
    {
        // Arrange
        var device = new ModbusDevice(
            Guid.NewGuid(),
            "Test PLC",
            "192.0.2.1",
            502,
            DeviceCategory.Acceso,
            ConnectionLifecycle.OnDemand,
            1,
            0);

        // Act
        var result = await device.TestConnectionAsync(CancellationToken.None);

        // Assert
        Assert.IsFalse(result, "Connection should fail for unreachable IP.");
    }

    [TestMethod]
    public async Task UdpDevice_TestConnection_ShouldReturnTrue_WhenEndpointIsValidlyFormatted()
    {
        // Arrange
        // UDP TestConnection currently just checks if socket can be opened/connected locally
        var device = new UdpDevice(
            Guid.NewGuid(),
            "Test Sign",
            "127.0.0.1",
            9999,
            DeviceCategory.Visualizacion,
            ConnectionLifecycle.FireAndForget);

        // Act
        var result = await device.TestConnectionAsync(CancellationToken.None);

        // Assert
        Assert.IsTrue(result, "UDP 'Connection' (socket open) should succeed for local loopback.");
    }
}
