// Exercise the real page models without requiring an Android or MAUI runtime.
namespace TelemetryMobile.Localization
{
    public sealed class LocalizationManager
    {
        public static LocalizationManager Instance { get; } = new();
        public string this[string key] => key;
        public void SetCulture(string language) { }
    }
}

namespace TelemetryMobile.Services
{
    public sealed class DemoNavigationService
    {
        public int SignInCount { get; private set; }
        public string? LastDeviceId { get; private set; }
        public string? EventsDeviceId { get; private set; }
        public void SignIn() => SignInCount++;
        public Task OpenDeviceAsync(string id) { LastDeviceId = id; return Task.CompletedTask; }
        public Task OpenDeviceEventsAsync(string id) { EventsDeviceId = id; return Task.CompletedTask; }
    }
}
