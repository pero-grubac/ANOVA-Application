using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace AnovaApp.Desktop.Controls;

/// <summary>Base for the small charts: shared brushes and text drawing.</summary>
public abstract class ChartElement : FrameworkElement
{
    public static readonly DependencyProperty ForegroundProperty = Register<Brush?>(typeof(ChartElement), nameof(Foreground), null);
    public static readonly DependencyProperty SecondaryForegroundProperty = Register<Brush?>(typeof(ChartElement), nameof(SecondaryForeground), null);
    public static readonly DependencyProperty AxisBrushProperty = Register<Brush?>(typeof(ChartElement), nameof(AxisBrush), null);

    public Brush? Foreground
    {
        get => (Brush?)GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public Brush? SecondaryForeground
    {
        get => (Brush?)GetValue(SecondaryForegroundProperty);
        set => SetValue(SecondaryForegroundProperty, value);
    }

    public Brush? AxisBrush
    {
        get => (Brush?)GetValue(AxisBrushProperty);
        set => SetValue(AxisBrushProperty, value);
    }

    protected static DependencyProperty Register<T>(Type owner, string name, T defaultValue,
        FrameworkPropertyMetadataOptions options = FrameworkPropertyMetadataOptions.AffectsRender) =>
        DependencyProperty.Register(name, typeof(T), owner, new FrameworkPropertyMetadata(defaultValue, options));

    protected FormattedText Text(string text, Brush? brush, double size = 12, FontWeight? weight = null) =>
        new(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
            new Typeface(new FontFamily("Segoe UI Variable Text, Segoe UI"), FontStyles.Normal, weight ?? FontWeights.Normal, FontStretches.Normal),
            size, brush ?? Brushes.Gray, VisualTreeHelper.GetDpi(this).PixelsPerDip);

    protected static Pen PenOf(Brush? brush, double thickness, PenLineCap cap = PenLineCap.Flat, DashStyle? dash = null) =>
        new(brush ?? Brushes.Gray, thickness) { StartLineCap = cap, EndLineCap = cap, DashStyle = dash ?? DashStyles.Solid };
}
