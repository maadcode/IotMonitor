using IotMonitor.Domain.Models;

namespace IotMonitor.Application.Abstractions;




public interface IDeviceOrchestrator
{
    
    
    
    event EventHandler<DeviceStatusChangedEventArgs> StatusChanged;

    
    
    
    
    
    Task<IReadOnlyCollection<DeviceSnapshot>> GetDashboardSnapshotAsync(CancellationToken cancellationToken);

    
    
    
    
    
    Task<IReadOnlyCollection<DeviceSnapshot>> TestAllConnectionsAsync(CancellationToken cancellationToken);

    
    
    
    
    
    
    Task ReconnectDeviceAsync(Guid deviceId, CancellationToken cancellationToken);
}
