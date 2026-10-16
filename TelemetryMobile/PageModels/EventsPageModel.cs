using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelemetryMobile.Services;

namespace TelemetryMobile.PageModels;

public partial class EventsPageModel : ObservableObject
{
    private readonly MockTelemetryService data;
    private readonly DemoNavigationService navigation;
    private string selectedFilter = "All";
    private string? deviceId;
    [ObservableProperty] private IReadOnlyList<EventItem> events = [];
    [ObservableProperty] private bool isDeviceScope;
    [ObservableProperty] private string deviceName = string.Empty;
    public IReadOnlyList<FilterOption> Filters { get; }

    public EventsPageModel(MockTelemetryService data, DemoNavigationService navigation)
    {
        this.data = data;
        this.navigation = navigation;
        Filters = new[] { "All", "Information", "Warning", "Alarm" }.Select(key => new FilterOption(key, key, SelectFilterCommand) { IsSelected = key == "All" }).ToArray();
        Refresh();
    }

    public void SetDevice(string? id)
    {
        deviceId = id;
        IsDeviceScope = id is not null;
        DeviceName = data.Devices.FirstOrDefault(d => d.Id == id)?.Name ?? string.Empty;
        Refresh();
    }

    [RelayCommand] private void SelectFilter(string value)
    {
        selectedFilter = value;
        foreach (var filter in Filters) filter.IsSelected = filter.Value == value;
        Refresh();
    }

    private void Refresh() => Events = data.Events
        .Where(e => deviceId is null || e.DeviceId == deviceId)
        .Where(e => selectedFilter == "All" || e.Severity.ToString() == selectedFilter)
        .Select(e => new EventItem(e, data.Devices.First(d => d.Id == e.DeviceId), navigation)).ToArray();
}
