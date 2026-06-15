using IotMonitor.Application.Abstractions;
using IotMonitor.Data.Abstractions;
using IotMonitor.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IotMonitor.Application.Services;






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
        _logger.LogInformation("Device Worker Service starting.");

        if (_orchestrator is not DeviceOrchestrator concreteOrchestrator)
        {
            _logger.LogError("Orchestrator is not a DeviceOrchestrator. Worker cannot start.");
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();

        var allDevices = (await repository.GetAllDevicesAsync(stoppingToken)).ToList();

        var alwaysOnDevices = allDevices
            .Where(d => d.DeviceType.Lifecycle == ConnectionLifecycle.AlwaysOn)
            .ToList();

        var udpDevices = allDevices
            .Where(d => d.DeviceType.Lifecycle == ConnectionLifecycle.FireAndForget)
            .ToList();

        var httpDevices = allDevices
            .Where(d => d.DeviceType.Lifecycle == ConnectionLifecycle.OnDemand)
            .ToList();

        if (alwaysOnDevices.Count == 0 && udpDevices.Count == 0 && httpDevices.Count == 0)
        {
            _logger.LogWarning("No AlwaysOn, UDP or HTTP devices found. Worker is idle.");
            return;
        }

        
        await concreteOrchestrator.GetDashboardSnapshotAsync(stoppingToken);

        var allTasks = new List<Task>();

        if (alwaysOnDevices.Count > 0)
        {
            _logger.LogInformation("Starting {Count} AlwaysOn device runner(s).", alwaysOnDevices.Count);

            allTasks.AddRange(alwaysOnDevices.Select(device =>
            {
                var runner = new AlwaysOnDeviceRunner(
                    device.Id,
                    device.IpAddress,
                    device.Port,
                    concreteOrchestrator,
                    _logger);

                return runner.RunAsync(stoppingToken);
            }));
        }

        if (udpDevices.Count > 0)
        {
            _logger.LogInformation("Starting {Count} UDP ping runner(s).", udpDevices.Count);

            allTasks.AddRange(udpDevices.Select(device =>
            {
                var runner = new UdpPingRunner(
                    device.Id,
                    device.IpAddress,
                    concreteOrchestrator,
                    _logger);

                return runner.RunAsync(stoppingToken);
            }));
        }

        if (httpDevices.Count > 0)
        {
            _logger.LogInformation("Starting {Count} HTTP ping runner(s).", httpDevices.Count);

            allTasks.AddRange(httpDevices.Select(device =>
            {
                var runner = new HttpPingRunner(
                    device.Id,
                    device.IpAddress,
                    device.Port,
                    concreteOrchestrator,
                    _logger);

                return runner.RunAsync(stoppingToken);
            }));
        }

        await Task.WhenAll(allTasks);

        _logger.LogInformation("Device Worker Service stopped.");
    }
}
