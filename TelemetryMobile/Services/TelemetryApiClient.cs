using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using TelemetryMobile.Models;

namespace TelemetryMobile.Services;

public interface ITelemetryApi
{
    Task<IReadOnlyList<DeviceListItem>> GetDevicesAsync(int customerId, CancellationToken cancellationToken = default);
    Task<TelemetryDevice> GetDeviceAsync(int deviceId, CancellationToken cancellationToken = default);
    Task<EventPage> GetEventsAsync(int deviceId, string? cursor = null, int limit = 20, CancellationToken cancellationToken = default);
}

public sealed class TelemetryApiException(HttpStatusCode status, string? code, string? traceId,
    TimeSpan? retryAfter) : Exception($"Mobile API: {(int)status}, code={code}, traceId={traceId}")
{
    public HttpStatusCode Status { get; } = status;
    public string? Code { get; } = code;
    public string? TraceId { get; } = traceId;
    public TimeSpan? RetryAfter { get; } = retryAfter;
}

public sealed class TelemetryApiClient(HttpClient http) : ITelemetryApi
{
    public static readonly Uri BaseAddress = new("http://api.konstek.de/");
    private readonly SemaphoreSlim requests = new(4);
    private DateTimeOffset retryAt;
    private TelemetryApiException? backoffError;
    private readonly object retryLock = new();

    public async Task<IReadOnlyList<DeviceListItem>> GetDevicesAsync(int customerId, CancellationToken cancellationToken = default) =>
        await GetAsync($"api/mobile/customers/{customerId.ToString(CultureInfo.InvariantCulture)}/devices",
            MobileApiJsonContext.Default.ListDeviceListItem, cancellationToken);

    public Task<TelemetryDevice> GetDeviceAsync(int deviceId, CancellationToken cancellationToken = default) =>
        GetAsync($"api/mobile/devices/{deviceId.ToString(CultureInfo.InvariantCulture)}",
            MobileApiJsonContext.Default.TelemetryDevice, cancellationToken);

    public Task<EventPage> GetEventsAsync(int deviceId, string? cursor = null, int limit = 20, CancellationToken cancellationToken = default)
    {
        if (limit is < 1 or > 100) throw new ArgumentOutOfRangeException(nameof(limit));
        var path = $"api/mobile/devices/{deviceId.ToString(CultureInfo.InvariantCulture)}/events?limit={limit.ToString(CultureInfo.InvariantCulture)}";
        if (cursor is not null) path += "&cursor=" + Uri.EscapeDataString(cursor);
        return GetAsync(path, MobileApiJsonContext.Default.EventPage, cancellationToken);
    }

    private async Task<T> GetAsync<T>(string path, JsonTypeInfo<T> type, CancellationToken cancellationToken)
    {
        await requests.WaitAsync(cancellationToken);
        try
        {
            for (var attempt = 0; ; attempt++)
            {
                lock (retryLock)
                {
                    if (retryAt > DateTimeOffset.UtcNow)
                        throw new TelemetryApiException(backoffError!.Status, backoffError.Code, backoffError.TraceId, retryAt - DateTimeOffset.UtcNow);
                }
                using var request = new HttpRequestMessage(HttpMethod.Get, path);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using var response = await http.SendAsync(request, cancellationToken);
                if (response.StatusCode == HttpStatusCode.InternalServerError && attempt < 2)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1 << attempt), cancellationToken);
                    continue;
                }
                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                if (response.IsSuccessStatusCode)
                    return await JsonSerializer.DeserializeAsync(stream, type, cancellationToken)
                        ?? throw new JsonException("The API returned an empty response.");

                ApiProblem? problem = null;
                try { problem = await JsonSerializer.DeserializeAsync(stream, MobileApiJsonContext.Default.ApiProblem, cancellationToken); }
                catch (JsonException) { /* Proxies may return a non-JSON error page. */ }
                var retry = response.Headers.RetryAfter;
                TimeSpan? delay = retry?.Delta ?? (retry?.Date - DateTimeOffset.UtcNow);
                if (response.StatusCode is HttpStatusCode.TooManyRequests or HttpStatusCode.ServiceUnavailable)
                {
                    delay = delay > TimeSpan.Zero ? delay : TimeSpan.FromSeconds(30);
                    lock (retryLock)
                    {
                        retryAt = DateTimeOffset.UtcNow + delay.Value;
                        backoffError = new TelemetryApiException(response.StatusCode, problem?.Code, problem?.TraceId, delay);
                    }
                }
                throw new TelemetryApiException(response.StatusCode, problem?.Code, problem?.TraceId, delay);
            }
        }
        finally { requests.Release(); }
    }
}
