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
        if (query.TryGetValue("deviceId", out var id) && id is string value) model.Load(value);
    }

    private async void OnBackClicked(object? sender, EventArgs e) => await Shell.Current.GoToAsync("..");
    private void OnMenuClicked(object? sender, EventArgs e) => model.AllEventsCommand.Execute(null);
}
