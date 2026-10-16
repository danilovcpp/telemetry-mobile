using TelemetryMobile.Localization;
using TelemetryMobile.Services;
using TelemetryMobile.Theming;

namespace TelemetryMobile.Pages;

public partial class DevicesPage : ContentPage
{
    private readonly DemoNavigationService navigation;
    public DevicesPage(DevicesPageModel model, DemoNavigationService navigation)
    {
        InitializeComponent();
        BindingContext = model;
        this.navigation = navigation;
    }

    private void OnSummarySizeChanged(object? sender, EventArgs e)
    {
        var width = Math.Max(104, (SummaryScroll.Width - 16) / 3);
        OnlineSummary.WidthRequest = OfflineSummary.WidthRequest = ErrorSummary.WidthRequest = width;
    }

    private async void OnMenuClicked(object? sender, EventArgs e)
    {
        var loc = LocalizationManager.Instance;
        var model = (DevicesPageModel)BindingContext;
        var options = model.Filters.Select(f => f.Caption).Concat(new[] { loc["LightTheme"], loc["SignOut"] }).ToArray();
        var choice = await DisplayActionSheetAsync(loc["Filter"], loc["Cancel"], null, options);
        var filter = model.Filters.FirstOrDefault(f => f.Caption == choice);
        if (filter is not null) model.SelectFilterCommand.Execute(filter.Value);
        if (choice == loc["LightTheme"]) ThemeManager.Instance.IsLightTheme = !ThemeManager.Instance.IsLightTheme;
        if (choice == loc["SignOut"]) navigation.SignOut();
    }
}
