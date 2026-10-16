namespace TelemetryMobile.UI.Components;
public partial class StatusBadge : ContentView
{
    public StatusBadge() { InitializeComponent(); UpdateState(); }
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(StatusBadge), string.Empty);
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State), typeof(StatusState), typeof(StatusBadge), StatusState.Unknown, propertyChanged: (b, o, n) => ((StatusBadge)b).UpdateState());
    public StatusState State { get => (StatusState)GetValue(StateProperty); set => SetValue(StateProperty, value); }

    private void UpdateState()
    {
        if (Dot is null) return;
        Dot.SetDynamicResource(Shape.FillProperty, StatusResources.ColorKey(State));
        StatusLabel.SetDynamicResource(Label.TextColorProperty, StatusResources.ColorKey(State));
    }
}
