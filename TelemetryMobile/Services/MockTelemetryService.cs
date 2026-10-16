using TelemetryMobile.Models;

namespace TelemetryMobile.Services;

/// <summary>Deterministic demo data. No network requests or persisted credentials.</summary>
public sealed class MockTelemetryService
{
    public IReadOnlyList<TelemetryDevice> Devices { get; }
    public IReadOnlyList<TelemetryEvent> Events { get; }

    public MockTelemetryService()
    {
        DeviceMetric Metric(string key, string value, string icon, string color, int seed) =>
            new(key, value, icon + ".png", color,
                Enumerable.Range(0, 28).Select(i => (float)(0.45 + 0.16 * Math.Sin(i * 0.7 + seed) + 0.1 * Math.Cos(i * 1.3 + seed))).ToArray());

        TelemetryDevice Device(string id, string name, string address, string icon, DeviceStatus status,
            string model, int seed) => new(id, name, address, icon + ".png", status, model, "v2.4.1",
                status == DeviceStatus.Online ? "09:41" : "08:17",
                status == DeviceStatus.Offline ? [] :
                icon == "temperature" ? [Metric("Temperature", "24°C", icon, "ColorStatusSuccess", seed), Metric("Humidity", "46%", "sensor", "ColorAccentPrimary", seed + 1)] :
                icon == "ups" ? [Metric("Battery", "98%", icon, "ColorStatusSuccess", seed), Metric("Load", "32%", "cpu", "ColorAccentPrimary", seed + 1)] :
                icon == "fan" ? [Metric("Temperature", "78°C", "temperature", "ColorStatusError", seed), Metric("FanSpeed", "820 rpm", icon, "ColorStatusWarning", seed + 1)] :
                [Metric("Cpu", id == "server-01" ? "12%" : "18%", "cpu", "ColorStatusSuccess", seed),
                 Metric("Memory", "38%", "memory", "ColorAccentPrimary", seed + 1),
                 Metric("Temperature", "42°C", "temperature", "ColorStatusWarning", seed + 2),
                 Metric("Network", "12 / 8", "network", "ColorChartPurple", seed + 3)]);

        var devices = new List<TelemetryDevice>
        {
            Device("server-01", "Сервер-01", "192.168.1.10", "server", DeviceStatus.Online, "Dell PowerEdge R640", 1),
            Device("camera-01", "Камера-01", "192.168.1.21", "camera", DeviceStatus.Online, "Hikvision DS-2CD", 2),
            Device("sensor-01", "Датчик температуры", "192.168.2.15", "temperature", DeviceStatus.Online, "Sensirion SHT35", 3),
            Device("controller-01", "Контроллер", "192.168.3.12", "cpu", DeviceStatus.Offline, "Siemens S7-1200", 4),
            Device("ventilation-01", "Вентиляция", "192.168.3.18", "fan", DeviceStatus.Error, "Vents VUT 350", 5),
            Device("ups-01", "UPS-01", "192.168.4.10", "ups", DeviceStatus.Online, "APC Smart-UPS 1500", 6)
        };
        for (var i = 2; i <= 11; i++)
            devices.Add(Device($"server-{i:00}", $"Сервер-{i:00}", $"192.168.1.{30 + i}", "server",
                i >= 10 ? DeviceStatus.Offline : DeviceStatus.Online, "Dell PowerEdge R640", i));
        Devices = devices.AsReadOnly();
        Events = Array.AsReadOnly(new[]
        {
            new TelemetryEvent("server-01", "DeviceOnline", "09:41", EventSeverity.Information, true),
            new TelemetryEvent("sensor-01", "TemperatureNormal", "09:32", EventSeverity.Information, true),
            new TelemetryEvent("controller-01", "ConnectionInterrupted", "08:17", EventSeverity.Warning),
            new TelemetryEvent("ventilation-01", "HighTemperature", "07:56", EventSeverity.Alarm),
            new TelemetryEvent("camera-01", "DeviceOnline", "06:12", EventSeverity.Information, true),
            new TelemetryEvent("server-01", "ScheduledRestart", "02:30", EventSeverity.Information),
            new TelemetryEvent("ups-01", "DeviceOnline", "01:14", EventSeverity.Information, true),
            new TelemetryEvent("ups-01", "PowerFailure", "23:48", EventSeverity.Alarm)
        });
    }
}
