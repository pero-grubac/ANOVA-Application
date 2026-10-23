namespace AnovaApp.Core;

/// <summary>A closed interval <c>[Lower, Upper]</c>.</summary>
public readonly record struct ConfidenceInterval(double Lower, double Upper)
{
    public bool Contains(double value) => value >= Lower && value <= Upper;

    public override string ToString() => $"[{Lower:F5} ; {Upper:F5}]";
}

/// <summary>The contrast <c>αa − αb</c> between two alternatives and its confidence interval.</summary>
public sealed record ContrastResult
{
    /// <summary>Zero-based index of the first alternative.</summary>
    public required int AlternativeA { get; init; }

    /// <summary>Zero-based index of the second alternative.</summary>
    public required int AlternativeB { get; init; }

    /// <summary>Estimated contrast, <c>αa − αb</c>.</summary>
    public required double Estimate { get; init; }

    /// <summary>Standard deviation of the contrast, <c>sc</c>.</summary>
    public required double StandardError { get; init; }

    /// <summary>Two-sided critical value of Student's t distribution.</summary>
    public required double TCritical { get; init; }

    public required ConfidenceInterval Interval { get; init; }

    public required double Confidence { get; init; }

    /// <summary>The two alternatives differ significantly: the interval does not contain zero.</summary>
    public bool IsSignificant => !Interval.Contains(0);
}
