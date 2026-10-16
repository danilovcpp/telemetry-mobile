using TelemetryMobile.Localization;

namespace TelemetryMobile.Pages;

public partial class EventsPage : ContentPage, IQueryAttributable
{
    private readonly EventsPageModel model;
    public EventsPage(EventsPageModel model)
    {
        InitializeComponent();
        BindingContext = this.model = model;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        model.SetDevice(query.TryGetValue("deviceId", out var id) ? id as string : null);
        Shell.SetTabBarIsVisible(this, !model.IsDeviceScope);
    }

    private async void OnBackClicked(object? sender, EventArgs e) => await Shell.Current.GoToAsync("..");
    private async void OnFilterClicked(object? sender, EventArgs e)
    {
        var loc = LocalizationManager.Instance;
        var choice = await DisplayActionSheetAsync(loc["Filter"], loc["Cancel"], null, model.Filters.Select(f => f.Caption).ToArray());
        var filter = model.Filters.FirstOrDefault(f => f.Caption == choice);
        if (filter is not null) model.SelectFilterCommand.Execute(filter.Value);
    }
}
