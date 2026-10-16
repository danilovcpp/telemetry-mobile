namespace TelemetryMobile.UI.Components;

/// <summary>Small vector chart of mock samples; scales with the available card width.</summary>
public sealed class Sparkline : GraphicsView, IDrawable
{
    public static readonly BindableProperty SamplesProperty = BindableProperty.Create(nameof(Samples), typeof(float[]), typeof(Sparkline), null,
        propertyChanged: (b, o, n) => ((Sparkline)b).Invalidate());
    public static readonly BindableProperty ColorKeyProperty = BindableProperty.Create(nameof(ColorKey), typeof(string), typeof(Sparkline), "ColorAccentPrimary",
        propertyChanged: (b, o, n) => ((Sparkline)b).SetDynamicResource(StrokeColorProperty, (string)n));
    public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(nameof(StrokeColor), typeof(Color), typeof(Sparkline), Colors.Transparent,
        propertyChanged: (b, o, n) => ((Sparkline)b).Invalidate());
    public float[]? Samples { get => (float[]?)GetValue(SamplesProperty); set => SetValue(SamplesProperty, value); }
    public string ColorKey { get => (string)GetValue(ColorKeyProperty); set => SetValue(ColorKeyProperty, value); }
    public Color StrokeColor { get => (Color)GetValue(StrokeColorProperty); set => SetValue(StrokeColorProperty, value); }

    public Sparkline()
    {
        Drawable = this;
        InputTransparent = true;
        SetDynamicResource(StrokeColorProperty, "ColorAccentPrimary");
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Samples is not { Length: > 1 } samples) return;
        var line = new PathF();
        var width = Math.Max(0, dirtyRect.Width - 4);
        var height = Math.Max(0, dirtyRect.Height - 8);
        for (var i = 0; i < samples.Length; i++)
        {
            var x = 2 + width * i / (samples.Length - 1);
            var y = 4 + height * (1 - Math.Clamp(samples[i], 0, 1));
            if (i == 0) line.MoveTo(x, y); else line.LineTo(x, y);
        }
        canvas.StrokeColor = StrokeColor;
        canvas.StrokeSize = 2;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.DrawPath(line);
        line.LineTo(2 + width, dirtyRect.Height);
        line.LineTo(2, dirtyRect.Height);
        line.Close();
        canvas.FillColor = StrokeColor.WithAlpha(0.08f);
        canvas.FillPath(line);
    }
}
