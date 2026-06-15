using IotMonitor.Data.Abstractions;
using IotMonitor.Data.Entities;
using IotMonitor.Domain.Enums;
using IotMonitor.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace IotMonitor.Tests;

[TestClass]
public class LightControlServiceTests
{
    private Mock<IServiceScopeFactory> _scopeFactoryMock = null!;
    private Mock<IServiceScope> _scopeMock = null!;
    private Mock<IServiceProvider> _serviceProviderMock = null!;
    private Mock<IDeviceRepository> _repositoryMock = null!;
    private Mock<ILogger<LightControlService>> _loggerMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _scopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _repositoryMock = new Mock<IDeviceRepository>();
        _loggerMock = new Mock<ILogger<LightControlService>>();

        _scopeFactoryMock.Setup(x => x.CreateScope()).Returns(_scopeMock.Object);
        _scopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IDeviceRepository))).Returns(_repositoryMock.Object);
    }

    [TestMethod]
    public async Task SetColorAsync_ShouldThrow_WhenDeviceNotFound()
    {
        
        var deviceId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetDeviceByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DeviceEntity?)null);

        var service = new LightControlService(_scopeFactoryMock.Object, _loggerMock.Object);

        
        bool thrown = false;
        try 
        {
            await service.SetColorAsync(deviceId, "Red", CancellationToken.None);
        }
        catch (InvalidOperationException)
        {
            thrown = true;
        }
        Assert.IsTrue(thrown, "Expected InvalidOperationException was not thrown.");
    }

    [TestMethod]
    public async Task SetColorAsync_ShouldThrow_WhenDeviceIsNotModbus()
    {
        
        var deviceId = Guid.NewGuid();
        var entity = new HttpDeviceEntity
        {
            Id = deviceId,
            DeviceType = new DeviceTypeEntity { Lifecycle = ConnectionLifecycle.OnDemand },
            EndpointPath = "/"
        };
        
        _repositoryMock.Setup(x => x.GetDeviceByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var service = new LightControlService(_scopeFactoryMock.Object, _loggerMock.Object);

        
        bool thrown = false;
        try 
        {
            await service.SetColorAsync(deviceId, "Green", CancellationToken.None);
        }
        catch (InvalidOperationException)
        {
            thrown = true;
        }
        Assert.IsTrue(thrown, "Expected InvalidOperationException was not thrown.");
    }
}
