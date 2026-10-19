using Microsoft.Maui.Storage;

namespace TelemetryMobile.Services;

/// <summary>MVP customer selection only. No password or authentication token is stored.</summary>
public sealed class CustomerSession
{
    private const string CustomerKey = "CustomerId";
    public int? CustomerId { get; private set; }

    public CustomerSession()
    {
        var saved = Preferences.Default.Get(CustomerKey, 0);
        CustomerId = saved > 0 ? saved : null;
    }

    public void SignIn(int customerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(customerId);
        Preferences.Default.Set(CustomerKey, customerId);
        CustomerId = customerId;
    }

    public void SignOut()
    {
        Preferences.Default.Remove(CustomerKey);
        CustomerId = null;
    }
}
