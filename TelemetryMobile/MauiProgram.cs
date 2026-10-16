using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

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
        return builder.Build();
    }
}
