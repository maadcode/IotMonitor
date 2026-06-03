using IotMonitor.Data;
using IotMonitor.Domain.Enums;
using IotMonitor.Domain.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace IotMonitor.Tests;

[TestClass]
public sealed class PersistenceTests
{
    [TestMethod]
    public async Task SeededDeviceTypesAreAvailableAsync()
    {
        await using var testContext = await SqliteTestContext.CreateAsync();
        var types = await testContext.Context.DeviceTypes.AsNoTracking().ToListAsync();

        Assert.IsTrue(types.Any(type => type.Code == "SENSOR_MODBUS"));
        Assert.IsTrue(types.Any(type => type.Code == "CAMARA_HTTP"));
    }

    [TestMethod]
    public async Task ModbusConfigUsesTptMappingAsync()
    {
        await using var testContext = await SqliteTestContext.CreateAsync();
        var device = new ModbusConfig(
            Guid.NewGuid(),
            "Sensor Patio",
            "SENSOR_MODBUS",
            "192.168.1.10",
            502,
            DeviceCategory.Lectura,
            1,
            100);

        testContext.Context.ModbusConfigs.Add(device);
        await testContext.Context.SaveChangesAsync();

        var loaded = await testContext.Context.Devices.OfType<ModbusConfig>().SingleAsync();

        Assert.AreEqual(device.UnitId, loaded.UnitId);
        Assert.AreEqual(device.PrimaryCoil, loaded.PrimaryCoil);
    }

    private sealed class SqliteTestContext : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;

        private SqliteTestContext(SqliteConnection connection, IotMonitorDbContext context)
        {
            _connection = connection;
            Context = context;
        }

        public IotMonitorDbContext Context { get; }

        public static async Task<SqliteTestContext> CreateAsync()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<IotMonitorDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new IotMonitorDbContext(options);
            await context.Database.EnsureCreatedAsync();

            return new SqliteTestContext(connection, context);
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
