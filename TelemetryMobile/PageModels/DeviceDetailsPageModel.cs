using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelemetryMobile.Localization;
using TelemetryMobile.Services;

namespace TelemetryMobile.PageModels;

public partial class DeviceDetailsPageModel(MockTelemetryService data, DemoNavigationService navigation) : ObservableObject
{
    [ObservableProperty] private DeviceItem? device;
    [ObservableProperty] private IReadOnlyList<MetricItem> metrics = [];
    [ObservableProperty] private IReadOnlyList<EventItem> recentEvents = [];
    public string LastContact => Device is null ? string.Empty : $"{LocalizationManager.Instance["Today"]}, {Device.Device.LastContact}";
    public bool HasMetrics => Metrics.Count > 0;

    public void Load(string id)
    {
        var device = data.Devices.First(d => d.Id == id);
        Device = new DeviceItem(device, navigation);
        Metrics = device.Metrics.Select(m => new MetricItem(m)).ToArray();
        RecentEvents = data.Events.Where(e => e.DeviceId == id).Take(1).Select(e => new EventItem(e, device, navigation)).ToArray();
        OnPropertyChanged(nameof(LastContact));
        OnPropertyChanged(nameof(HasMetrics));
    }

    [RelayCommand] private Task AllEvents() => Device is null ? Task.CompletedTask : navigation.OpenDeviceEventsAsync(Device.Device.Id);
}
