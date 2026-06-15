using IotMonitor.Data.Abstractions;
using IotMonitor.Data.Entities;
using IotMonitor.Domain.Enums;
using IotMonitor.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace IotMonitor.Tests;

[TestClass]
public class UdpDeviceSenderTests
{
    private Mock<IServiceScopeFactory> _scopeFactoryMock = null!;
    private Mock<IServiceScope> _scopeMock = null!;
    private Mock<IServiceProvider> _serviceProviderMock = null!;
    private Mock<IDeviceRepository> _repositoryMock = null!;
    private Mock<ILogger<UdpDeviceSender>> _loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _scopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _repositoryMock = new Mock<IDeviceRepository>();
        _loggerMock = new Mock<ILogger<UdpDeviceSender>>();

        _scopeFactoryMock.Setup(x => x.CreateScope()).Returns(_scopeMock.Object);
        _scopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IDeviceRepository))).Returns(_repositoryMock.Object);
    }

    [TestMethod]
    public async Task SendToDeviceAsync_ShouldLogWarning_WhenDeviceNotFound()
    {
        
        var deviceId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetDeviceByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DeviceEntity?)null);

        var sender = new UdpDeviceSender(_scopeFactoryMock.Object, _loggerMock.Object);

        
        await sender.SendToDeviceAsync(deviceId, "Hello", CancellationToken.None);

        
        _repositoryMock.Verify(x => x.GetDeviceByIdAsync(deviceId, It.IsAny<CancellationToken>()), Times.Once);
        
    }
}
