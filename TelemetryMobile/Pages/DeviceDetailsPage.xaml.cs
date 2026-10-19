namespace TelemetryMobile.Pages;

public partial class DeviceDetailsPage : ContentPage, IQueryAttributable
{
    private readonly DeviceDetailsPageModel model;
    public DeviceDetailsPage(DeviceDetailsPageModel model)
    {
        InitializeComponent();
        BindingContext = this.model = model;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("deviceId", out var id) && id is int value) model.SetDevice(value);
    }

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

    private async void OnBackClicked(object? sender, EventArgs e) => await Shell.Current.GoToAsync("..");
    private void OnMenuClicked(object? sender, EventArgs e) => model.AllEventsCommand.Execute(null);
}
