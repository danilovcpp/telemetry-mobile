using System.Text.Json;
using System.Text.Json.Serialization;

namespace TelemetryMobile.Models;

public sealed record DeviceListItem(int Id, string DeviceCode, string Name);
public sealed record TelemetryDevice(int Id, string DeviceCode, string Name, int CustomerId,
    DateTimeOffset? LastEventAtUtc, int? LastStatusCode);
public sealed record TelemetryEvent(Guid Id, int StatusCode, DateTimeOffset OccurredAtUtc,
    DateTimeOffset ReceivedAtUtc, JsonElement? Payload);
public sealed record EventPage(IReadOnlyList<TelemetryEvent> Items, string? NextCursor, bool HasMore);
public sealed record DeviceEvent(DeviceListItem Device, TelemetryEvent Event);
public sealed record ApiProblem(string? Code, string? TraceId);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(List<DeviceListItem>))]
[JsonSerializable(typeof(TelemetryDevice))]
[JsonSerializable(typeof(EventPage))]
[JsonSerializable(typeof(ApiProblem))]
internal partial class MobileApiJsonContext : JsonSerializerContext;
