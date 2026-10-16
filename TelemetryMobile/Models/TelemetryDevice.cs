namespace TelemetryMobile.Models;

public enum DeviceStatus { Online, Offline, Error }
public enum EventSeverity { Information, Warning, Alarm }

public sealed record DeviceMetric(string NameKey, string Value, string Icon, string ColorKey, float[] Samples);

public sealed record TelemetryDevice(string Id, string Name, string Address, string Icon,
    DeviceStatus Status, string Model, string SoftwareVersion, string LastContact,
    IReadOnlyList<DeviceMetric> Metrics);

public sealed record TelemetryEvent(string DeviceId, string TitleKey, string Time,
    EventSeverity Severity, bool IsRecovery = false);
