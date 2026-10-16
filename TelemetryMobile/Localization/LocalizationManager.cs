using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace TelemetryMobile.Localization;

/// <summary>Bindings to the indexer refresh existing views when the language changes.</summary>
public sealed class LocalizationManager : INotifyPropertyChanged
{
    public static LocalizationManager Instance { get; } = new();
    private readonly ResourceManager resources = new("TelemetryMobile.Resources.Localization.AppResources", typeof(LocalizationManager).Assembly);
    public event PropertyChangedEventHandler? PropertyChanged;
    public CultureInfo Culture { get; private set; } = CultureInfo.GetCultureInfo("ru");
    public string this[string key] => resources.GetString(key, Culture) ?? key;

    public void Initialize()
    {
        var saved = Preferences.Default.Get("AppLanguage", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
        SetCulture(saved is "ru" or "en" or "de" ? saved : "en");
    }

    public void SetCulture(string language)
    {
        if (language is not ("ru" or "en" or "de"))
            throw new ArgumentOutOfRangeException(nameof(language));
        Culture = CultureInfo.GetCultureInfo(language);
        CultureInfo.CurrentCulture = Culture;
        CultureInfo.CurrentUICulture = Culture;
        CultureInfo.DefaultThreadCurrentCulture = Culture;
        CultureInfo.DefaultThreadCurrentUICulture = Culture;
        Preferences.Default.Set("AppLanguage", language);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Culture)));
        // MAUI indexer bindings must refresh for every translated key.
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
}
