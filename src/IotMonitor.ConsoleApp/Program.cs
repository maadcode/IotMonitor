using IotMonitor.Application.Abstractions;
using IotMonitor.Application.Services;
using IotMonitor.Data;
using IotMonitor.Data.Abstractions;
using IotMonitor.Data.Repositories;
using IotMonitor.Infrastructure.Services;
using IotMonitor.Presentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
Console.WriteLine($"Entorno actual: {builder.Environment.EnvironmentName}");

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss ";
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<IotMonitorDbContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("IotMonitor.Data")));

builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddSingleton<IUdpDeviceSender, UdpDeviceSender>();
builder.Services.AddSingleton<IDeviceOrchestrator, DeviceOrchestrator>();
builder.Services.AddSingleton<IPhotoCaptureService, PhotoCaptureService>();
builder.Services.AddSingleton<IAccessControlService, AccessControlService>();
builder.Services.AddSingleton<ILightControlService, LightControlService>();
builder.Services.AddHostedService<DeviceWorkerService>();
builder.Services.AddSingleton<ConsoleShell>();

using var host = builder.Build();

// Start all IHostedServices (including DeviceWorkerService / TCP runners)
await host.StartAsync();

var shell = host.Services.GetRequiredService<ConsoleShell>();
await shell.RunAsync(CancellationToken.None).ConfigureAwait(false);

// Gracefully stop all background services when the user exits the shell
await host.StopAsync();
