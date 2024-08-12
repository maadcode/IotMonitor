namespace IotServer.Models;

public class MessageModel
{
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public Protocol Protocol { get; set; }
}

public enum Protocol
{
    TCP,
    UDP,
}