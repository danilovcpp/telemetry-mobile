using System.Windows.Input;
namespace TelemetryMobile.UI.Components;
public partial class DeviceCard : ContentView
{
    public DeviceCard() => InitializeComponent();
    public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(ImageSource), typeof(DeviceCard), null);
    public ImageSource Icon { get => (ImageSource)GetValue(IconProperty); set => SetValue(IconProperty, value); }

    public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(DeviceCard), string.Empty);
    public string Name { get => (string)GetValue(NameProperty); set => SetValue(NameProperty, value); }

    public static readonly BindableProperty SecondaryTextProperty = BindableProperty.Create(nameof(SecondaryText), typeof(string), typeof(DeviceCard), string.Empty);
    public string SecondaryText { get => (string)GetValue(SecondaryTextProperty); set => SetValue(SecondaryTextProperty, value); }

    public static readonly BindableProperty StatusTextProperty = BindableProperty.Create(nameof(StatusText), typeof(string), typeof(DeviceCard), string.Empty);
    public string StatusText { get => (string)GetValue(StatusTextProperty); set => SetValue(StatusTextProperty, value); }

    public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State), typeof(StatusState), typeof(DeviceCard), StatusState.Unknown);
    public StatusState State { get => (StatusState)GetValue(StateProperty); set => SetValue(StateProperty, value); }

    public static readonly BindableProperty ShowNavigationProperty = BindableProperty.Create(nameof(ShowNavigation), typeof(bool), typeof(DeviceCard), false);
    public bool ShowNavigation { get => (bool)GetValue(ShowNavigationProperty); set => SetValue(ShowNavigationProperty, value); }

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(DeviceCard), null, propertyChanged: (b, o, n) => ((DeviceCard)b).OnPropertyChanged(nameof(IsInteractive)));
    public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(DeviceCard), null);
    public object CommandParameter { get => (object)GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

    public bool IsInteractive => Command is not null;
}
