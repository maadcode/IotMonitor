using IotMonitor.Data;
using IotMonitor.Data.Entities;
using IotMonitor.Data.Repositories;
using IotMonitor.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace IotMonitor.Tests;

[TestClass]
public class DeviceRepositoryTests
{
    private IotMonitorDbContext _context = null!;
    private DeviceRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<IotMonitorDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new IotMonitorDbContext(options);
        _repository = new DeviceRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public async Task GetAllDevicesAsync_ShouldReturnAllDevices()
    {
        
        var type = new DeviceTypeEntity { Code = "TEST", Name = "Test Type", Lifecycle = ConnectionLifecycle.AlwaysOn };
        _context.DeviceTypes.Add(type);
        _context.Devices.Add(new HttpDeviceEntity { Alias = "Device 1", DeviceType = type, DeviceTypeCode = "TEST", CategoryId = DeviceCategory.Lectura, IpAddress = "127.0.0.1", EndpointPath = "/" });
        _context.Devices.Add(new HttpDeviceEntity { Alias = "Device 2", DeviceType = type, DeviceTypeCode = "TEST", CategoryId = DeviceCategory.Seguridad, IpAddress = "127.0.0.1", EndpointPath = "/" });
        await _context.SaveChangesAsync();

        
        var result = await _repository.GetAllDevicesAsync(CancellationToken.None);

        
        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public async Task GetDeviceByIdAsync_ShouldReturnCorrectDevice()
    {
        
        var deviceId = Guid.NewGuid();
        var type = new DeviceTypeEntity { Code = "TEST", Name = "Test Type", Lifecycle = ConnectionLifecycle.AlwaysOn };
        _context.DeviceTypes.Add(type);
        _context.Devices.Add(new HttpDeviceEntity { Id = deviceId, Alias = "Target Device", DeviceType = type, DeviceTypeCode = "TEST", CategoryId = DeviceCategory.Lectura, IpAddress = "127.0.0.1", EndpointPath = "/" });
        await _context.SaveChangesAsync();

        
        var result = await _repository.GetDeviceByIdAsync(deviceId, CancellationToken.None);

        
        Assert.IsNotNull(result);
        Assert.AreEqual("Target Device", result.Alias);
        Assert.AreEqual(deviceId, result.Id);
    }

    [TestMethod]
    public async Task GetDeviceTypesAsync_ShouldReturnAllTypes()
    {
        
        _context.DeviceTypes.Add(new DeviceTypeEntity { Code = "T1", Name = "Type 1", Lifecycle = ConnectionLifecycle.AlwaysOn });
        _context.DeviceTypes.Add(new DeviceTypeEntity { Code = "T2", Name = "Type 2", Lifecycle = ConnectionLifecycle.OnDemand });
        await _context.SaveChangesAsync();

        
        var result = await _repository.GetDeviceTypesAsync(CancellationToken.None);

        
        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public async Task AddDeviceAsync_ShouldAddDeviceToContext()
    {
        
        var type = new DeviceTypeEntity { Code = "TEST", Name = "Test Type", Lifecycle = ConnectionLifecycle.AlwaysOn };
        _context.DeviceTypes.Add(type);
        await _context.SaveChangesAsync();
        
        var device = new HttpDeviceEntity { Alias = "New Device", DeviceType = type, DeviceTypeCode = "TEST", CategoryId = DeviceCategory.Lectura, IpAddress = "127.0.0.1", EndpointPath = "/" };

        
        await _repository.AddDeviceAsync(device, CancellationToken.None);
        await _repository.SaveChangesAsync(CancellationToken.None);

        
        var savedDevice = await _context.Devices.FirstOrDefaultAsync(d => d.Alias == "New Device");
        Assert.IsNotNull(savedDevice);
    }
}
