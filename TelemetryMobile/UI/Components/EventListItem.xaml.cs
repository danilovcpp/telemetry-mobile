namespace TelemetryMobile.UI.Components;
public partial class EventListItem : ContentView
{
    public EventListItem() => InitializeComponent();
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(EventListItem), string.Empty);
    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

    public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(nameof(Description), typeof(string), typeof(EventListItem), string.Empty);
    public string Description { get => (string)GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }

    public static readonly BindableProperty StatusTextProperty = BindableProperty.Create(nameof(StatusText), typeof(string), typeof(EventListItem), string.Empty);
    public string StatusText { get => (string)GetValue(StatusTextProperty); set => SetValue(StatusTextProperty, value); }

    public static readonly BindableProperty TimestampProperty = BindableProperty.Create(nameof(Timestamp), typeof(string), typeof(EventListItem), string.Empty);
    public string Timestamp { get => (string)GetValue(TimestampProperty); set => SetValue(TimestampProperty, value); }

    public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State), typeof(StatusState), typeof(EventListItem), StatusState.Unknown);
    public StatusState State { get => (StatusState)GetValue(StateProperty); set => SetValue(StateProperty, value); }

}
