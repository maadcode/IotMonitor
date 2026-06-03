using IotMonitor.Application.Abstractions;
using IotMonitor.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IotMonitor.Data;

/// <summary>
/// Dependency injection helpers for persistence.
/// </summary>
public static class PersistenceExtensions
{
    /// <summary>
    /// Registers EF Core persistence services.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="connectionString">SQL Server connection string.</param>
    /// <returns>Updated service collection.</returns>
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<IotMonitorDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IDeviceTypeRepository, DeviceTypeRepository>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IModbusConfigRepository, ModbusConfigRepository>();
        services.AddScoped<IHttpConfigRepository, HttpConfigRepository>();

        return services;
    }

    /// <summary>
    /// Applies pending migrations at startup.
    /// </summary>
    /// <param name="serviceProvider">Root service provider.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static async Task ApplyMigrationsAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IotMonitorDbContext>();
        await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
    }
}
