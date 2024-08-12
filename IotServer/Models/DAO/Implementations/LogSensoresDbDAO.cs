using IotServer.Models.DAO.Interfaces;
using System.Data.SqlClient;

namespace IotServer.Models.DAO.Implementations;

public class LogSensoresDbDAO : IDAO
{
    private readonly string _connectionString;

    public LogSensoresDbDAO(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void SaveMessage(MessageModel message)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "INSERT INTO LogSensores (ReceivedDate, Content, Protocol) VALUES (@ReceivedDate, @Content, @Protocol)";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ReceivedDate", message.Timestamp);
                command.Parameters.AddWithValue("@Content", message.Content);
                command.Parameters.AddWithValue("@Protocol", message.Protocol.ToString());
                command.ExecuteNonQuery();
            }
        }
    }
}