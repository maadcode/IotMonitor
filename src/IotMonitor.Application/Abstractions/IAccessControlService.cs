namespace IotMonitor.Application.Abstractions;




public interface IAccessControlService
{
    
    
    
    
    
    
    
    Task SetAccessStateAsync(Guid deviceId, bool active, CancellationToken cancellationToken);
}
