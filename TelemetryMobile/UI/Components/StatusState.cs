namespace TelemetryMobile.UI.Components;

/// <summary>Presentation state only; domain models map to it in the page model.</summary>
public enum StatusState { Unknown, Online, Offline, Warning, Error }

internal static class StatusResources
{
    public static string ColorKey(StatusState state) => state switch
    {
        StatusState.Online => "ColorStatusSuccess",
        StatusState.Offline => "ColorStatusOffline",
        StatusState.Warning => "ColorStatusWarning",
        StatusState.Error => "ColorStatusError",
        _ => "ColorTextMuted"
    };
}
