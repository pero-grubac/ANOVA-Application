using AnovaApp.Core;

namespace AnovaApp.Desktop.ViewModels;

/// <summary>A row of the ANOVA table, already formatted for display.</summary>
public sealed record AnovaTableRow(string Source, string SumOfSquares, string DegreesOfFreedom, string MeanSquare, string F);

/// <summary>The mean and effect of one alternative, for the effects chart.</summary>
public sealed record EffectBar(string Name, double Mean, double Effect);

/// <summary>Display model of an <see cref="AnovaResult"/>.</summary>
public sealed class AnovaSummary(AnovaResult result, IReadOnlyList<string> names)
{
    public AnovaResult Result { get; } = result;

    public bool IsSignificant => Result.IsSignificant;

    public string Verdict => IsSignificant
        ? "The alternatives differ significantly"
        : "No significant difference between the alternatives";

    public string VerdictDetail => IsSignificant
        ? $"F = {Format.Number(Result.F)} is above the critical {Format.Number(Result.FCritical)} at {Format.Confidence(Result.Confidence)} confidence."
        : $"F = {Format.Number(Result.F)} does not exceed the critical {Format.Number(Result.FCritical)} at {Format.Confidence(Result.Confidence)} confidence.";

    public string FText => Format.Number(Result.F);
    public string FCriticalText => Format.Number(Result.FCritical);
    public string PValueText => Format.PValue(Result.PValue);
    public string ExplainedText => Format.Percent(Result.ExplainedVariation);
    public string GrandMeanText => Format.Number(Result.GrandMean);

    public IReadOnlyList<AnovaTableRow> Table { get; } =
    [
        new("Alternatives", Format.Number(result.Ssa), result.DfAlternatives.ToString(), Format.Number(result.MsAlternatives), Format.Number(result.F)),
        new("Error", Format.Number(result.Sse), result.DfError.ToString(), Format.Number(result.MsError), ""),
        new("Total", Format.Number(result.Sst), result.DfTotal.ToString(), "", ""),
    ];

    public IReadOnlyList<EffectBar> Effects { get; } =
        names.Select((name, j) => new EffectBar(name, result.Means[j], result.Effects[j])).ToArray();
}

/// <summary>Display model of a <see cref="ContrastResult"/>.</summary>
public sealed class ContrastSummary(ContrastResult result, string nameA, string nameB)
{
    public ContrastResult Result { get; } = result;

    public bool IsSignificant => Result.IsSignificant;

    public string Title => $"{nameA} − {nameB}";

    public string Verdict => IsSignificant
        ? $"{(Result.Estimate > 0 ? nameA : nameB)} is significantly higher"
        : "The difference is not significant";

    public string VerdictDetail => IsSignificant
        ? $"The {Format.Confidence(Result.Confidence)} interval does not contain zero."
        : $"The {Format.Confidence(Result.Confidence)} interval contains zero.";

    public string EstimateText => Format.Number(Result.Estimate);
    public string StandardErrorText => Format.Number(Result.StandardError);
    public string TCriticalText => Format.Number(Result.TCritical);
    public string IntervalText => $"[{Format.Number(Result.Interval.Lower)} ; {Format.Number(Result.Interval.Upper)}]";
}
