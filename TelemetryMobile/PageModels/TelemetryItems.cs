using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using TelemetryMobile.Localization;
using TelemetryMobile.Models;
using TelemetryMobile.Services;
using TelemetryMobile.UI.Components;

namespace TelemetryMobile.PageModels;

public partial class FilterOption(string key, string value, ICommand command) : ObservableObject
{
    public string Caption => LocalizationManager.Instance[key];
    public string Value { get; } = value;
    public ICommand Command { get; } = command;
    [ObservableProperty] private bool isSelected;
}

public sealed class DeviceItem(DeviceListItem device, DemoNavigationService navigation)
{
    public DeviceListItem Device { get; } = device;
    public string Name => Device.Name;
    public string Address => Device.DeviceCode;
    public string Icon => "sensor.png";
    public ICommand OpenCommand { get; } = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(() => navigation.OpenDeviceAsync(device.Id));
}

public sealed class EventItem(TelemetryEvent entry, DeviceListItem device, DemoNavigationService navigation)
{
    public Guid Id => entry.Id;
    public string Title => string.Format(LocalizationManager.Instance["StatusCode"], entry.StatusCode);
    public string Description => device.Name;
    public string Timestamp => entry.OccurredAtUtc.ToLocalTime().ToString("g");
    public string Payload => entry.Payload?.ToString() ?? string.Empty;
    public bool HasPayload => !string.IsNullOrEmpty(Payload);
    // The API has no agreed status dictionary; retain the numeric code without guessing severity.
    public StatusState State => StatusState.Unknown;
    public ICommand OpenCommand { get; } = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(() => navigation.OpenDeviceAsync(device.Id));
}
