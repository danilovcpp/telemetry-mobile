using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Core;
using TelemetryMobile.Resources.Styles.Themes;

namespace TelemetryMobile.Theming;

/// <summary>Applies the semantic palette and persists the user's explicit choice.</summary>
public sealed class ThemeManager : ObservableObject
{
    private const string PreferenceKey = "AppTheme";
    public static ThemeManager Instance { get; } = new();
    private Application? application;
    private ResourceDictionary? styles;
    private ResourceDictionary? palette;
    private bool isLightTheme;

    private ThemeManager() { }

    // Behaviors have no resource ancestry, so bind their colors explicitly.
    public Color IconTintColor => (Color)(palette?["ColorTextSecondary"] ?? Colors.Transparent);
    public Color PageBackgroundColor => (Color)(palette?["ColorPageBackground"] ?? Colors.Transparent);
    public StatusBarStyle StatusBarStyle => IsLightTheme ? StatusBarStyle.DarkContent : StatusBarStyle.LightContent;

    public bool IsLightTheme
    {
        get => isLightTheme;
        set
        {
            if (!SetProperty(ref isLightTheme, value))
                return;
            ApplyTheme();
            Preferences.Default.Set(PreferenceKey, value ? "Light" : "Dark");
        }
    }

    public void Initialize(Application app)
    {
        application = app;
        styles = app.Resources;
        isLightTheme = Preferences.Default.Get(PreferenceKey, "Dark") == "Light";
        ApplyTheme();
        OnPropertyChanged(nameof(IsLightTheme));
    }

    private void ApplyTheme()
    {
        if (application is null || styles is null)
            return;

        ResourceDictionary next = IsLightTheme ? new LightTheme() : new DarkTheme();
        // Add first so dynamic resources always have a complete palette to resolve.
        styles.MergedDictionaries.Add(next);
        if (palette is not null)
            styles.MergedDictionaries.Remove(palette);
        palette = next;
        application.UserAppTheme = IsLightTheme ? AppTheme.Light : AppTheme.Dark;
        OnPropertyChanged(nameof(IconTintColor));
        OnPropertyChanged(nameof(PageBackgroundColor));
        OnPropertyChanged(nameof(StatusBarStyle));
    }
}
