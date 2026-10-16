using TelemetryMobile.Localization;
namespace TelemetryMobile;

public partial class App : Application
{
    private readonly IServiceProvider services;
    public App(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        UserAppTheme = AppTheme.Dark;
        LocalizationManager.Instance.Initialize();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
#if DESIGN_SYSTEM_SHOWCASE
        return new Window(new Pages.Development.DesignSystemShowcasePage());
#else
        return new Window(services.GetRequiredService<LoginPage>());
#endif
    }
}
