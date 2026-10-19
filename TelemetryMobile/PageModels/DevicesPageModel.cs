using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelemetryMobile.Models;
using TelemetryMobile.Services;

namespace TelemetryMobile.PageModels;

public partial class DevicesPageModel(ITelemetryApi api, CustomerSession session, DemoNavigationService navigation) : RemotePageModel
{
    private IReadOnlyList<DeviceListItem> allDevices = [];
    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private bool isSearchVisible;
    [ObservableProperty] private IReadOnlyList<DeviceItem> devices = [];
    public string DeviceCount => allDevices.Count.ToString();
    public string CustomerId => session.CustomerId?.ToString() ?? string.Empty;

    partial void OnSearchTextChanged(string value) => ApplySearch();
    [RelayCommand] private void ToggleSearch() => IsSearchVisible = !IsSearchVisible;

    [RelayCommand]
    public Task RefreshAsync() => RunAsync(async cancellationToken =>
    {
        if (session.CustomerId is not { } id) return;
        allDevices = await api.GetDevicesAsync(id, cancellationToken);
        ApplySearch();
        OnPropertyChanged(nameof(DeviceCount));
    });

    private void ApplySearch() => Devices = allDevices
        .Where(d => d.Name.Contains(SearchText.Trim(), StringComparison.OrdinalIgnoreCase) || d.DeviceCode.Contains(SearchText.Trim(), StringComparison.OrdinalIgnoreCase))
        .Select(d => new DeviceItem(d, navigation)).ToArray();
}
