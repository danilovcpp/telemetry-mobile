namespace TelemetryMobile.UI.Components;
public partial class SummaryStatCard : ContentView
{
    public SummaryStatCard() { InitializeComponent(); UpdateState(); }
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(string), typeof(SummaryStatCard), string.Empty);
    public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

    public static readonly BindableProperty CaptionProperty = BindableProperty.Create(nameof(Caption), typeof(string), typeof(SummaryStatCard), string.Empty);
    public string Caption { get => (string)GetValue(CaptionProperty); set => SetValue(CaptionProperty, value); }

    public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State), typeof(StatusState), typeof(SummaryStatCard), StatusState.Unknown, propertyChanged: (b, o, n) => ((SummaryStatCard)b).UpdateState());
    public StatusState State { get => (StatusState)GetValue(StateProperty); set => SetValue(StateProperty, value); }

    private void UpdateState()
    {
        if (ValueLabel is not null)
            ValueLabel.SetDynamicResource(Label.TextColorProperty, State == StatusState.Unknown ? "ColorTextPrimary" : StatusResources.ColorKey(State));
    }
}
