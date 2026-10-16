using System.Windows.Input;
namespace TelemetryMobile.UI.Components;
public partial class AppButton : ContentView
{
    public AppButton() => InitializeComponent();
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(AppButton), string.Empty);
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public static readonly BindableProperty BusyTextProperty = BindableProperty.Create(nameof(BusyText), typeof(string), typeof(AppButton), string.Empty);
    public string BusyText { get => (string)GetValue(BusyTextProperty); set => SetValue(BusyTextProperty, value); }

    public static readonly BindableProperty IsBusyProperty = BindableProperty.Create(nameof(IsBusy), typeof(bool), typeof(AppButton), false);
    public bool IsBusy { get => (bool)GetValue(IsBusyProperty); set => SetValue(IsBusyProperty, value); }

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(AppButton), null);
    public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(AppButton), null);
    public object CommandParameter { get => (object)GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

}
