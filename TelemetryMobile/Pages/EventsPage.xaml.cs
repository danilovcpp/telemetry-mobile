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
        model.SetDevice(query.TryGetValue("deviceId", out var id) && id is int value ? value : null);
        Shell.SetTabBarIsVisible(this, !model.IsDeviceScope);
    }

    private async void OnBackClicked(object? sender, EventArgs e) => await Shell.Current.GoToAsync("..");
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await model.RefreshAsync();
    }

    protected override void OnDisappearing()
    {
        model.CancelLoading();
        base.OnDisappearing();
    }
}
