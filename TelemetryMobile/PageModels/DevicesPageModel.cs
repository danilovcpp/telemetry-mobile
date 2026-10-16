using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelemetryMobile.Models;
using TelemetryMobile.Services;

namespace TelemetryMobile.PageModels;

public partial class DevicesPageModel : ObservableObject
{
    private readonly MockTelemetryService data;
    private readonly DemoNavigationService navigation;
    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private bool isSearchVisible;
    [ObservableProperty] private IReadOnlyList<DeviceItem> devices = [];
    public IReadOnlyList<FilterOption> Filters { get; }
    public string OnlineCount => data.Devices.Count(d => d.Status == DeviceStatus.Online).ToString();
    public string OfflineCount => data.Devices.Count(d => d.Status == DeviceStatus.Offline).ToString();
    public string ErrorCount => data.Devices.Count(d => d.Status == DeviceStatus.Error).ToString();
    private string selectedFilter = "All";

    public DevicesPageModel(MockTelemetryService data, DemoNavigationService navigation)
    {
        this.data = data;
        this.navigation = navigation;
        Filters = new[] { "All", "Online", "Offline", "Error" }.Select(key => new FilterOption(key, key, SelectFilterCommand) { IsSelected = key == "All" }).ToArray();
        Refresh();
    }

    partial void OnSearchTextChanged(string value) => Refresh();
    [RelayCommand] private void ToggleSearch() => IsSearchVisible = !IsSearchVisible;
    [RelayCommand] private void SelectFilter(string value)
    {
        selectedFilter = value;
        foreach (var filter in Filters) filter.IsSelected = filter.Value == value;
        Refresh();
    }

    private void Refresh() => Devices = data.Devices
        .Where(d => selectedFilter == "All" || d.Status.ToString() == selectedFilter)
        .Where(d => d.Name.Contains(SearchText.Trim(), StringComparison.OrdinalIgnoreCase) || d.Address.Contains(SearchText.Trim(), StringComparison.OrdinalIgnoreCase))
        .Select(d => new DeviceItem(d, navigation)).ToArray();
}
