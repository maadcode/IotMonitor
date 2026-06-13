using IotMonitor.Application.Abstractions;
using IotMonitor.Application.Services;
using IotMonitor.Data;
using IotMonitor.Data.Abstractions;
using IotMonitor.Data.Repositories;
using IotMonitor.Presentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
	options.SingleLine = true;
	options.TimestampFormat = "HH:mm:ss ";
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost;Database=IotMonitor;User Id=sa;Password=DevContainer_Sql_123!;TrustServerCertificate=True";

builder.Services.AddDbContext<IotMonitorDbContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("IotMonitor.Data")));

builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddSingleton<IDeviceOrchestrator, InMemoryDeviceOrchestrator>();
builder.Services.AddSingleton<ConsoleShell>();

using var host = builder.Build();

var shell = host.Services.GetRequiredService<ConsoleShell>();
await shell.RunAsync(CancellationToken.None).ConfigureAwait(false);
