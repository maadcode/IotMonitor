using IotMonitor.Domain.Abstractions;
using IotMonitor.Domain.Enums;
using IotMonitor.Domain.Interfaces;

namespace IotMonitor.Infrastructure.Devices;




public sealed class HttpDevice : DeviceBase, IPhotographic
{
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(10) };
    private readonly string _endpointPath;

    public HttpDevice(
        Guid id,
        string alias,
        string ipAddress,
        int port,
        DeviceCategory category,
        ConnectionLifecycle lifecycle,
        string endpointPath)
        : base(id, alias, ipAddress, port, category, lifecycle)
    {
        _endpointPath = endpointPath.StartsWith('/') ? endpointPath : $"/{endpointPath}";
    }

    private string BaseUrl => $"http://{IpAddress}:{Port}";

    
    public override async Task<bool> TestConnectionAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(2));

            var request = new HttpRequestMessage(HttpMethod.Head, BaseUrl);
            var response = await _httpClient.SendAsync(request, cts.Token);
            
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    
    public async Task<string> CapturePhotoAsync(string targetFilePath, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}{_endpointPath}", cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var fileStream = File.Create(targetFilePath);
        await stream.CopyToAsync(fileStream, cancellationToken);

        return targetFilePath;
    }
}
