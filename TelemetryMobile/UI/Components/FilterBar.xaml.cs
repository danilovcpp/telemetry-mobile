using System.Collections;
namespace TelemetryMobile.UI.Components;

public partial class FilterBar : ContentView
{
    public FilterBar() => InitializeComponent();
    public static readonly BindableProperty ItemsProperty = BindableProperty.Create(nameof(Items), typeof(IEnumerable), typeof(FilterBar), null);
    public IEnumerable Items { get => (IEnumerable)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }
}
