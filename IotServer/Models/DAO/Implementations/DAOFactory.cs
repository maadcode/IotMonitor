using IotServer.Models.DAO.Interfaces;

namespace IotServer.Models.DAO.Implementations;

public class DAOFactory
{
    public static IDAO CreateDAO(string daoType, string? filePath = null, string? connectionString = null)
    {
        return daoType switch
        {
            "file" => new LogSensoresFileDAO(filePath),
            "database" => new LogSensoresDbDAO(connectionString),
            _ => throw new ArgumentException("Invalid DAO type")
        };
    }
}