namespace IotMonitor.Application.Abstractions;




public interface IUdpDeviceSender
{
    
    
    
    
    Task SendToDeviceAsync(Guid deviceId, string message, CancellationToken cancellationToken);
}
