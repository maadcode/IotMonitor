namespace IotMonitor.Domain.Interfaces;




public interface IAccessController
{
    
    
    
    
    
    
    Task SetAccessStateAsync(bool active, CancellationToken cancellationToken);
}
