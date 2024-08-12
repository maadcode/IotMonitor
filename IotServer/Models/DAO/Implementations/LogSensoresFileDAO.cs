using IotServer.Models.DAO.Interfaces;

namespace IotServer.Models.DAO.Implementations;

public class LogSensoresFileDAO : IDAO
{
    private readonly string _filePath;
    public LogSensoresFileDAO(string filePath)
    {
        _filePath = filePath;
    }

    public void SaveMessage(MessageModel message)
    {
        using (StreamWriter writer = new StreamWriter(_filePath, true))
        {
            writer.WriteLine($"{message.Timestamp}: {message.Content}");
        }
    }
}