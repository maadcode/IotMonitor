namespace IotMonitor.Domain.Interfaces;




public interface IPhotographic
{
    
    
    
    
    
    
    Task<string> CapturePhotoAsync(string targetFilePath, CancellationToken cancellationToken);
}
