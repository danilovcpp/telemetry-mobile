using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelemetryMobile.Localization;

namespace TelemetryMobile.PageModels;

public partial class LoginPageModel : ObservableObject
{
    [ObservableProperty] private string customerId = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private bool customerIdInvalid;
    [ObservableProperty] private bool passwordInvalid;
    [ObservableProperty] private bool authUnavailable;

    partial void OnCustomerIdChanged(string value) { CustomerIdInvalid = false; AuthUnavailable = false; }
    partial void OnPasswordChanged(string value) { PasswordInvalid = false; AuthUnavailable = false; }

    [RelayCommand]
    private void SignIn()
    {
        CustomerIdInvalid = string.IsNullOrWhiteSpace(CustomerId);
        PasswordInvalid = string.IsNullOrEmpty(Password);
        AuthUnavailable = !CustomerIdInvalid && !PasswordInvalid;
        // Preserve the existing behavior until a real authentication service is connected.
        // No mock session, credential persistence, or fabricated business navigation.
    }

    [RelayCommand]
    private void ChangeLanguage(string language) => LocalizationManager.Instance.SetCulture(language);
}
