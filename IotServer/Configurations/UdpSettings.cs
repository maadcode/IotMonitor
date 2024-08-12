namespace IotServer.Configurations;

public class UdpSettings
{
    public int UdpServerPort { get; set; }
    public string? NodeRedServerIp { get; set; }
    public int NodeRedServerPort { get; set; }
    public string? Persistencia { get; set; }
    public string? FilePath { get; set; }
    public string? ConnectionString { get; set; }
}