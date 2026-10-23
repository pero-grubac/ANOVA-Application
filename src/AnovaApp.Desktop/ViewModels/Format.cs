using System.Globalization;

namespace AnovaApp.Desktop.ViewModels;

/// <summary>Number formatting shared by the views, independent of the system locale.</summary>
internal static class Format
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    public static string Number(double value)
    {
        if (double.IsNaN(value)) return "—";
        if (double.IsPositiveInfinity(value)) return "∞";
        if (double.IsNegativeInfinity(value)) return "−∞";

        double magnitude = Math.Abs(value);
        string text = magnitude != 0 && (magnitude >= 1e6 || magnitude < 1e-4)
            ? value.ToString("0.###E+0", Culture)
            : value.ToString("0.####", Culture);
        return text.Replace('-', '−');
    }

    public static string PValue(double p) =>
        double.IsNaN(p) ? "—" : p < 0.0001 ? "< 0.0001" : p.ToString("0.0000", Culture);

    public static string Percent(double fraction) => (fraction * 100).ToString("0.#", Culture) + "%";

    /// <summary>A confidence level keeps every decimal the user typed, for example 99.95%.</summary>
    public static string Confidence(double fraction) => (fraction * 100).ToString("0.####", Culture) + "%";

    /// <summary>Lowest confidence accepted; anything lower is almost certainly a typo.</summary>
    public const double MinConfidence = 0.5;

    /// <summary>
    /// Parses a confidence level written as a percentage (<c>97.5</c>, <c>97.5%</c>) or a fraction (<c>0.975</c>).
    /// Returns <see langword="null"/> unless it is at least 50% and below 100%.
    /// </summary>
    public static double? ParseConfidence(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        string number = text.Trim().TrimEnd('%').Trim().Replace(',', '.');
        if (!double.TryParse(number, NumberStyles.Float, Culture, out double value) || !double.IsFinite(value))
            return null;

        double confidence = value >= 1 ? value / 100 : value;
        return confidence >= MinConfidence && confidence < 1 ? confidence : null;
    }

    /// <summary>Parses a measurement typed with either a decimal point or a decimal comma.</summary>
    public static double? ParseMeasurement(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        return double.TryParse(text.Trim().Replace(',', '.').Replace('−', '-'), NumberStyles.Float, Culture, out double value)
               && double.IsFinite(value)
            ? value
            : null;
    }

    public static string Measurement(double value) => value.ToString("R", Culture);
}
