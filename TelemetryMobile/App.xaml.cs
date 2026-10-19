using TelemetryMobile.Localization;
namespace TelemetryMobile;

public partial class App : Application
{
    private readonly IServiceProvider services;
    public App(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        Theming.ThemeManager.Instance.Initialize(this);
        LocalizationManager.Instance.Initialize();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
#if DESIGN_SYSTEM_SHOWCASE
        return new Window(new Pages.Development.DesignSystemShowcasePage());
#else
        return new Window(services.GetRequiredService<Services.CustomerSession>().CustomerId.HasValue
            ? new AppShell(services) : services.GetRequiredService<LoginPage>());
#endif
    }
}
