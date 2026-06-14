using IotMonitor.Application.Abstractions;
using IotMonitor.Domain.Enums;
using IotMonitor.Domain.Models;
using Spectre.Console;

namespace IotMonitor.Presentation;

/// <summary>
/// Renders the tactical console and routes user actions.
/// </summary>
public sealed class ConsoleShell(IDeviceOrchestrator orchestrator, IUdpDeviceSender udpSender, IPhotoCaptureService photoCaptureService)
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
                            .AddChoices("Actualizar todo", "Reconectar Sensor", "Enviar Mensaje UDP", "Tomar Foto", "Salir"));

                    switch (option)
                    {
                        case "Actualizar todo":
                            await orchestrator.TestAllConnectionsAsync(cancellationToken).ConfigureAwait(false);
                            break;
                        case "Reconectar Sensor":
                            await HandleReconnectionAsync(cancellationToken).ConfigureAwait(false);
                            break;
                        case "Enviar Mensaje UDP":
                            await HandleSendUdpMessageAsync(cancellationToken).ConfigureAwait(false);
                            break;
                        case "Tomar Foto":
                            await HandleCapturePhotoAsync(cancellationToken).ConfigureAwait(false);
                            break;
                        case "Salir":
                            keepRunning = false;
                            break;
                    }
                }
            });
    }

    private async Task HandleCapturePhotoAsync(CancellationToken cancellationToken)
    {
        var devices = await orchestrator.GetDashboardSnapshotAsync(cancellationToken).ConfigureAwait(false);
        var httpChoices = devices
            .Where(d => d.Lifecycle == ConnectionLifecycle.OnDemand)
            .Select(d => $"{d.Alias} ({d.Id})")
            .ToList();

        if (httpChoices.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay camaras HTTP disponibles.[/]");
            await Task.Delay(2000, cancellationToken);
            return;
        }

        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Selecciona la camara")
                .AddChoices(httpChoices)
                .AddChoices("Cancelar"));

        if (selection == "Cancelar") return;

        var idPart = selection.Split('(')[1].TrimEnd(')');
        if (!Guid.TryParse(idPart, out var deviceId)) return;

        try
        {
            AnsiConsole.MarkupLine("[grey]Capturando foto...[/]");
            var savedPath = await photoCaptureService.CapturePhotoAsync(deviceId, cancellationToken)
                .ConfigureAwait(false);
            AnsiConsole.MarkupLine($"[green]Foto guardada en:[/] {Markup.Escape(savedPath)}");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error al capturar foto: {Markup.Escape(ex.Message)}[/]");
        }

        await Task.Delay(2000, cancellationToken);
    }

    private async Task HandleSendUdpMessageAsync(CancellationToken cancellationToken)
    {
        var devices = await orchestrator.GetDashboardSnapshotAsync(cancellationToken).ConfigureAwait(false);
        var udpChoices = devices
            .Where(d => d.Lifecycle == ConnectionLifecycle.FireAndForget)
            .Select(d => $"{d.Alias} ({d.Id})")
            .ToList();

        if (udpChoices.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay dispositivos UDP disponibles.[/]");
            await Task.Delay(2000, cancellationToken);
            return;
        }

        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Selecciona el dispositivo UDP")
                .AddChoices(udpChoices)
                .AddChoices("Cancelar"));

        if (selection == "Cancelar") return;

        var idPart = selection.Split('(')[1].TrimEnd(')');
        if (!Guid.TryParse(idPart, out var deviceId)) return;

        // AnsiConsole.Ask (TextPrompt) cannot run inside a Live display due to Spectre's
        // exclusivity mode — use raw Console I/O instead.
        AnsiConsole.MarkupLine("[grey]Ingresa el mensaje a enviar:[/] ");
        var message = Console.ReadLine() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(message)) return;

        try
        {
            await udpSender.SendToDeviceAsync(deviceId, message, cancellationToken).ConfigureAwait(false);
            AnsiConsole.MarkupLine($"[green]Mensaje enviado a {Markup.Escape(selection)}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error al enviar mensaje: {Markup.Escape(ex.Message)}[/]");
        }

        await Task.Delay(1500, cancellationToken);
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
            DeviceStatus.Ready => "[blue]Conectado (en espera)[/]",
            _ => "[grey]Desconocido[/]",
        };
    }
}
