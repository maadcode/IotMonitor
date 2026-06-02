using IotMonitor.Application.Abstractions;
using IotMonitor.Application.Services;
using IotMonitor.Presentation;
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

builder.Services.AddSingleton<IDeviceOrchestrator, InMemoryDeviceOrchestrator>();
builder.Services.AddSingleton<ConsoleShell>();

using var host = builder.Build();

var shell = host.Services.GetRequiredService<ConsoleShell>();
await shell.RunAsync(CancellationToken.None).ConfigureAwait(false);
