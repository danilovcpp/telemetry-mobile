namespace TelemetryMobile.Services;

public sealed class DemoNavigationService(IServiceProvider services)
{
    public void SignIn()
    {
        var window = Application.Current!.Windows[0];
        window.Page = new AppShell(services);
    }

    public void SignOut()
    {
        Application.Current!.Windows[0].Page = services.GetRequiredService<LoginPage>();
    }

    public Task OpenDeviceAsync(string id) => Shell.Current.GoToAsync(nameof(DeviceDetailsPage),
        new Dictionary<string, object> { ["deviceId"] = id });

    public Task OpenDeviceEventsAsync(string id) => Shell.Current.GoToAsync("device-events",
        new Dictionary<string, object> { ["deviceId"] = id });
}
