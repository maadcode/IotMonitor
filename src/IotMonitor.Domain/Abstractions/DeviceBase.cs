using IotMonitor.Domain.Enums;

namespace IotMonitor.Domain.Abstractions;




public abstract class DeviceBase
{
    
    
    
    
    
    
    
    
    
    protected DeviceBase(
        Guid id,
        string alias,
        string ipAddress,
        int port,
        DeviceCategory category,
        ConnectionLifecycle lifecycle)
    {
        Id = id;
        Alias = string.IsNullOrWhiteSpace(alias)
            ? throw new ArgumentException("Alias is required.", nameof(alias))
            : alias;
        IpAddress = string.IsNullOrWhiteSpace(ipAddress)
            ? throw new ArgumentException("IP address is required.", nameof(ipAddress))
            : ipAddress;
        Port = port;
        Category = category;
        Lifecycle = lifecycle;
    }

    
    
    
    public Guid Id { get; }

    
    
    
    public string Alias { get; }

    
    
    
    public string IpAddress { get; }

    
    
    
    public int Port { get; }

    
    
    
    public DeviceCategory Category { get; }

    
    
    
    public ConnectionLifecycle Lifecycle { get; }

    
    
    
    
    
    public abstract Task<bool> TestConnectionAsync(CancellationToken cancellationToken);
}
