using TelemetryMobile.Models;

namespace TelemetryMobile.Services;

/// <summary>Merges per-device cursor histories into one globally ordered, bounded feed.</summary>
public sealed class DeviceEventFeed(ITelemetryApi api, IReadOnlyList<DeviceListItem> devices)
{
    private sealed class History(DeviceListItem device)
    {
        public DeviceListItem Device { get; } = device;
        public Queue<TelemetryEvent> Buffer { get; } = new();
        public string? Cursor { get; set; }
        public bool HasMore { get; set; } = true;
    }

    private readonly History[] histories = devices.Select(d => new History(d)).ToArray();
    public bool HasMore => histories.Any(h => h.Buffer.Count > 0 || h.HasMore);

    public async Task<IReadOnlyList<DeviceEvent>> LoadMoreAsync(CancellationToken cancellationToken = default)
    {
        var result = new List<DeviceEvent>();
        // Only commit consumed events after a complete page, so retrying a failed request loses nothing.
        var consumed = histories.ToDictionary(h => h, _ => 0);
        while (result.Count < 20)
        {
            await Task.WhenAll(histories.Where(h => h.Buffer.Count == consumed[h] && h.HasMore).Select(async history =>
            {
                var page = await api.GetEventsAsync(history.Device.Id, history.Cursor, cancellationToken: cancellationToken);
                if (page.HasMore && (string.IsNullOrEmpty(page.NextCursor) || page.NextCursor == history.Cursor || page.Items.Count == 0))
                    throw new System.Text.Json.JsonException("Invalid event pagination response.");
                foreach (var entry in page.Items) history.Buffer.Enqueue(entry);
                history.Cursor = page.NextCursor;
                history.HasMore = page.HasMore;
            }));
            var next = histories.Where(h => h.Buffer.Count > consumed[h])
                .OrderByDescending(h => h.Buffer.ElementAt(consumed[h]).OccurredAtUtc)
                .ThenByDescending(h => h.Buffer.ElementAt(consumed[h]).Id).FirstOrDefault();
            if (next is null) break;
            result.Add(new DeviceEvent(next.Device, next.Buffer.ElementAt(consumed[next]++)));
        }
        foreach (var history in histories)
            for (var i = 0; i < consumed[history]; i++) history.Buffer.Dequeue();
        return result;
    }
}
