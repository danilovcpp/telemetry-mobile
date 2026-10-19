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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((DevicesPageModel)BindingContext).RefreshAsync();
    }

    protected override void OnDisappearing()
    {
        ((DevicesPageModel)BindingContext).CancelLoading();
        base.OnDisappearing();
    }

    private async void OnMenuClicked(object? sender, EventArgs e)
    {
        var loc = LocalizationManager.Instance;
        var choice = await DisplayActionSheetAsync(loc["Menu"], loc["Cancel"], null, loc["LightTheme"], loc["SignOut"]);
        if (choice == loc["LightTheme"]) ThemeManager.Instance.IsLightTheme = !ThemeManager.Instance.IsLightTheme;
        if (choice == loc["SignOut"]) navigation.SignOut();
    }
}
