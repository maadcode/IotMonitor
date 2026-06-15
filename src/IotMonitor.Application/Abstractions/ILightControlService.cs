namespace IotMonitor.Application.Abstractions;




public interface ILightControlService
{
    
    
    
    
    
    
    
    Task SetColorAsync(Guid deviceId, string color, CancellationToken cancellationToken);
}
