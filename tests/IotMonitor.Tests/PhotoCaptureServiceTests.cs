using IotMonitor.Data.Abstractions;
using IotMonitor.Data.Entities;
using IotMonitor.Domain.Enums;
using IotMonitor.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace IotMonitor.Tests;

[TestClass]
public class PhotoCaptureServiceTests
{
    private Mock<IServiceScopeFactory> _scopeFactoryMock = null!;
    private Mock<IServiceScope> _scopeMock = null!;
    private Mock<IServiceProvider> _serviceProviderMock = null!;
    private Mock<IDeviceRepository> _repositoryMock = null!;
    private Mock<ILogger<PhotoCaptureService>> _loggerMock = null!;
    private Mock<IConfiguration> _configurationMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _scopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _repositoryMock = new Mock<IDeviceRepository>();
        _loggerMock = new Mock<ILogger<PhotoCaptureService>>();
        _configurationMock = new Mock<IConfiguration>();

        _scopeFactoryMock.Setup(x => x.CreateScope()).Returns(_scopeMock.Object);
        _scopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IDeviceRepository))).Returns(_repositoryMock.Object);

        _configurationMock.Setup(x => x["PhotoStorage:BasePath"]).Returns("test_photos");
    }

    [TestMethod]
    public async Task CapturePhotoAsync_ShouldThrow_WhenDeviceNotFound()
    {
        
        var deviceId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetDeviceByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DeviceEntity?)null);

        var service = new PhotoCaptureService(_scopeFactoryMock.Object, _configurationMock.Object, _loggerMock.Object);

        
        bool thrown = false;
        try 
        {
            await service.CapturePhotoAsync(deviceId, CancellationToken.None);
        }
        catch (InvalidOperationException)
        {
            thrown = true;
        }
        Assert.IsTrue(thrown, "Expected InvalidOperationException was not thrown.");
    }

    [TestMethod]
    public async Task CapturePhotoAsync_ShouldThrow_WhenDeviceIsNotHttp()
    {
        
        var deviceId = Guid.NewGuid();
        var entity = new ModbusDeviceEntity
        {
            Id = deviceId,
            DeviceType = new DeviceTypeEntity { Lifecycle = ConnectionLifecycle.OnDemand }
        };
        
        _repositoryMock.Setup(x => x.GetDeviceByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var service = new PhotoCaptureService(_scopeFactoryMock.Object, _configurationMock.Object, _loggerMock.Object);

        
        bool thrown = false;
        try 
        {
            await service.CapturePhotoAsync(deviceId, CancellationToken.None);
        }
        catch (InvalidOperationException)
        {
            thrown = true;
        }
        Assert.IsTrue(thrown, "Expected InvalidOperationException was not thrown.");
    }
}
