namespace TelemetryMobile.Pages;
public partial class LoginPage : ContentPage
{
    public LoginPage(LoginPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    private void OnViewportSizeChanged(object? sender, EventArgs e)
    {
        // Content may grow naturally for large fonts; ScrollView keeps fields reachable above IME.
        ContentLayout.MinimumHeightRequest = Math.Max(0, LoginScroll.Height);
        ContentLayout.WidthRequest = Math.Min((double)Application.Current!.Resources["LoginMaxWidth"], Math.Max(0, LoginScroll.Width));
        LoginDecoration.IsVisible = LoginScroll.Height >= (double)Application.Current.Resources["LoginDecorationMinHeight"];
    }
    private void OnCustomerCompleted(object? sender, EventArgs e) => PasswordField.FocusInput();
    private void OnPasswordCompleted(object? sender, EventArgs e) => ((LoginPageModel)BindingContext).SignInCommand.Execute(null);
}
