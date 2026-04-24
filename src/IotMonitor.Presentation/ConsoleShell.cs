using IotMonitor.Application.Abstractions;
using IotMonitor.Domain.Enums;
using IotMonitor.Domain.Models;
using Spectre.Console;

namespace IotMonitor.Presentation;

/// <summary>
/// Renders the tactical console and routes user actions.
/// </summary>
public sealed class ConsoleShell(IDeviceOrchestrator orchestrator)
{
    /// <summary>
    /// Runs the shell loop until the user exits.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the session.</param>
    /// <returns>A completion task.</returns>
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(orchestrator);

        var keepRunning = true;
        while (keepRunning && !cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[aqua]Console Monitor Devices[/]").LeftJustified());

            var snapshot = await orchestrator.GetDashboardSnapshotAsync(cancellationToken).ConfigureAwait(false);
            RenderDashboard(snapshot);

            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecciona una accion")
                    .AddChoices("Actualizar estado", "Salir"));

            switch (option)
            {
                case "Actualizar estado":
                    _ = await orchestrator.TestAllConnectionsAsync(cancellationToken).ConfigureAwait(false);
                    break;
                case "Salir":
                    keepRunning = false;
                    break;
            }
        }
    }

    private static void RenderDashboard(IReadOnlyCollection<DeviceSnapshot> devices)
    {
        var table = new Table().Border(TableBorder.Rounded).Expand();
        table.AddColumn("Alias");
        table.AddColumn("Categoria");
        table.AddColumn("Ciclo");
        table.AddColumn("Estado");
        table.AddColumn("Ultima conexion");

        foreach (var device in devices.OrderBy(x => x.Category).ThenBy(x => x.Alias))
        {
            table.AddRow(
                device.Alias,
                device.Category.ToString(),
                device.Lifecycle.ToString(),
                FormatStatus(device.Status),
                device.LastConnectionUtc?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A");
        }

        AnsiConsole.Write(table);
    }

    private static string FormatStatus(DeviceStatus status)
    {
        return status switch
        {
            DeviceStatus.Connected => "[green]Conectado[/]",
            DeviceStatus.Disconnected => "[red]Desconectado[/]",
            DeviceStatus.Ready => "[yellow]Ready[/]",
            _ => "[grey]Desconocido[/]",
        };
    }
}
