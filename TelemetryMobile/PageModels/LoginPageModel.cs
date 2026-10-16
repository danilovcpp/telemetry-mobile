using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelemetryMobile.Localization;
using TelemetryMobile.Services;

namespace TelemetryMobile.PageModels;

public partial class LoginPageModel(DemoNavigationService navigation) : ObservableObject
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
        if (CustomerIdInvalid || PasswordInvalid) return;
        if (CustomerId.Trim() == "test" && Password == "test")
        {
            Password = string.Empty;
            navigation.SignIn();
        }
        else
        {
            AuthUnavailable = true;
        }
    }

    [RelayCommand]
    private void ChangeLanguage(string language) => LocalizationManager.Instance.SetCulture(language);
}
