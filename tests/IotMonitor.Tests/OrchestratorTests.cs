using Moq;
using IotMonitor.Application.Services;
using IotMonitor.Data.Abstractions;
using IotMonitor.Data.Entities;
using IotMonitor.Domain.Enums;
using IotMonitor.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace IotMonitor.Tests;

[TestClass]
public class OrchestratorTests
{
    private Mock<IDeviceRepository> _repositoryMock = null!;
    private Mock<IServiceScopeFactory> _scopeFactoryMock = null!;
    private Mock<IServiceScope> _scopeMock = null!;
    private Mock<IServiceProvider> _serviceProviderMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _repositoryMock = new Mock<IDeviceRepository>();
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _scopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();

        _scopeFactoryMock.Setup(x => x.CreateScope()).Returns(_scopeMock.Object);
        _scopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IDeviceRepository))).Returns(_repositoryMock.Object);
    }

    [TestMethod]
    public async Task GetDashboardSnapshotAsync_ShouldInitializeAndReturnDevices()
    {
        
        var deviceId = Guid.NewGuid();
        var devices = new List<DeviceEntity>
        {
            new TestDeviceEntity
            {
                Id = deviceId,
                Alias = "Test Device",
                CategoryId = DeviceCategory.Lectura,
                DeviceType = new DeviceTypeEntity { Lifecycle = ConnectionLifecycle.AlwaysOn }
            }
        };

        _repositoryMock.Setup(x => x.GetAllDevicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(devices);

        var orchestrator = new DeviceOrchestrator(_scopeFactoryMock.Object);

        
        var snapshot = await orchestrator.GetDashboardSnapshotAsync(CancellationToken.None);

        
        Assert.AreEqual(1, snapshot.Count);
        Assert.AreEqual(deviceId, snapshot.First().Id);
        Assert.AreEqual(DeviceStatus.Unknown, snapshot.First().Status);
        _repositoryMock.Verify(x => x.GetAllDevicesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task UpdateDeviceStatus_ShouldFireEventOnStatusChange()
    {
        
        var deviceId = Guid.NewGuid();
        var devices = new List<DeviceEntity>
        {
            new TestDeviceEntity
            {
                Id = deviceId,
                Alias = "Test Device",
                CategoryId = DeviceCategory.Lectura,
                DeviceType = new DeviceTypeEntity { Lifecycle = ConnectionLifecycle.AlwaysOn }
            }
        };

        _repositoryMock.Setup(x => x.GetAllDevicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(devices);

        var orchestrator = new DeviceOrchestrator(_scopeFactoryMock.Object);
        
        
        await orchestrator.GetDashboardSnapshotAsync(CancellationToken.None);

        DeviceStatusChangedEventArgs? raisedEvent = null;
        orchestrator.StatusChanged += (s, e) => raisedEvent = e;

        
        await orchestrator.UpdateDeviceStatusInternalAsync(deviceId, DeviceStatus.Connected, CancellationToken.None);

        
        Assert.IsNotNull(raisedEvent);
        Assert.AreEqual(deviceId, raisedEvent.DeviceId);
        Assert.AreEqual(DeviceStatus.Unknown, raisedEvent.OldStatus);
        Assert.AreEqual(DeviceStatus.Connected, raisedEvent.NewStatus);
        
        var snapshot = await orchestrator.GetDashboardSnapshotAsync(CancellationToken.None);
        Assert.AreEqual(DeviceStatus.Connected, snapshot.First().Status);
    }

    private class TestDeviceEntity : DeviceEntity { }
}
