using IotMonitor.Application.Abstractions;
using IotMonitor.Data.Abstractions;
using IotMonitor.Data.Entities;
using IotMonitor.Infrastructure.Devices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IotMonitor.Infrastructure.Services;

/// <summary>
/// Captures a photo from an HTTP-based device and stores it under the configured base path.
/// Path format: {BasePath}/{yyyyMMdd}/{DeviceId}-{yyyyMMdd}-{HHmmss}.jpg
/// </summary>
public sealed class PhotoCaptureService : IPhotoCaptureService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PhotoCaptureService> _logger;
    private readonly string _basePath;

    public PhotoCaptureService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<PhotoCaptureService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _basePath = configuration["PhotoStorage:BasePath"]
            ?? throw new InvalidOperationException("PhotoStorage:BasePath is not configured.");
    }

    /// <inheritdoc />
    public async Task<string> CapturePhotoAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();

        var entity = await repository.GetDeviceByIdAsync(deviceId, cancellationToken);

        if (entity is not HttpDeviceEntity httpEntity)
        {
            throw new InvalidOperationException(
                $"Device {deviceId} is not an HTTP device or was not found.");
        }

        var device = new HttpDevice(
            httpEntity.Id,
            httpEntity.Alias,
            httpEntity.IpAddress,
            httpEntity.Port,
            httpEntity.CategoryId,
            httpEntity.DeviceType.Lifecycle,
            httpEntity.EndpointPath);

        var now = DateTimeOffset.Now;
        var datePart = now.ToString("yyyyMMdd");
        var timePart = now.ToString("HHmmss");
        var fileName = $"{deviceId}-{datePart}-{timePart}.jpg";
        var directory = Path.Combine(_basePath, datePart);
        var filePath = Path.Combine(directory, fileName);

        Directory.CreateDirectory(directory);

        _logger.LogInformation(
            "[HTTP] Capturing photo from '{Alias}' ({Id}) → {Path}",
            httpEntity.Alias, deviceId, filePath);

        await device.CapturePhotoAsync(filePath, cancellationToken);

        _logger.LogInformation(
            "[HTTP] Photo saved: {Path}", filePath);

        return filePath;
    }
}
