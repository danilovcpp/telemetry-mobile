using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CommunityToolkit.Mvvm.Input;
using TelemetryMobile.Models;
using TelemetryMobile.PageModels;
using TelemetryMobile.Services;

var assertions = 0;
void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
    assertions++;
}
HttpClient Http(HttpMessageHandler handler) => new(handler) { BaseAddress = TelemetryApiClient.BaseAddress, Timeout = TimeSpan.FromSeconds(20) };

if (args.Contains("--live"))
{
    using var http = Http(new HttpClientHandler());
    var live = new TelemetryApiClient(http);
    var first = await live.GetDevicesAsync(1);
    var tenth = await live.GetDevicesAsync(10);
    Check(first.Count > 0 && tenth.Count > 0 && !first.Select(d => d.Id).Intersect(tenth.Select(d => d.Id)).Any(), "Live customers must have separate device lists.");
    foreach (var device in new[] { first[0], tenth[0] })
    {
        var details = await live.GetDeviceAsync(device.Id);
        Check(details.Id == device.Id && details.DeviceCode == device.DeviceCode, "Live detail ID and code.");
        var page = await live.GetEventsAsync(device.Id, limit: 2);
        Check(page.Items.Count <= 2, "Live event page limit.");
        if (page.HasMore)
        {
            var next = await live.GetEventsAsync(device.Id, page.NextCursor, 2);
            Check(!page.Items.Select(e => e.Id).Intersect(next.Items.Select(e => e.Id)).Any(), "Live opaque cursor must not repeat events.");
        }
    }
    try { await live.GetDevicesAsync(999999); throw new Exception("Expected missing customer."); }
    catch (TelemetryApiException e) { Check(e.Status == HttpStatusCode.NotFound && e.Code == "customer.not_found" && e.TraceId is not null, "Live Problem Details."); }
    Console.WriteLine($"Passed {assertions} live API checks against {TelemetryApiClient.BaseAddress}");
    return;
}

var backend = new FakeBackend();
using var client = Http(backend);
var api = new TelemetryApiClient(client);
var session = new CustomerSession();
session.SignOut();
var navigation = new DemoNavigationService();
var login = new LoginPageModel(session, api, navigation);
foreach (var invalid in new[] { "", "test", "0", "-1", "1.5", "2147483648" })
{
    login.CustomerId = invalid;
    await login.SignInCommand.ExecuteAsync(null);
    Check(login.CustomerIdInvalid && session.CustomerId is null, "Invalid customer IDs must not create a session.");
}
Check(backend.Paths.Count == 0, "Invalid IDs must not call the API.");
login.CustomerId = "999";
await login.SignInCommand.ExecuteAsync(null);
Check(login.ErrorMessage == "CustomerNotFound" && session.CustomerId is null && navigation.SignInCount == 0, "Missing customer must not persist or navigate.");
login.CustomerId = " 1 ";
login.Password = "any arbitrary password";
await login.SignInCommand.ExecuteAsync(null);
Check(session.CustomerId == 1 && navigation.SignInCount == 1 && login.Password == "", "Any password accepted; ID saved; password cleared.");
Check(new CustomerSession().CustomerId == 1, "Customer ID survives session reconstruction.");
login.Password = "";
await login.SignInCommand.ExecuteAsync(null);
Check(navigation.SignInCount == 2, "An empty password is accepted in the MVP too.");

var devices = new DevicesPageModel(api, session, navigation);
var requestsBefore = backend.Paths.Count;
await devices.RefreshAsync();
Check(devices.Devices.Count == 3 && devices.DeviceCount == "3" && backend.Paths.Count == requestsBefore + 1, "Device list loads with a single request, without per-device detail requests.");
devices.SearchText = "CODE-2";
Check(devices.Devices.Count == 1 && devices.Devices[0].Device.Id == 2, "Search real device codes without case sensitivity.");
devices.SearchText = "sensor 1";
Check(devices.Devices.Count == 1, "Search real names without case sensitivity.");
await ((IAsyncRelayCommand)devices.Devices[0].OpenCommand).ExecuteAsync(null);
Check(navigation.LastDeviceId == 1, "Navigate with numeric internal device ID.");
backend.FailNext = true;
await devices.RefreshAsync();
Check(devices.HasError && !devices.IsBusy && devices.Devices.Count == 1, "A failed refresh keeps visible data and exposes a retryable error.");
await devices.RefreshAsync();
Check(!devices.HasError, "Retry clears the error.");

var detailsModel = new DeviceDetailsPageModel(api, session, navigation);
detailsModel.SetDevice(1);
await detailsModel.RefreshAsync();
Check(detailsModel.Device?.Device.Id == 1 && detailsModel.LastStatus == "Status 100000" && detailsModel.RecentEvents.Count == 3, "Real device details and recent history.");
await detailsModel.AllEventsCommand.ExecuteAsync(null);
Check(navigation.EventsDeviceId == 1, "All events keeps device scope.");
detailsModel.SetDevice(3);
await detailsModel.RefreshAsync();
Check(detailsModel.LastContact == "NoEvents" && detailsModel.LastStatus == "NoEvents" && detailsModel.RecentEvents.Count == 0, "Device without history handles explicit null status and time.");
detailsModel.SetDevice(999);
await detailsModel.RefreshAsync();
Check(navigation.CloseDeviceCount == 1, "Deleted device closes its page.");

var events = new EventsPageModel(api, session, navigation);
await events.RefreshAsync();
Check(events.Events.Count == 20 && events.HasMore, "Global history loads a bounded first page.");
while (events.HasMore) await events.LoadMoreCommand.ExecuteAsync(null);
var expected = backend.History.Where(p => p.Key != 46).SelectMany(p => p.Value).OrderByDescending(e => e.OccurredAtUtc).ThenByDescending(e => e.Id).Select(e => e.Id).ToArray();
Check(events.Events.Select(e => e.Id).SequenceEqual(expected), "Merged histories keep global order, including timestamps tied across devices, without gaps or duplicates.");
Check(events.Events.Count == events.Events.Select(e => e.Id).Distinct().Count(), "Event IDs are unique across page boundaries.");
await events.RefreshAsync();
Check(events.Events.Count == 20, "Refresh replaces loaded history instead of appending.");
backend.InvalidCursorOnce = true;
await events.LoadMoreCommand.ExecuteAsync(null);
Check(!events.HasError && events.Events.Count == 20, "Invalid cursor restarts from the first page.");
events.SetDevice(2);
await events.RefreshAsync();
Check(events.IsDeviceScope && events.DeviceName == "Sensor 2" && events.Events.Count == 7 && events.Events.All(e => e.Description == "Sensor 2"), "Scoped history only contains the selected customer's device.");
events.SetDevice(46);
await events.RefreshAsync();
Check(events.Events.Count == 0, "A different customer's device cannot enter the selected feed.");
events.SetDevice(null);
await events.RefreshAsync();
backend.FailNext = true;
await events.LoadMoreCommand.ExecuteAsync(null);
Check(events.HasError && events.Events.Count == 20, "Failed pagination retains the existing page.");
while (events.HasMore) await events.LoadMoreCommand.ExecuteAsync(null);
Check(events.Events.Select(e => e.Id).SequenceEqual(expected), "Retry after a partial merged page loses no buffered events.");

session.SignOut();
Check(new CustomerSession().CustomerId is null, "Sign out removes the persisted customer ID.");
session.SignIn(10);
var otherDevices = new DevicesPageModel(api, session, navigation);
await otherDevices.RefreshAsync();
Check(otherDevices.Devices.Count == 1 && otherDevices.Devices[0].Device.Id == 46, "A new customer gets its own devices.");
var otherEvents = new EventsPageModel(api, session, navigation);
await otherEvents.RefreshAsync();
Check(otherEvents.Events.Count == 1 && otherEvents.Events[0].Description == "Customer ten", "A new customer gets its own events.");

using var datesHttp = Http(new ScriptedHandler((_, _) => new HttpResponseMessage(HttpStatusCode.OK)
{
    Content = new StringContent("""
    {"items":[
      {"id":"00000000-0000-0000-0000-000000000001","statusCode":200,"occurredAtUtc":"2026-10-16T10:05:00Z","receivedAtUtc":"2026-10-16T15:36:32.18105Z","payload":{"nested":{"a":[1,true]},"voltage":3.3}},
      {"id":"00000000-0000-0000-0000-000000000002","statusCode":100001,"occurredAtUtc":"2026-10-16T15:36:32.1234567Z","receivedAtUtc":"2026-10-16T15:36:32Z","payload":null}
    ],"nextCursor":null,"hasMore":false,"futureField":true}
    """, Encoding.UTF8, "application/json")
}));
var dates = await new TelemetryApiClient(datesHttp).GetEventsAsync(1);
Check(dates.Items.Count == 2 && dates.Items[0].Payload?.GetProperty("nested").GetProperty("a")[1].GetBoolean() == true && dates.Items[1].Payload is null, "Parse UTC with 0–7 fractional digits, arbitrary payload, nulls and unknown fields.");
Check(new EventItem(dates.Items[0], new DeviceListItem(1, "code", "name"), navigation).Timestamp == dates.Items[0].OccurredAtUtc.ToLocalTime().ToString("g"), "Show event occurrence in the local timezone.");

foreach (var status in new[] { HttpStatusCode.TooManyRequests, HttpStatusCode.ServiceUnavailable })
{
    using var throttledHttp = Http(new ScriptedHandler((_, _) =>
    {
        var response = new HttpResponseMessage(status) { Content = new StringContent("not json") };
        response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(15));
        return response;
    }));
    var throttled = new TelemetryApiClient(throttledHttp);
    try { await throttled.GetDevicesAsync(1); throw new Exception("Expected backoff."); }
    catch (TelemetryApiException e) { Check(e.Status == status && e.RetryAfter == TimeSpan.FromSeconds(15), "Respect Retry-After even for a proxy error response."); }
    try { await throttled.GetDevicesAsync(1); throw new Exception("Expected shared backoff."); }
    catch (TelemetryApiException e) { Check(e.RetryAfter > TimeSpan.Zero, "Requests during Retry-After fail locally without hammering the server."); }
}

var encodedCursor = "opaque/+?=& value";
string? requestedQuery = null;
using var cursorHttp = Http(new ScriptedHandler((request, _) =>
{
    requestedQuery = request.RequestUri!.Query;
    return FakeBackend.Json(new EventPage([], null, false));
}));
await new TelemetryApiClient(cursorHttp).GetEventsAsync(1, encodedCursor);
Check(requestedQuery == "?limit=20&cursor=" + Uri.EscapeDataString(encodedCursor), "Cursor must be encoded as one opaque query value.");

using var cancelledHttp = Http(new WaitingHandler());
var cancelledModel = new DevicesPageModel(new TelemetryApiClient(cancelledHttp), session, navigation);
var pending = cancelledModel.RefreshAsync();
cancelledModel.CancelLoading();
await pending;
Check(!cancelledModel.IsBusy && !cancelledModel.HasError && cancelledModel.Devices.Count == 0, "Leaving the page cancels pending requests without an error or stale data.");
Console.WriteLine($"Passed {assertions} assertions: session, API serialization/errors, device views, scoped and merged pagination, refresh, cancellation and customer isolation.");

sealed class ScriptedHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> respond) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(respond(request, cancellationToken));
}
sealed class WaitingHandler : HttpMessageHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await Task.Delay(Timeout.Infinite, cancellationToken);
        throw new InvalidOperationException();
    }
}
sealed class FakeBackend : HttpMessageHandler
{
    public List<string> Paths { get; } = [];
    public bool FailNext { get; set; }
    public bool InvalidCursorOnce { get; set; }
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
    public Dictionary<int, List<TelemetryEvent>> History { get; } = new()
    {
        [1] = MakeEvents(1, 45), [2] = MakeEvents(2, 7), [3] = [], [46] = MakeEvents(46, 1)
    };
    private static List<TelemetryEvent> MakeEvents(int deviceId, int count) => Enumerable.Range(0, count)
        .Select(i => new TelemetryEvent(Guid.Parse($"00000000-0000-0000-{deviceId:0000}-{i:000000000000}"), 100000 + i,
            DateTimeOffset.Parse("2026-10-19T09:00:00Z").AddMinutes(-i), DateTimeOffset.Parse("2026-10-19T09:00:00Z"), null)).ToList();
    public static HttpResponseMessage Json<T>(T value) => new(HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(value, Options), Encoding.UTF8, "application/json") };
    private static HttpResponseMessage Problem(HttpStatusCode status, string code) => new(status) { Content = new StringContent(JsonSerializer.Serialize(new ApiProblem(code, "test-trace"), Options), Encoding.UTF8, "application/problem+json") };
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Paths.Add(request.RequestUri!.PathAndQuery);
        if (FailNext) { FailNext = false; throw new HttpRequestException("Simulated connection loss"); }
        var segments = request.RequestUri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var id = int.Parse(segments[3]);
        if (segments[2] == "customers")
            return Task.FromResult(id switch
            {
                1 => Json(new[] { new DeviceListItem(1, "code-1", "Sensor 1"), new DeviceListItem(2, "code-2", "Sensor 2"), new DeviceListItem(3, "code-3", "Sensor 3") }),
                10 => Json(new[] { new DeviceListItem(46, "code-46", "Customer ten") }),
                _ => Problem(HttpStatusCode.NotFound, "customer.not_found")
            });
        if (!History.TryGetValue(id, out var history)) return Task.FromResult(Problem(HttpStatusCode.NotFound, "device.not_found"));
        if (segments.Length == 4)
            return Task.FromResult(Json(new TelemetryDevice(id, "code-" + id, "Sensor " + id, id == 46 ? 10 : 1, history.FirstOrDefault()?.OccurredAtUtc, history.FirstOrDefault()?.StatusCode)));
        var query = request.RequestUri.Query.TrimStart('?').Split('&').Select(p => p.Split('=', 2)).ToDictionary(p => p[0], p => Uri.UnescapeDataString(p[1]));
        var offset = 0;
        if (query.TryGetValue("cursor", out var cursor))
        {
            if (InvalidCursorOnce) { InvalidCursorOnce = false; return Task.FromResult(Problem(HttpStatusCode.BadRequest, "pagination.invalid_cursor")); }
            if (!cursor.StartsWith(id + ":")) throw new Exception("Cursor leaked across devices.");
            offset = int.Parse(cursor.Split(':')[1].Split('/')[0]);
        }
        var limit = int.Parse(query["limit"]);
        var items = history.Skip(offset).Take(limit).ToArray();
        var hasMore = offset + items.Length < history.Count;
        return Task.FromResult(Json(new EventPage(items, hasMore ? $"{id}:{offset + items.Length}/+?=" : null, hasMore)));
    }
}
