using System.Collections.Concurrent;
using IotMonitor.Application.Abstractions;
using IotMonitor.Data.Abstractions;
using IotMonitor.Data.Entities;
using IotMonitor.Domain.Enums;
using IotMonitor.Domain.Interfaces;
using IotMonitor.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace IotMonitor.Application.Services;

/// <summary>
/// Stateful orchestrator that manages the runtime status of all devices.
/// </summary>
public sealed class DeviceOrchestrator : IDeviceOrchestrator
{
    private readonly ConcurrentDictionary<Guid, DeviceSnapshot> _deviceStates = new();
    private readonly IServiceScopeFactory _scopeFactory;
    private bool _isInitialized;
    private readonly SemaphoreSlim _initializationSemaphore = new(1, 1);

    public event EventHandler<DeviceStatusChangedEventArgs>? StatusChanged;

    public DeviceOrchestrator(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DeviceSnapshot>> GetDashboardSnapshotAsync(CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);
        return _deviceStates.Values.ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DeviceSnapshot>> TestAllConnectionsAsync(CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);

        // AlwaysOn devices are maintained by DeviceWorkerService runners in real time.
        // This method just returns the current snapshot so the console can refresh.
        return _deviceStates.Values.ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public Task ReconnectDeviceAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        // AlwaysOnDeviceRunner reconnects automatically after any connection drop.
        // Setting Unknown signals the UI that a reconnect is in progress.
        return UpdateDeviceStatusInternalAsync(deviceId, DeviceStatus.Unknown, cancellationToken);
    }

    /// <summary>
    /// Updates the status of a device and fires the StatusChanged event if it changed.
    /// </summary>
    public Task UpdateDeviceStatusInternalAsync(Guid deviceId, DeviceStatus newStatus, CancellationToken cancellationToken)
    {
        if (_deviceStates.TryGetValue(deviceId, out var oldSnapshot))
        {
            if (oldSnapshot.Status != newStatus)
            {
                var newSnapshot = oldSnapshot with
                {
                    Status = newStatus,
                    LastConnectionUtc = newStatus is DeviceStatus.Connected or DeviceStatus.Ready
                        ? DateTimeOffset.UtcNow
                        : oldSnapshot.LastConnectionUtc
                };

                if (_deviceStates.TryUpdate(deviceId, newSnapshot, oldSnapshot))
                {
                    StatusChanged?.Invoke(this, new DeviceStatusChangedEventArgs(deviceId, oldSnapshot.Status, newStatus));
                }
            }
        }

        return Task.CompletedTask;
    }

    private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        if (_isInitialized) return;

        await _initializationSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_isInitialized) return;

            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
            var devices = await repository.GetAllDevicesAsync(cancellationToken);

            foreach (var device in devices)
            {
                var capabilities = new List<string>();
                if (device is HttpDeviceEntity) capabilities.Add(nameof(IPhotographic));
                if (device is UdpDeviceEntity) capabilities.Add(nameof(IUdpMessenger));
                if (device is ModbusDeviceEntity)
                {
                    capabilities.Add(nameof(IAccessController));
                    capabilities.Add(nameof(ILightController));
                }

                var snapshot = new DeviceSnapshot(
                    device.Id,
                    device.Alias,
                    device.CategoryId,
                    device.DeviceType.Lifecycle,
                    DeviceStatus.Unknown,
                    null,
                    capabilities.AsReadOnly());

                _deviceStates.TryAdd(device.Id, snapshot);
            }

            _isInitialized = true;
        }
        finally
        {
            _initializationSemaphore.Release();
        }
    }
}
