namespace IotMonitor.Application.Abstractions;




public interface IPhotoCaptureService
{
    
    
    
    
    
    
    Task<string> CapturePhotoAsync(Guid deviceId, CancellationToken cancellationToken);
}
