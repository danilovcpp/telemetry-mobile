using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using TelemetryMobile.Services;

namespace TelemetryMobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
            .UseMauiCommunityToolkit();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddTransient<LoginPageModel>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddSingleton<MockTelemetryService>();
        builder.Services.AddSingleton<DemoNavigationService>();
        builder.Services.AddTransient<DevicesPageModel>();
        builder.Services.AddTransient<DevicesPage>();
        builder.Services.AddTransient<DeviceDetailsPageModel>();
        builder.Services.AddTransient<DeviceDetailsPage>();
        builder.Services.AddTransient<EventsPageModel>();
        builder.Services.AddTransient<EventsPage>();
        return builder.Build();
    }
}
