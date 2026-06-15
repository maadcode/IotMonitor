namespace IotMonitor.Domain.Interfaces;




public interface ILightController
{
    
    
    
    
    
    
    Task SetColorAsync(string color, CancellationToken cancellationToken);
}
