using IotMonitor.Domain.Enums;

namespace IotMonitor.Domain.Models;

/// <summary>
/// Immutable dashboard projection for a device.
/// </summary>
/// <param name="Id">Unique device identifier.</param>
/// <param name="Alias">Display alias in the console.</param>
/// <param name="Category">Business category used for navigation.</param>
/// <param name="Lifecycle">Communication lifecycle pattern.</param>
/// <param name="Status">Current runtime health status.</param>
/// <param name="LastConnectionUtc">Last successful connection timestamp in UTC.</param>
/// <param name="Capabilities">List of supported interface/capability names.</param>
public sealed record DeviceSnapshot(
    Guid Id,
    string Alias,
    DeviceCategory Category,
    ConnectionLifecycle Lifecycle,
    DeviceStatus Status,
    DateTimeOffset? LastConnectionUtc,
    IReadOnlyCollection<string> Capabilities);