namespace IotServer.Configurations;

public class TcpSettings
{
    public int TcpServerPort { get; set; }
    public string? Persistencia { get; set; }
    public string? FilePath { get; set; }
    public string? ConnectionString { get; set; }
}