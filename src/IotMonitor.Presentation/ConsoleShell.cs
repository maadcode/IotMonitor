using IotMonitor.Application.Abstractions;
using IotMonitor.Domain.Enums;
using IotMonitor.Domain.Interfaces;
using IotMonitor.Domain.Models;
using Spectre.Console;

namespace IotMonitor.Presentation;

/// <summary>
/// Renders the tactical console and routes user actions.
/// </summary>
public sealed class ConsoleShell(
    IDeviceOrchestrator orchestrator, 
    IUdpDeviceSender udpSender, 
    IPhotoCaptureService photoCaptureService,
    IAccessControlService accessControlService,
    ILightControlService lightControlService)
{
    private Table _dashboardTable = null!;

    /// <summary>
    /// Runs the shell loop until the user exits.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the session.</param>
    /// <returns>A completion task.</returns>
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(orchestrator);

        var snapshot = await orchestrator.GetDashboardSnapshotAsync(cancellationToken).ConfigureAwait(false);
        _dashboardTable = CreateDashboardTable(snapshot);

        // Subscribe to state changes to update the table object
        orchestrator.StatusChanged += async (s, e) =>
        {
            var updatedSnapshot = await orchestrator.GetDashboardSnapshotAsync(cancellationToken).ConfigureAwait(false);
            UpdateDashboardTable(_dashboardTable, updatedSnapshot);
        };

        var keepRunning = true;
        
        // Main Loop: We render the dashboard and then show the interactive menu.
        while (keepRunning && !cancellationToken.IsCancellationRequested)
        {
            RenderStaticDashboard();

            var rootOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Menu Principal[/]")
                    .AddChoices("Gestionar Categorias", "Actualizar Todo", "Salir"));

            switch (rootOption)
            {
                case "Gestionar Categorias":
                    await HandleCategoryNavigationAsync(cancellationToken).ConfigureAwait(false);
                    break;
                case "Actualizar Todo":
                    await orchestrator.TestAllConnectionsAsync(cancellationToken).ConfigureAwait(false);
                    AnsiConsole.MarkupLine("[green]Pruebas de conectividad lanzadas.[/]");
                    await Task.Delay(1000, cancellationToken);
                    break;
                case "Salir":
                    keepRunning = false;
                    break;
            }
        }
    }

    private void RenderStaticDashboard()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(_dashboardTable);
        AnsiConsole.WriteLine();
    }

    private async Task HandleCategoryNavigationAsync(CancellationToken cancellationToken)
    {
        var devices = await orchestrator.GetDashboardSnapshotAsync(cancellationToken).ConfigureAwait(false);
        var categories = devices.Select(d => d.Category.ToString()).Distinct().OrderBy(c => c).ToList();

        var categorySelection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Selecciona una [blue]Categoria[/]")
                .AddChoices(categories)
                .AddChoices("Volver"));

        if (categorySelection == "Volver") return;

        if (Enum.TryParse<DeviceCategory>(categorySelection, out var selectedCategory))
        {
            await HandleDeviceNavigationAsync(selectedCategory, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task HandleDeviceNavigationAsync(DeviceCategory category, CancellationToken cancellationToken)
    {
        var devices = await orchestrator.GetDashboardSnapshotAsync(cancellationToken).ConfigureAwait(false);
        var categoryDevices = devices.Where(d => d.Category == category).ToList();

        if (categoryDevices.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay dispositivos en esta categoria.[/]");
            await Task.Delay(1500, cancellationToken);
            return;
        }

        var deviceChoices = categoryDevices
            .Select(d => $"{d.Alias} [grey]({d.Status})[/] | {d.Id}")
            .ToList();

        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Dispositivos en [blue]{category}[/]")
                .AddChoices(deviceChoices)
                .AddChoices("Volver"));

        if (selection == "Volver") return;

        var idPart = selection.Split('|')[1].Trim();
        if (Guid.TryParse(idPart, out var deviceId))
        {
            var device = categoryDevices.First(d => d.Id == deviceId);
            await HandleActionNavigationAsync(device, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task HandleActionNavigationAsync(DeviceSnapshot device, CancellationToken cancellationToken)
    {
        var actions = new List<string>();

        if (device.Capabilities.Contains(nameof(IPhotographic))) actions.Add("Tomar Foto");
        if (device.Capabilities.Contains(nameof(IUdpMessenger))) actions.Add("Enviar Mensaje UDP");
        if (device.Capabilities.Contains(nameof(IAccessController))) actions.Add("Controlar Acceso (Abrir/Cerrar)");
        if (device.Capabilities.Contains(nameof(ILightController))) actions.Add("Cambiar Color Semaforo");
        if (device.Lifecycle == ConnectionLifecycle.AlwaysOn) actions.Add("Reconectar");

        if (actions.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Este dispositivo no tiene acciones tacticas disponibles.[/]");
            await Task.Delay(1500, cancellationToken);
            return;
        }

        var actionSelection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Acciones para [green]{device.Alias}[/]")
                .AddChoices(actions)
                .AddChoices("Volver"));

        if (actionSelection == "Volver") return;

        try
        {
            switch (actionSelection)
            {
                case "Tomar Foto":
                    await ExecuteCapturePhotoAsync(device, cancellationToken);
                    break;
                case "Enviar Mensaje UDP":
                    await ExecuteSendUdpMessageAsync(device, cancellationToken);
                    break;
                case "Controlar Acceso (Abrir/Cerrar)":
                    await ExecuteAccessControlAsync(device, cancellationToken);
                    break;
                case "Cambiar Color Semaforo":
                    await ExecuteLightControlAsync(device, cancellationToken);
                    break;
                case "Reconectar":
                    await orchestrator.ReconnectDeviceAsync(device.Id, cancellationToken);
                    AnsiConsole.MarkupLine("[green]Comando de reconexion enviado.[/]");
                    break;
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error al ejecutar accion: {Markup.Escape(ex.Message)}[/]");
        }

        await Task.Delay(2000, cancellationToken);
    }

    private async Task ExecuteCapturePhotoAsync(DeviceSnapshot device, CancellationToken cancellationToken)
    {
        if (!AnsiConsole.Confirm($"¿Deseas capturar una foto desde [green]{device.Alias}[/]?")) return;

        AnsiConsole.MarkupLine("[grey]Capturando foto...[/]");
        var savedPath = await photoCaptureService.CapturePhotoAsync(device.Id, cancellationToken).ConfigureAwait(false);
        AnsiConsole.MarkupLine($"[green]Foto guardada en:[/] {Markup.Escape(savedPath)}");
    }

    private async Task ExecuteSendUdpMessageAsync(DeviceSnapshot device, CancellationToken cancellationToken)
    {
        AnsiConsole.MarkupLine("[grey]Ingresa el mensaje a enviar:[/] ");
        var message = Console.ReadLine() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(message)) return;

        if (!AnsiConsole.Confirm($"¿Enviar \"{Markup.Escape(message)}\" a [green]{device.Alias}[/]?")) return;

        await udpSender.SendToDeviceAsync(device.Id, message, cancellationToken).ConfigureAwait(false);
        AnsiConsole.MarkupLine($"[green]Mensaje enviado correctamente.[/]");
    }

    private async Task ExecuteAccessControlAsync(DeviceSnapshot device, CancellationToken cancellationToken)
    {
        var state = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Selecciona el estado")
                .AddChoices("Activar/Abrir", "Desactivar/Cerrar", "Cancelar"));

        if (state == "Cancelar") return;

        bool active = state == "Activar/Abrir";
        await accessControlService.SetAccessStateAsync(device.Id, active, cancellationToken).ConfigureAwait(false);
        AnsiConsole.MarkupLine($"[green]Comando de acceso '{state}' enviado.[/]");
    }

    private async Task ExecuteLightControlAsync(DeviceSnapshot device, CancellationToken cancellationToken)
    {
        var color = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Selecciona el color")
                .AddChoices("Rojo", "Amarillo", "Verde", "Cancelar"));

        if (color == "Cancelar") return;

        await lightControlService.SetColorAsync(device.Id, color.ToLowerInvariant(), cancellationToken).ConfigureAwait(false);
        AnsiConsole.MarkupLine($"[green]Color cambiado a {color}.[/]");
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
