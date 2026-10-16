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

public sealed class DeviceItem(TelemetryDevice device, DemoNavigationService navigation)
{
    public TelemetryDevice Device { get; } = device;
    public string Name => Device.Name;
    public string Address => Device.Address;
    public string Icon => Device.Icon;
    public StatusState State => Device.Status switch { DeviceStatus.Online => StatusState.Online, DeviceStatus.Offline => StatusState.Offline, _ => StatusState.Error };
    public string StatusText => LocalizationManager.Instance[Device.Status.ToString()];
    public ICommand OpenCommand { get; } = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(() => navigation.OpenDeviceAsync(device.Id));
}

public sealed class EventItem(TelemetryEvent entry, TelemetryDevice device, DemoNavigationService navigation)
{
    public string Title => LocalizationManager.Instance[entry.TitleKey];
    public string Description => device.Name;
    public string Timestamp => entry.Time;
    public StatusState State => entry.Severity switch { EventSeverity.Alarm => StatusState.Error, EventSeverity.Warning => StatusState.Warning, _ => entry.IsRecovery ? StatusState.Online : StatusState.Unknown };
    public string StatusText => LocalizationManager.Instance[entry.Severity.ToString()];
    public ICommand OpenCommand { get; } = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(() => navigation.OpenDeviceAsync(device.Id));
}

public sealed class MetricItem(DeviceMetric metric)
{
    public string Name => LocalizationManager.Instance[metric.NameKey];
    public string Value => metric.NameKey == "Network" ? $"{metric.Value} {LocalizationManager.Instance["Mbps"]}" : metric.Value;
    public string Icon => metric.Icon;
    public string ColorKey => metric.ColorKey;
    public float[] Samples => metric.Samples;
}
