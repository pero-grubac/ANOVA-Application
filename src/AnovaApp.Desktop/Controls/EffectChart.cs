using System.Windows;
using System.Windows.Media;
using AnovaApp.Desktop.ViewModels;

namespace AnovaApp.Desktop.Controls;

/// <summary>
/// Horizontal bars of each alternative's effect around the grand mean:
/// to the right when the alternative is above it, to the left when below.
/// </summary>
public sealed class EffectChart : ChartElement
{
    private const double RowHeight = 34;
    private const double NameWidth = 120;
    private const double MeanWidth = 64;
    private const double BarHeight = 14;

    public static readonly DependencyProperty ItemsProperty = Register<IReadOnlyList<EffectBar>?>(typeof(EffectChart), nameof(Items), null,
        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure);
    public static readonly DependencyProperty PositiveBrushProperty = Register<Brush?>(typeof(EffectChart), nameof(PositiveBrush), null);
    public static readonly DependencyProperty NegativeBrushProperty = Register<Brush?>(typeof(EffectChart), nameof(NegativeBrush), null);

    public IReadOnlyList<EffectBar>? Items
    {
        get => (IReadOnlyList<EffectBar>?)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public Brush? PositiveBrush
    {
        get => (Brush?)GetValue(PositiveBrushProperty);
        set => SetValue(PositiveBrushProperty, value);
    }

    public Brush? NegativeBrush
    {
        get => (Brush?)GetValue(NegativeBrushProperty);
        set => SetValue(NegativeBrushProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize) =>
        new(double.IsInfinity(availableSize.Width) ? 360 : availableSize.Width, (Items?.Count ?? 0) * RowHeight);

    protected override void OnRender(DrawingContext dc)
    {
        var items = Items;
        if (items is null || items.Count == 0)
            return;

        double plotLeft = NameWidth;
        double plotRight = ActualWidth - MeanWidth;
        double center = (plotLeft + plotRight) / 2;
        double half = Math.Max(0, (plotRight - plotLeft) / 2 - 56);
        double maxEffect = items.Max(i => Math.Abs(i.Effect));

        dc.DrawLine(PenOf(AxisBrush, 1, dash: DashStyles.Dash), new Point(center, 0), new Point(center, items.Count * RowHeight));

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            double mid = i * RowHeight + RowHeight / 2;

            var name = Text(item.Name, Foreground, 13);
            name.MaxTextWidth = NameWidth - 12;
            name.MaxLineCount = 1;
            name.Trimming = TextTrimming.CharacterEllipsis;
            dc.DrawText(name, new Point(0, mid - name.Height / 2));

            double length = maxEffect == 0 ? 0 : Math.Abs(item.Effect) / maxEffect * half;
            bool positive = item.Effect >= 0;
            var bar = positive
                ? new Rect(center, mid - BarHeight / 2, length, BarHeight)
                : new Rect(center - length, mid - BarHeight / 2, length, BarHeight);
            dc.DrawRoundedRectangle(positive ? PositiveBrush : NegativeBrush, null, bar, 3, 3);

            var value = Text((positive ? "+" : "") + Format.Number(item.Effect), SecondaryForeground, 12);
            double valueX = positive ? bar.Right + 6 : bar.Left - 6 - value.Width;
            dc.DrawText(value, new Point(valueX, mid - value.Height / 2));

            var mean = Text(Format.Number(item.Mean), Foreground, 13, FontWeights.SemiBold);
            dc.DrawText(mean, new Point(ActualWidth - mean.Width, mid - mean.Height / 2));
        }
    }
}
