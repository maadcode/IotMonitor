namespace IotMonitor.Domain.Interfaces;




public interface IUdpMessenger
{
    
    
    
    
    
    
    Task SendMessageAsync(string message, CancellationToken cancellationToken);
}
