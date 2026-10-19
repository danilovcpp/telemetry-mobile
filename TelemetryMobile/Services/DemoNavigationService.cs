namespace TelemetryMobile.Services;

public sealed class DemoNavigationService(IServiceProvider services, CustomerSession session)
{
    public void SignIn()
    {
        var window = Application.Current!.Windows[0];
        window.Page = new AppShell(services);
    }

    public void SignOut()
    {
        session.SignOut();
        Application.Current!.Windows[0].Page = services.GetRequiredService<LoginPage>();
    }

    public Task CloseDeviceAsync() => Shell.Current.GoToAsync("..");

    public Task OpenDeviceAsync(int id) => Shell.Current.GoToAsync(nameof(DeviceDetailsPage),
        new Dictionary<string, object> { ["deviceId"] = id });

    public Task OpenDeviceEventsAsync(int id) => Shell.Current.GoToAsync("device-events",
        new Dictionary<string, object> { ["deviceId"] = id });
}
