using System.Windows.Input;
namespace TelemetryMobile.UI.Components;
public partial class SectionHeader : ContentView
{
    public SectionHeader() => InitializeComponent();
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(SectionHeader), string.Empty);
    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

    public static readonly BindableProperty ActionTextProperty = BindableProperty.Create(nameof(ActionText), typeof(string), typeof(SectionHeader), string.Empty, propertyChanged: (b, o, n) => ((SectionHeader)b).OnPropertyChanged(nameof(HasAction)));
    public string ActionText { get => (string)GetValue(ActionTextProperty); set => SetValue(ActionTextProperty, value); }

    public static readonly BindableProperty ActionCommandProperty = BindableProperty.Create(nameof(ActionCommand), typeof(ICommand), typeof(SectionHeader), null);
    public ICommand ActionCommand { get => (ICommand)GetValue(ActionCommandProperty); set => SetValue(ActionCommandProperty, value); }

    public static readonly BindableProperty ActionParameterProperty = BindableProperty.Create(nameof(ActionParameter), typeof(object), typeof(SectionHeader), null);
    public object ActionParameter { get => (object)GetValue(ActionParameterProperty); set => SetValue(ActionParameterProperty, value); }

    public bool HasAction => !string.IsNullOrWhiteSpace(ActionText);
}
