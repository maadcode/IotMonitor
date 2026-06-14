using IotMonitor.Application.Abstractions;
using IotMonitor.Data.Abstractions;
using IotMonitor.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IotMonitor.Application.Services;

/// <summary>
/// Background service that manages Always-On device workers and polling.
/// </summary>
public sealed class DeviceWorkerService : BackgroundService
{
    private readonly ILogger<DeviceWorkerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDeviceOrchestrator _orchestrator;

    public DeviceWorkerService(
        ILogger<DeviceWorkerService> logger,
        IServiceScopeFactory scopeFactory,
        IDeviceOrchestrator orchestrator)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _orchestrator = orchestrator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Device Worker Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);

            try
            {
                await PollAlwaysOnDevicesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during device polling.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }

        _logger.LogInformation("Device Worker Service is stopping.");
    }

    private async Task PollAlwaysOnDevicesAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
        
        var allDevices = await repository.GetAllDevicesAsync(ct);
        var alwaysOnDevices = allDevices.Where(d => d.DeviceType.Lifecycle == ConnectionLifecycle.AlwaysOn);

        foreach (var device in alwaysOnDevices)
        {
            // In a real implementation, we would check if a specific worker for this device exists.
            // For now, we simulate the connectivity test.
            
            // To update the orchestrator, we need to cast or have a specific method.
            if (_orchestrator is DeviceOrchestrator concreteOrchestrator)
            {
                // Simulate a random status for Always-On devices for now
                var random = new Random();
                var status = random.Next(0, 10) > 2 ? DeviceStatus.Connected : DeviceStatus.Disconnected;
                
                await concreteOrchestrator.UpdateDeviceStatusInternalAsync(device.Id, status, ct);
            }
        }
    }
}
