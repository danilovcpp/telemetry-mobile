using System.Windows.Input;
using TelemetryMobile.Localization;

namespace TelemetryMobile.Pages.Development;

/// <summary>Only compiled in Debug; opt in with DesignSystemShowcase=true.</summary>
public partial class DesignSystemShowcasePage : ContentPage
{
    public DesignSystemShowcasePage()
    {
        InitializeComponent();
        LanguagePicker.SelectedIndex = LocalizationManager.Instance.Culture.TwoLetterISOLanguageName switch { "en" => 1, "de" => 2, _ => 0 };
        ICommand feedback = new Command(() => Feedback.Text = LocalizationManager.Instance["SampleEvent"]);
        SampleDevice.Command = feedback;
        EventsHeader.ActionCommand = feedback;
    }
    private void OnButtonClicked(object? sender, EventArgs e) => Feedback.Text = LocalizationManager.Instance["SampleEvent"];
    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        if (LanguagePicker.SelectedIndex >= 0)
            LocalizationManager.Instance.SetCulture(new[] { "ru", "en", "de" }[LanguagePicker.SelectedIndex]);
    }
}
