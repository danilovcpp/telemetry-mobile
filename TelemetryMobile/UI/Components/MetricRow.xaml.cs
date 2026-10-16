namespace TelemetryMobile.UI.Components;
public partial class MetricRow : ContentView
{
    public MetricRow() => InitializeComponent();
    public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(MetricRow), string.Empty);
    public string Name { get => (string)GetValue(NameProperty); set => SetValue(NameProperty, value); }

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(string), typeof(MetricRow), string.Empty);
    public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

    public static readonly BindableProperty UnitProperty = BindableProperty.Create(nameof(Unit), typeof(string), typeof(MetricRow), string.Empty);
    public string Unit { get => (string)GetValue(UnitProperty); set => SetValue(UnitProperty, value); }

    public static readonly BindableProperty TrendProperty = BindableProperty.Create(nameof(Trend), typeof(string), typeof(MetricRow), string.Empty);
    public string Trend { get => (string)GetValue(TrendProperty); set => SetValue(TrendProperty, value); }

    public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(ImageSource), typeof(MetricRow), null);
    public ImageSource Icon { get => (ImageSource)GetValue(IconProperty); set => SetValue(IconProperty, value); }

}
