using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelemetryMobile.Localization;
using TelemetryMobile.Services;

namespace TelemetryMobile.PageModels;

public partial class DeviceDetailsPageModel(ITelemetryApi api, CustomerSession session, DemoNavigationService navigation) : RemotePageModel
{
    private int deviceId;
    [ObservableProperty] private DeviceItem? device;
    [ObservableProperty] private IReadOnlyList<EventItem> recentEvents = [];
    [ObservableProperty] private string lastContact = string.Empty;
    [ObservableProperty] private string lastStatus = string.Empty;
    public void SetDevice(int id) => deviceId = id;

    [RelayCommand]
    public Task RefreshAsync() => RunAsync(async cancellationToken =>
    {
        if (deviceId <= 0 || session.CustomerId is not { } customerId) return;
        try
        {
            var details = await api.GetDeviceAsync(deviceId, cancellationToken);
            if (details.CustomerId != customerId)
            {
                await navigation.CloseDeviceAsync();
                return;
            }
            var device = new Models.DeviceListItem(details.Id, details.DeviceCode, details.Name);
            Device = new DeviceItem(device, navigation);
            LastContact = details.LastEventAtUtc?.ToLocalTime().ToString("g") ?? LocalizationManager.Instance["NoEvents"];
            LastStatus = details.LastStatusCode is { } status
                ? string.Format(LocalizationManager.Instance["StatusCode"], status) : LocalizationManager.Instance["NoEvents"];
            var page = await api.GetEventsAsync(deviceId, limit: 3, cancellationToken: cancellationToken);
            RecentEvents = page.Items.Select(e => new EventItem(e, device, navigation)).ToArray();
        }
        catch (TelemetryApiException error) when (error.Code == "device.not_found")
        {
            await navigation.CloseDeviceAsync();
        }
    });

    [RelayCommand] private Task AllEvents() => Device is null ? Task.CompletedTask : navigation.OpenDeviceEventsAsync(Device.Device.Id);
}
