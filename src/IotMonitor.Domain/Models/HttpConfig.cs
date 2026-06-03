using IotMonitor.Domain.Enums;

namespace IotMonitor.Domain.Models;

/// <summary>
/// HTTP-specific configuration mapped via TPT.
/// </summary>
public sealed class HttpConfig : Device
{
    private HttpConfig()
    {
    }

    /// <summary>
    /// Initializes a new <see cref="HttpConfig"/> instance.
    /// </summary>
    /// <param name="id">Unique device identifier.</param>
    /// <param name="alias">Friendly name shown in the console UI.</param>
    /// <param name="deviceTypeCode">Device type code from catalog.</param>
    /// <param name="ipAddress">Target IP address.</param>
    /// <param name="port">Target network port.</param>
    /// <param name="category">Logical device category.</param>
    /// <param name="endpointPath">HTTP endpoint path.</param>
    public HttpConfig(
        Guid id,
        string alias,
        string deviceTypeCode,
        string ipAddress,
        int port,
        DeviceCategory category,
        string endpointPath)
        : base(id, alias, deviceTypeCode, ipAddress, port, category)
    {
        EndpointPath = endpointPath;
    }

    /// <summary>
    /// Gets the endpoint path for HTTP operations.
    /// </summary>
    public string EndpointPath { get; private set; } = string.Empty;
}
