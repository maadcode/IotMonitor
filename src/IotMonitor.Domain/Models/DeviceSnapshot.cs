using IotMonitor.Domain.Enums;

namespace IotMonitor.Domain.Models;











public sealed record DeviceSnapshot(
    Guid Id,
    string Alias,
    DeviceCategory Category,
    ConnectionLifecycle Lifecycle,
    DeviceStatus Status,
    DateTimeOffset? LastConnectionUtc,
    IReadOnlyCollection<string> Capabilities);