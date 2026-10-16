using TelemetryMobile.Localization;

namespace TelemetryMobile;

public sealed class AppShell : Shell
{
    private static bool routesRegistered;

    public AppShell(IServiceProvider services)
    {
        FlyoutBehavior = FlyoutBehavior.Disabled;
        SetNavBarIsVisible(this, false);
        SetDynamicResource(BackgroundColorProperty, "ColorPageBackground");
        SetDynamicResource(TabBarBackgroundColorProperty, "ColorPageBackground");
        SetDynamicResource(TabBarForegroundColorProperty, "ColorAccentPrimary");
        SetDynamicResource(TabBarTitleColorProperty, "ColorAccentPrimary");
        SetDynamicResource(TabBarUnselectedColorProperty, "ColorTextSecondary");

        var tabs = new TabBar();
        tabs.Items.Add(CreateTab<DevicesPage>("Devices", "devices.png", "devices", services));
        tabs.Items.Add(CreateTab<EventsPage>("Events", "events.png", "events", services));
        Items.Add(tabs);

        if (!routesRegistered)
        {
            Routing.RegisterRoute(nameof(DeviceDetailsPage), typeof(DeviceDetailsPage));
            Routing.RegisterRoute("device-events", typeof(EventsPage));
            routesRegistered = true;
        }
    }

    private static Tab CreateTab<T>(string titleKey, string icon, string route, IServiceProvider services) where T : ContentPage
    {
        var tab = new Tab { Icon = icon, Route = route };
        tab.SetBinding(BaseShellItem.TitleProperty, new Binding($"[{titleKey}]", source: LocalizationManager.Instance));
        tab.Items.Add(new ShellContent { Route = route + "-root", ContentTemplate = new DataTemplate(() => services.GetRequiredService<T>()) });
        return tab;
    }
}
