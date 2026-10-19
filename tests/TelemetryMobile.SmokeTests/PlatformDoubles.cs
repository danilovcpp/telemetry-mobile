// Exercise the real API client and page models without a MAUI runtime.
namespace TelemetryMobile.Localization
{
    public sealed class LocalizationManager
    {
        public static LocalizationManager Instance { get; } = new();
        public string this[string key] => key switch { "StatusCode" => "Status {0}", "RetryAfter" => "Retry in {0} seconds", _ => key };
        public void SetCulture(string language) { }
    }
}

namespace Microsoft.Maui.Storage
{
    public sealed class Preferences
    {
        public static Preferences Default { get; } = new();
        private readonly Dictionary<string, int> values = new();
        public int Get(string key, int fallback) => values.GetValueOrDefault(key, fallback);
        public void Set(string key, int value) => values[key] = value;
        public void Remove(string key) => values.Remove(key);
    }
}

namespace TelemetryMobile.Services
{
    public sealed class DemoNavigationService
    {
        public int SignInCount { get; private set; }
        public int CloseDeviceCount { get; private set; }
        public int? LastDeviceId { get; private set; }
        public int? EventsDeviceId { get; private set; }
        public void SignIn() => SignInCount++;
        public Task CloseDeviceAsync() { CloseDeviceCount++; return Task.CompletedTask; }
        public Task OpenDeviceAsync(int id) { LastDeviceId = id; return Task.CompletedTask; }
        public Task OpenDeviceEventsAsync(int id) { EventsDeviceId = id; return Task.CompletedTask; }
    }
}
