using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelemetryMobile.Localization;
using TelemetryMobile.Services;

namespace TelemetryMobile.PageModels;

public partial class LoginPageModel(CustomerSession session, ITelemetryApi api, DemoNavigationService navigation) : RemotePageModel
{
    [ObservableProperty] private string customerId = session.CustomerId?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private bool customerIdInvalid;

    partial void OnCustomerIdChanged(string value) { CustomerIdInvalid = false; ErrorMessage = string.Empty; }

    [RelayCommand]
    private async Task SignIn()
    {
        CustomerIdInvalid = !int.TryParse(CustomerId.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0;
        if (CustomerIdInvalid) return;
        await RunAsync(async cancellationToken =>
        {
            // Verify the customer before persisting the selection. Password is ignored in this MVP.
            await api.GetDevicesAsync(id, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            session.SignIn(id);
            Password = string.Empty;
            navigation.SignIn();
        });
    }

    [RelayCommand] private void ChangeLanguage(string language) => LocalizationManager.Instance.SetCulture(language);
}
