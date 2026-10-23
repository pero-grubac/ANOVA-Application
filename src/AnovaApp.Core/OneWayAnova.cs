using MathNet.Numerics.Distributions;

namespace AnovaApp.Core;

/// <summary>One-factor analysis of variance with equal sample sizes, and contrasts between alternatives.</summary>
public static class OneWayAnova
{
    /// <summary>Allocates the variation of <paramref name="experiment"/> and runs the F-test.</summary>
    /// <param name="confidence">Confidence level, between 0 and 1 (exclusive), for example 0.95.</param>
    public static AnovaResult Analyze(Experiment experiment, double confidence)
    {
        ArgumentNullException.ThrowIfNull(experiment);
        ValidateConfidence(confidence);

        int k = experiment.AlternativeCount;
        int n = experiment.MeasurementCount;

        var means = new double[k];
        for (int j = 0; j < k; j++)
            means[j] = experiment[j].Average();

        double grandMean = means.Average();
        var effects = means.Select(m => m - grandMean).ToArray();

        double sse = 0;
        double sst = 0;
        for (int j = 0; j < k; j++)
        {
            foreach (double y in experiment[j])
            {
                sse += (y - means[j]) * (y - means[j]);
                sst += (y - grandMean) * (y - grandMean);
            }
        }

        // Computed directly instead of SST − SSE, which loses precision when SSA is tiny.
        double ssa = n * effects.Sum(a => a * a);

        int dfAlternatives = k - 1;
        int dfError = k * (n - 1);
        double msAlternatives = ssa / dfAlternatives;
        double msError = sse / dfError;

        double f = msError > 0 ? msAlternatives / msError
                 : ssa > 0 ? double.PositiveInfinity
                 : double.NaN;

        double pValue = double.IsNaN(f) ? 1
                      : double.IsPositiveInfinity(f) ? 0
                      : 1 - FisherSnedecor.CDF(dfAlternatives, dfError, f);

        return new AnovaResult
        {
            AlternativeCount = k,
            MeasurementCount = n,
            Means = means,
            GrandMean = grandMean,
            Effects = effects,
            Ssa = ssa,
            Sse = sse,
            Sst = sst,
            F = f,
            FCritical = FisherSnedecor.InvCDF(dfAlternatives, dfError, confidence),
            PValue = pValue,
            Confidence = confidence,
        };
    }

    /// <summary>Estimates the contrast <c>αa − αb</c> with a two-sided confidence interval.</summary>
    /// <param name="alternativeA">Zero-based index of the first alternative.</param>
    /// <param name="alternativeB">Zero-based index of the second alternative.</param>
    /// <param name="confidence">Confidence level, between 0 and 1 (exclusive), for example 0.90.</param>
    public static ContrastResult Contrast(AnovaResult anova, int alternativeA, int alternativeB, double confidence)
    {
        ArgumentNullException.ThrowIfNull(anova);
        ValidateConfidence(confidence);
        ArgumentOutOfRangeException.ThrowIfNegative(alternativeA);
        ArgumentOutOfRangeException.ThrowIfNegative(alternativeB);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(alternativeA, anova.AlternativeCount);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(alternativeB, anova.AlternativeCount);
        if (alternativeA == alternativeB)
            throw new ArgumentException("A contrast compares two different alternatives.", nameof(alternativeB));

        double estimate = anova.Effects[alternativeA] - anova.Effects[alternativeB];

        // Contrast weights are +1 and −1, so Σh² = 2 and Var(c) = se² · 2 / n.
        double standardError = Math.Sqrt(anova.MsError * 2.0 / anova.MeasurementCount);
        double tCritical = StudentT.InvCDF(0, 1, anova.DfError, 1 - (1 - confidence) / 2);
        double halfWidth = tCritical * standardError;

        return new ContrastResult
        {
            AlternativeA = alternativeA,
            AlternativeB = alternativeB,
            Estimate = estimate,
            StandardError = standardError,
            TCritical = tCritical,
            Interval = new ConfidenceInterval(estimate - halfWidth, estimate + halfWidth),
            Confidence = confidence,
        };
    }

    private static void ValidateConfidence(double confidence)
    {
        if (!(confidence > 0 && confidence < 1))
            throw new ArgumentOutOfRangeException(nameof(confidence), confidence, "Confidence must be between 0 and 1.");
    }
}
