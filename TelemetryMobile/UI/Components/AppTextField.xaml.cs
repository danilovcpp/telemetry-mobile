using System.Windows.Input;
namespace TelemetryMobile.UI.Components;

public partial class AppTextField : ContentView
{
    public AppTextField() { InitializeComponent(); UpdateState(); }
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(AppTextField), string.Empty, defaultBindingMode: BindingMode.TwoWay);
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(AppTextField), string.Empty);
    public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

    public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(AppTextField), string.Empty, propertyChanged: OnVisualPropertyChanged);
    public string ErrorText { get => (string)GetValue(ErrorTextProperty); set => SetValue(ErrorTextProperty, value); }

    public static readonly BindableProperty LeadingIconProperty = BindableProperty.Create(nameof(LeadingIcon), typeof(ImageSource), typeof(AppTextField), null, propertyChanged: OnVisualPropertyChanged);
    public ImageSource LeadingIcon { get => (ImageSource)GetValue(LeadingIconProperty); set => SetValue(LeadingIconProperty, value); }

    public static readonly BindableProperty TrailingIconProperty = BindableProperty.Create(nameof(TrailingIcon), typeof(ImageSource), typeof(AppTextField), null, propertyChanged: OnVisualPropertyChanged);
    public ImageSource TrailingIcon { get => (ImageSource)GetValue(TrailingIconProperty); set => SetValue(TrailingIconProperty, value); }

    public static readonly BindableProperty TrailingCommandProperty = BindableProperty.Create(nameof(TrailingCommand), typeof(ICommand), typeof(AppTextField), null);
    public ICommand TrailingCommand { get => (ICommand)GetValue(TrailingCommandProperty); set => SetValue(TrailingCommandProperty, value); }

    public static readonly BindableProperty TrailingDescriptionProperty = BindableProperty.Create(nameof(TrailingDescription), typeof(string), typeof(AppTextField), string.Empty);
    public string TrailingDescription { get => (string)GetValue(TrailingDescriptionProperty); set => SetValue(TrailingDescriptionProperty, value); }

    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(AppTextField), false);
    public bool IsPassword { get => (bool)GetValue(IsPasswordProperty); set => SetValue(IsPasswordProperty, value); }

    public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(nameof(IsReadOnly), typeof(bool), typeof(AppTextField), false);
    public bool IsReadOnly { get => (bool)GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(AppTextField), Keyboard.Default);
    public Keyboard Keyboard { get => (Keyboard)GetValue(KeyboardProperty); set => SetValue(KeyboardProperty, value); }

    public static readonly BindableProperty ReturnTypeProperty = BindableProperty.Create(nameof(ReturnType), typeof(ReturnType), typeof(AppTextField), ReturnType.Next);
    public ReturnType ReturnType { get => (ReturnType)GetValue(ReturnTypeProperty); set => SetValue(ReturnTypeProperty, value); }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorText);
    public bool HasLeadingIcon => LeadingIcon is not null;
    public bool HasTrailingIcon => TrailingIcon is not null;
    public event EventHandler? Completed;
    public bool FocusInput() => Input.Focus();
    private void OnCompleted(object? sender, EventArgs e) => Completed?.Invoke(this, e);
    private void OnFocusChanged(object? sender, FocusEventArgs e) => UpdateState();
    private static void OnVisualPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var field = (AppTextField)bindable;
        field.OnPropertyChanged(nameof(HasError));
        field.OnPropertyChanged(nameof(HasLeadingIcon));
        field.OnPropertyChanged(nameof(HasTrailingIcon));
        field.UpdateState();
    }
    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == IsEnabledProperty.PropertyName) UpdateState();
    }
    private void UpdateState()
    {
        if (Container is not null)
            VisualStateManager.GoToState(Container, !IsEnabled ? "Disabled" : HasError ? "Error" : Input.IsFocused ? "Focused" : "Normal");
    }
    private void OnEntryHandlerChanged(object? sender, EventArgs e)
    {
        // Border owns the outline. No custom handler; only remove native Entry chrome.
#if ANDROID
        if (Input.Handler?.PlatformView is Android.Widget.EditText editText)
            editText.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif IOS || MACCATALYST
        if (Input.Handler?.PlatformView is UIKit.UITextField textField)
            textField.BorderStyle = UIKit.UITextBorderStyle.None;
#elif WINDOWS
        if (Input.Handler?.PlatformView is Microsoft.UI.Xaml.Controls.TextBox textBox)
        {
            textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            var resources = new Microsoft.UI.Xaml.ResourceDictionary();
            foreach (var key in new[] { "TextControlBackground", "TextControlBackgroundFocused", "TextControlBackgroundPointerOver", "TextControlBorderBrushFocused" })
                resources[key] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            textBox.Resources = resources;
        }
#endif
    }
}
