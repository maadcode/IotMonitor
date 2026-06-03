using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IotMonitor.Data;

/// <summary>
/// Design-time factory for EF Core migrations.
/// </summary>
public sealed class IotMonitorDbContextFactory : IDesignTimeDbContextFactory<IotMonitorDbContext>
{
    /// <inheritdoc />
    public IotMonitorDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IotMonitorDbContext>();
        var password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
        if (string.IsNullOrWhiteSpace(password))
        {
            password = "DevContainer_Sql_123!";
        }

        var connectionBuilder = new SqlConnectionStringBuilder
        {
            DataSource = "localhost,1433",
            InitialCatalog = "IotMonitor",
            UserID = "sa",
            Password = password,
            TrustServerCertificate = true,
        };

        optionsBuilder.UseSqlServer(connectionBuilder.ConnectionString);

        return new IotMonitorDbContext(optionsBuilder.Options);
    }
}
