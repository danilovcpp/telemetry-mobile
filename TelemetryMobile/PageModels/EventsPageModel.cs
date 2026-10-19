using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelemetryMobile.Models;
using TelemetryMobile.Services;

namespace TelemetryMobile.PageModels;

public partial class EventsPageModel(ITelemetryApi api, CustomerSession session, DemoNavigationService navigation) : RemotePageModel
{
    private DeviceEventFeed? feed;
    private int? deviceId;
    [ObservableProperty] private IReadOnlyList<EventItem> events = [];
    [ObservableProperty] private bool isDeviceScope;
    [ObservableProperty] private string deviceName = string.Empty;
    [ObservableProperty] private bool hasMore;

    public void SetDevice(int? id)
    {
        deviceId = id;
        IsDeviceScope = id is not null;
        feed = null;
        Events = [];
        HasMore = false;
    }

    [RelayCommand]
    public Task RefreshAsync() => RunAsync(async cancellationToken =>
    {
        feed = null;
        HasMore = false;
        Events = [];
        if (session.CustomerId is not { } customerId) return;
        // Resolve scope through the selected customer's list, never through another customer's cache.
        var devices = await api.GetDevicesAsync(customerId, cancellationToken);
        var selected = deviceId is { } id ? devices.Where(d => d.Id == id).ToArray() : devices;
        DeviceName = IsDeviceScope ? selected.FirstOrDefault()?.Name ?? string.Empty : string.Empty;
        feed = new DeviceEventFeed(api, selected);
        await AppendAsync(cancellationToken);
    });

    [RelayCommand]
    private Task LoadMoreAsync() => RunAsync(async cancellationToken =>
    {
        if (feed is null || !feed.HasMore) return;
        try { await AppendAsync(cancellationToken); }
        catch (TelemetryApiException error) when (error.Code == "pagination.invalid_cursor")
        {
            // Cursors are short-lived and scoped to one device. Restart the entire merged history.
            feed = null;
            Events = [];
            HasMore = false;
            var devices = await api.GetDevicesAsync(session.CustomerId!.Value, cancellationToken);
            feed = new DeviceEventFeed(api, deviceId is { } id ? devices.Where(d => d.Id == id).ToArray() : devices);
            await AppendAsync(cancellationToken);
        }
    });

    private async Task AppendAsync(CancellationToken cancellationToken)
    {
        var page = await feed!.LoadMoreAsync(cancellationToken);
        Events = Events.Concat(page.Select(e => new EventItem(e.Event, e.Device, navigation))).DistinctBy(e => e.Id).ToArray();
        HasMore = feed.HasMore;
    }
}
