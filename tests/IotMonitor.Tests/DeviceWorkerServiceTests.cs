using IotMonitor.Application.Services;
using IotMonitor.Data.Abstractions;
using IotMonitor.Data.Entities;
using IotMonitor.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace IotMonitor.Tests;

[TestClass]
public class DeviceWorkerServiceTests
{
    private Mock<ILogger<DeviceWorkerService>> _loggerMock = null!;
    private Mock<IServiceScopeFactory> _scopeFactoryMock = null!;
    private Mock<IServiceScope> _scopeMock = null!;
    private Mock<IServiceProvider> _serviceProviderMock = null!;
    private Mock<IDeviceRepository> _repositoryMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<DeviceWorkerService>>();
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _scopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _repositoryMock = new Mock<IDeviceRepository>();
        
        
        
        var orchestrator = new DeviceOrchestrator(_scopeFactoryMock.Object);

        _scopeFactoryMock.Setup(x => x.CreateScope()).Returns(_scopeMock.Object);
        _scopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IDeviceRepository))).Returns(_repositoryMock.Object);
    }

    [TestMethod]
    public async Task ExecuteAsync_ShouldStartRunnersForDevices()
    {
        
        var devices = new List<DeviceEntity>
        {
            new ModbusDeviceEntity 
            { 
                Id = Guid.NewGuid(), 
                Alias = "AlwaysOn", 
                IpAddress = "127.0.0.1", 
                Port = 1234, 
                DeviceType = new DeviceTypeEntity { Lifecycle = ConnectionLifecycle.AlwaysOn } 
            },
            new UdpDeviceEntity 
            { 
                Id = Guid.NewGuid(), 
                Alias = "UDP", 
                IpAddress = "127.0.0.1", 
                DeviceType = new DeviceTypeEntity { Lifecycle = ConnectionLifecycle.FireAndForget } 
            },
            new HttpDeviceEntity 
            { 
                Id = Guid.NewGuid(), 
                Alias = "HTTP", 
                IpAddress = "127.0.0.1", 
                Port = 80, 
                DeviceType = new DeviceTypeEntity { Lifecycle = ConnectionLifecycle.OnDemand },
                EndpointPath = "/"
            }
        };

        _repositoryMock.Setup(x => x.GetAllDevicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(devices);

        var orchestrator = new DeviceOrchestrator(_scopeFactoryMock.Object);
        var service = new DeviceWorkerService(_loggerMock.Object, _scopeFactoryMock.Object, orchestrator);

        using var cts = new CancellationTokenSource();
        
        
        var task = service.StartAsync(cts.Token);
        
        
        await Task.Delay(100);
        
        await service.StopAsync(CancellationToken.None);
        cts.Cancel();

        
        _repositoryMock.Verify(x => x.GetAllDevicesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }
}
