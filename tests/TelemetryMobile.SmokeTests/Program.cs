using TelemetryMobile.Models;
using TelemetryMobile.PageModels;
using TelemetryMobile.Services;

var assertions = 0;
void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
    assertions++;
}

var data = new MockTelemetryService();
var navigation = new DemoNavigationService();
var login = new LoginPageModel(navigation);
login.SignInCommand.Execute(null);
Check(login.CustomerIdInvalid && login.PasswordInvalid && navigation.SignInCount == 0, "Empty credentials must not create a session.");
login.CustomerId = "test";
login.Password = "wrong";
login.SignInCommand.Execute(null);
Check(login.AuthUnavailable && navigation.SignInCount == 0, "Invalid credentials must not create a session.");
login.Password = "test";
login.SignInCommand.Execute(null);
Check(navigation.SignInCount == 1 && login.Password == "", "test/test must sign in and clear the password.");

var devices = new DevicesPageModel(data, navigation);
Check(devices.Devices.Count == 16 && devices.OnlineCount == "12" && devices.OfflineCount == "3" && devices.ErrorCount == "1", "Summary counts must match the dataset.");
devices.SelectFilterCommand.Execute("Offline");
Check(devices.Devices.Count == 3 && devices.Devices.All(d => d.Device.Status == DeviceStatus.Offline), "Offline filter.");
devices.SearchText = "192.168.3.12";
Check(devices.Devices.Count == 1 && devices.Devices[0].Name == "Контроллер", "Search and status filter must combine.");
devices.SelectFilterCommand.Execute("Online");
Check(devices.Devices.Count == 0, "No matching devices produces an empty list.");
devices.SelectFilterCommand.Execute("All");
devices.SearchText = "СЕРВЕР-01";
Check(devices.Devices.Count == 1, "Name search must ignore case.");
await ((CommunityToolkit.Mvvm.Input.IAsyncRelayCommand)devices.Devices[0].OpenCommand).ExecuteAsync(null);
Check(navigation.LastDeviceId == "server-01", "Device selection must navigate to the selected device.");

var events = new EventsPageModel(data, navigation);
Check(events.Events.Count == 8, "All demo events must be visible.");
events.SelectFilterCommand.Execute("Alarm");
Check(events.Events.Count == 2, "Alarm filter.");
events.SetDevice("server-01");
Check(events.Events.Count == 0 && events.IsDeviceScope, "Device scope and severity filters must combine.");
events.SelectFilterCommand.Execute("All");
Check(events.Events.Count == 2 && events.Events.All(e => e.Description == "Сервер-01"), "Device events must remain scoped.");
events.SetDevice(null);
Check(events.Events.Count == 8 && !events.IsDeviceScope, "Clearing device scope restores global events.");

var details = new DeviceDetailsPageModel(data, navigation);
foreach (var device in data.Devices)
{
    details.Load(device.Id);
    Check(details.Device?.Device.Id == device.Id, "Each device must load its own details.");
    Check(details.HasMetrics == (device.Status != DeviceStatus.Offline), "Offline devices must not display live metrics.");
    Check(details.RecentEvents.All(e => e.Description == device.Name), "Recent events must belong to the device.");
}
details.Load("server-01");
Check(details.Metrics.Count == 4 && details.Metrics[0].Value == "12%", "Server metrics from reference.");
await details.AllEventsCommand.ExecuteAsync(null);
Check(navigation.EventsDeviceId == "server-01", "All events action must pass the current device ID.");
Check(data.Events.All(e => data.Devices.Any(d => d.Id == e.DeviceId)), "Every event must reference an existing device.");
Console.WriteLine($"Passed {assertions} assertions: demo login, filters, search, navigation targets, all device details and event consistency.");
