namespace AnovaApp.Core;

/// <summary>Allocation of variation and the F-test of a one-factor experiment.</summary>
public sealed record AnovaResult
{
    public required int AlternativeCount { get; init; }
    public required int MeasurementCount { get; init; }

    /// <summary>Column means, <c>ȳ.j</c>.</summary>
    public required IReadOnlyList<double> Means { get; init; }

    /// <summary>Grand mean, <c>ȳ..</c>.</summary>
    public required double GrandMean { get; init; }

    /// <summary>Effect of each alternative, <c>αj = ȳ.j − ȳ..</c>.</summary>
    public required IReadOnlyList<double> Effects { get; init; }

    /// <summary>Variation explained by the alternatives.</summary>
    public required double Ssa { get; init; }

    /// <summary>Variation left to error.</summary>
    public required double Sse { get; init; }

    /// <summary>Total variation, <c>SSA + SSE</c>.</summary>
    public required double Sst { get; init; }

    public int DfAlternatives => AlternativeCount - 1;
    public int DfError => AlternativeCount * (MeasurementCount - 1);
    public int DfTotal => AlternativeCount * MeasurementCount - 1;

    /// <summary>Mean square of the alternatives, <c>sa²</c>.</summary>
    public double MsAlternatives => Ssa / DfAlternatives;

    /// <summary>Mean square of the error, <c>se²</c>.</summary>
    public double MsError => Sse / DfError;

    /// <summary>Computed F statistic, <c>sa² / se²</c>.</summary>
    public required double F { get; init; }

    /// <summary>Critical value of the F distribution at <see cref="Confidence"/>.</summary>
    public required double FCritical { get; init; }

    /// <summary>Probability of an F at least this large if all alternatives were the same.</summary>
    public required double PValue { get; init; }

    /// <summary>Confidence level of the test, for example 0.95.</summary>
    public required double Confidence { get; init; }

    /// <summary>The alternatives differ significantly at <see cref="Confidence"/>.</summary>
    public bool IsSignificant => F > FCritical;

    /// <summary>Share of the total variation explained by the alternatives (0–1).</summary>
    public double ExplainedVariation => Sst == 0 ? 0 : Ssa / Sst;
}
