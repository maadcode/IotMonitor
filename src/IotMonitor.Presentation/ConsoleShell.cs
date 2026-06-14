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

        var snapshot = await orchestrator.GetDashboardSnapshotAsync(cancellationToken).ConfigureAwait(false);
        var table = CreateDashboardTable(snapshot);

        await AnsiConsole.Live(table)
            .StartAsync(async ctx =>
            {
                // Subscribe to state changes to update the live table
                orchestrator.StatusChanged += async (s, e) =>
                {
                    var updatedSnapshot = await orchestrator.GetDashboardSnapshotAsync(cancellationToken).ConfigureAwait(false);
                    UpdateDashboardTable(table, updatedSnapshot);
                    ctx.Refresh();
                };

                var keepRunning = true;
                while (keepRunning && !cancellationToken.IsCancellationRequested)
                {
                    ctx.Refresh();

                    var option = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Selecciona una accion")
                            .AddChoices("Actualizar todo", "Reconectar Sensor", "Salir"));

                    switch (option)
                    {
                        case "Actualizar todo":
                            await orchestrator.TestAllConnectionsAsync(cancellationToken).ConfigureAwait(false);
                            break;
                        case "Reconectar Sensor":
                            await HandleReconnectionAsync(cancellationToken).ConfigureAwait(false);
                            break;
                        case "Salir":
                            keepRunning = false;
                            break;
                    }
                }
            });
    }

    private async Task HandleReconnectionAsync(CancellationToken cancellationToken)
    {
        var devices = await orchestrator.GetDashboardSnapshotAsync(cancellationToken).ConfigureAwait(false);
        var sensorChoices = devices
            .Where(d => d.Lifecycle == ConnectionLifecycle.AlwaysOn)
            .Select(d => $"{d.Alias} ({d.Id})")
            .ToList();

        if (sensorChoices.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay sensores Always-On disponibles para reconectar.[/]");
            await Task.Delay(2000, cancellationToken);
            return;
        }

        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Selecciona el sensor a reconectar")
                .AddChoices(sensorChoices)
                .AddChoices("Cancelar"));

        if (selection == "Cancelar") return;

        var idPart = selection.Split('(')[1].TrimEnd(')');
        if (Guid.TryParse(idPart, out var deviceId))
        {
            await orchestrator.ReconnectDeviceAsync(deviceId, cancellationToken).ConfigureAwait(false);
            AnsiConsole.MarkupLine($"[green]Comando de reconexion enviado para {selection}[/]");
            await Task.Delay(1000, cancellationToken);
        }
    }

    private static Table CreateDashboardTable(IReadOnlyCollection<DeviceSnapshot> devices)
    {
        var table = new Table().Border(TableBorder.Rounded).Expand();
        table.Title("[aqua]Console Monitor Devices - Dashboard[/]");
        table.AddColumn("Alias");
        table.AddColumn("Categoria");
        table.AddColumn("Ciclo");
        table.AddColumn("Estado");
        table.AddColumn("Ultima conexion");

        PopulateTable(table, devices);
        return table;
    }

    private static void UpdateDashboardTable(Table table, IReadOnlyCollection<DeviceSnapshot> devices)
    {
        table.Rows.Clear();
        PopulateTable(table, devices);
    }

    private static void PopulateTable(Table table, IReadOnlyCollection<DeviceSnapshot> devices)
    {
        foreach (var device in devices.OrderBy(x => x.Category).ThenBy(x => x.Alias))
        {
            table.AddRow(
                device.Alias,
                device.Category.ToString(),
                device.Lifecycle.ToString(),
                FormatStatus(device.Status),
                device.LastConnectionUtc?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A");
        }
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
