using System.Collections.Concurrent;
using IotMonitor.Application.Abstractions;
using IotMonitor.Data.Abstractions;
using IotMonitor.Domain.Enums;
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

        // For now, this still simulates or delegates to the specific workers.
        // In Phase 4, the workers will update the state automatically for Always-On.
        // For On-Demand, we might trigger a one-off test here.
        
        foreach (var deviceId in _deviceStates.Keys)
        {
            // Simple simulation for now, to be replaced by actual protocol calls or worker integration
            await UpdateDeviceStatusInternalAsync(deviceId, DeviceStatus.Ready, cancellationToken);
        }

        return _deviceStates.Values.ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public async Task ReconnectDeviceAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        if (_deviceStates.TryGetValue(deviceId, out var snapshot))
        {
            // Here we would signal the specific worker to restart the socket.
            // For now, we just update the status to show it's happening.
            await UpdateDeviceStatusInternalAsync(deviceId, DeviceStatus.Connected, cancellationToken);
        }
    }

    /// <summary>
    /// Updates the status of a device and fires the StatusChanged event if it changed.
    /// </summary>
    public async Task UpdateDeviceStatusInternalAsync(Guid deviceId, DeviceStatus newStatus, CancellationToken cancellationToken)
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
                var snapshot = new DeviceSnapshot(
                    device.Id,
                    device.Alias,
                    device.CategoryId,
                    device.DeviceType.Lifecycle,
                    DeviceStatus.Unknown,
                    null);
                
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
