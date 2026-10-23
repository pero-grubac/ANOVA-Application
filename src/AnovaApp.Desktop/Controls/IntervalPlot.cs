using System.Windows;
using System.Windows.Media;
using AnovaApp.Desktop.ViewModels;

namespace AnovaApp.Desktop.Controls;

/// <summary>
/// A confidence interval on a number line with zero marked, so it is visible at a glance
/// whether the interval contains zero.
/// </summary>
public sealed class IntervalPlot : ChartElement
{
    public static readonly DependencyProperty LowerProperty = Register(typeof(IntervalPlot), nameof(Lower), 0.0);
    public static readonly DependencyProperty UpperProperty = Register(typeof(IntervalPlot), nameof(Upper), 0.0);
    public static readonly DependencyProperty EstimateProperty = Register(typeof(IntervalPlot), nameof(Estimate), 0.0);
    public static readonly DependencyProperty IntervalBrushProperty = Register<Brush?>(typeof(IntervalPlot), nameof(IntervalBrush), null);

    public double Lower
    {
        get => (double)GetValue(LowerProperty);
        set => SetValue(LowerProperty, value);
    }

    public double Upper
    {
        get => (double)GetValue(UpperProperty);
        set => SetValue(UpperProperty, value);
    }

    public double Estimate
    {
        get => (double)GetValue(EstimateProperty);
        set => SetValue(EstimateProperty, value);
    }

    public Brush? IntervalBrush
    {
        get => (Brush?)GetValue(IntervalBrushProperty);
        set => SetValue(IntervalBrushProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize) =>
        new(double.IsInfinity(availableSize.Width) ? 360 : availableSize.Width, 68);

    protected override void OnRender(DrawingContext dc)
    {
        if (!double.IsFinite(Lower) || !double.IsFinite(Upper) || ActualWidth < 40)
            return;

        double min = Math.Min(Lower, 0);
        double max = Math.Max(Upper, 0);
        double span = max - min;
        if (span == 0)
        {
            min -= 1;
            max += 1;
            span = 2;
        }
        min -= span * 0.12;
        max += span * 0.12;

        const double margin = 8;
        double X(double value) => margin + (value - min) / (max - min) * (ActualWidth - 2 * margin);

        const double axisY = 32;
        dc.DrawLine(PenOf(AxisBrush, 1), new Point(margin, axisY), new Point(ActualWidth - margin, axisY));

        double zero = X(0);
        dc.DrawLine(PenOf(SecondaryForeground, 1.5, dash: DashStyles.Dash), new Point(zero, 17), new Point(zero, axisY + 12));
        var zeroLabel = Text("0", SecondaryForeground, 11);
        dc.DrawText(zeroLabel, new Point(zero - zeroLabel.Width / 2, 0));

        double left = X(Lower);
        double right = X(Upper);
        dc.DrawLine(PenOf(IntervalBrush, 8, PenLineCap.Round), new Point(left, axisY), new Point(right, axisY));
        dc.DrawEllipse(Foreground, PenOf(IntervalBrush, 3), new Point(X(Estimate), axisY), 6, 6);

        var lowerLabel = Text(Format.Number(Lower), Foreground, 12);
        var upperLabel = Text(Format.Number(Upper), Foreground, 12);
        double lowerX = Math.Max(0, left - lowerLabel.Width / 2);
        double upperX = Math.Min(ActualWidth - upperLabel.Width, right - upperLabel.Width / 2);
        if (upperX < lowerX + lowerLabel.Width + 8)
            upperX = lowerX + lowerLabel.Width + 8;

        dc.DrawText(lowerLabel, new Point(lowerX, axisY + 16));
        dc.DrawText(upperLabel, new Point(upperX, axisY + 16));
    }
}
