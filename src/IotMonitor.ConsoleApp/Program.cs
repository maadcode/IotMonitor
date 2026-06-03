using IotMonitor.Application.Abstractions;
using IotMonitor.Application.Services;
using IotMonitor.Data;
using IotMonitor.Presentation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
	options.SingleLine = true;
	options.TimestampFormat = "HH:mm:ss ";
});

var connectionString = builder.Configuration.GetConnectionString("IotMonitor");
if (string.IsNullOrWhiteSpace(connectionString))
{
	var password = builder.Configuration["MSSQL_SA_PASSWORD"];
	if (string.IsNullOrWhiteSpace(password))
	{
		password = "DevContainer_Sql_123!";
	}

	var connectionBuilder = new SqlConnectionStringBuilder
	{
		DataSource = "sqlserver,1433",
		InitialCatalog = "IotMonitor",
		UserID = "sa",
		Password = password,
		TrustServerCertificate = true,
	};

	connectionString = connectionBuilder.ConnectionString;
}

builder.Services.AddPersistence(connectionString);
builder.Services.AddSingleton<IDeviceOrchestrator, InMemoryDeviceOrchestrator>();
builder.Services.AddSingleton<ConsoleShell>();

using var host = builder.Build();

await host.Services.ApplyMigrationsAsync().ConfigureAwait(false);

var shell = host.Services.GetRequiredService<ConsoleShell>();
await shell.RunAsync(CancellationToken.None).ConfigureAwait(false);
