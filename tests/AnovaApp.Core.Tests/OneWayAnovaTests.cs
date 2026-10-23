using AnovaApp.Core;

namespace AnovaApp.Core.Tests;

public class OneWayAnovaTests
{
    // Means 5, 9 and 10, grand mean 8, effects −3, 1 and 2.
    // SSE = 16 + 24 + 28 = 68, SSA = 6 · (9 + 1 + 4) = 84, SST = 152.
    private static readonly Experiment Example = Experiment.FromColumns(
        [6, 8, 4, 5, 3, 4],
        [8, 12, 9, 11, 6, 8],
        [13, 9, 11, 8, 7, 12]);

    [Fact]
    public void Analyze_ComputesMeansAndEffects()
    {
        var result = OneWayAnova.Analyze(Example, 0.95);

        Assert.Equal([5, 9, 10], result.Means.ToArray(), Precision(1e-12));
        Assert.Equal(8, result.GrandMean, 12);
        Assert.Equal([-3, 1, 2], result.Effects.ToArray(), Precision(1e-12));
    }

    [Fact]
    public void Analyze_AllocatesVariation()
    {
        var result = OneWayAnova.Analyze(Example, 0.95);

        Assert.Equal(84, result.Ssa, 10);
        Assert.Equal(68, result.Sse, 10);
        Assert.Equal(152, result.Sst, 10);
        Assert.Equal(result.Sst, result.Ssa + result.Sse, 10);
        Assert.Equal(84.0 / 152, result.ExplainedVariation, 10);
    }

    [Fact]
    public void Analyze_ComputesDegreesOfFreedomAndMeanSquares()
    {
        var result = OneWayAnova.Analyze(Example, 0.95);

        Assert.Equal(2, result.DfAlternatives);
        Assert.Equal(15, result.DfError);
        Assert.Equal(17, result.DfTotal);
        Assert.Equal(42, result.MsAlternatives, 10);
        Assert.Equal(68.0 / 15, result.MsError, 10);
    }

    [Fact]
    public void Analyze_RunsFTest()
    {
        var result = OneWayAnova.Analyze(Example, 0.95);

        Assert.Equal(42 / (68.0 / 15), result.F, 10);
        Assert.Equal(3.68232, result.FCritical, 4);
        Assert.Equal(0.0024, result.PValue, 4);
        Assert.True(result.IsSignificant);
    }

    [Fact]
    public void Analyze_HigherConfidenceRaisesCriticalValue()
    {
        var at95 = OneWayAnova.Analyze(Example, 0.95);
        var at99 = OneWayAnova.Analyze(Example, 0.99);

        Assert.Equal(6.35887, at99.FCritical, 4);
        Assert.True(at99.FCritical > at95.FCritical);
        Assert.True(at99.IsSignificant);
    }

    [Fact]
    public void Analyze_IdenticalAlternativesAreNotSignificant()
    {
        var experiment = Experiment.FromColumns([1, 2, 3], [1, 2, 3], [1, 2, 3]);

        var result = OneWayAnova.Analyze(experiment, 0.95);

        Assert.Equal(0, result.Ssa, 12);
        Assert.Equal(0, result.F, 12);
        Assert.Equal(1, result.PValue, 12);
        Assert.False(result.IsSignificant);
    }

    [Fact]
    public void Analyze_NoErrorVariation_GivesInfiniteF()
    {
        var experiment = Experiment.FromColumns([1, 1], [2, 2]);

        var result = OneWayAnova.Analyze(experiment, 0.95);

        Assert.True(double.IsPositiveInfinity(result.F));
        Assert.Equal(0, result.PValue);
        Assert.True(result.IsSignificant);
    }

    [Fact]
    public void Analyze_ConstantData_IsNotSignificant()
    {
        var experiment = Experiment.FromColumns([4, 4], [4, 4]);

        var result = OneWayAnova.Analyze(experiment, 0.95);

        Assert.True(double.IsNaN(result.F));
        Assert.Equal(1, result.PValue);
        Assert.False(result.IsSignificant);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(95)]
    [InlineData(-0.5)]
    [InlineData(double.NaN)]
    public void Analyze_RejectsInvalidConfidence(double confidence)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => OneWayAnova.Analyze(Example, confidence));
    }

    [Fact]
    public void Contrast_EstimatesDifferenceOfEffects()
    {
        var anova = OneWayAnova.Analyze(Example, 0.95);

        var contrast = OneWayAnova.Contrast(anova, 0, 1, 0.95);

        // c = α1 − α2 = −4, sc = se · √(2/n), t[0.975; 15] = 2.13145.
        double se = Math.Sqrt(68.0 / 15);
        double sc = se * Math.Sqrt(2.0 / 6);
        Assert.Equal(-4, contrast.Estimate, 10);
        Assert.Equal(sc, contrast.StandardError, 10);
        Assert.Equal(2.13145, contrast.TCritical, 4);
        Assert.Equal(-4 - contrast.TCritical * sc, contrast.Interval.Lower, 10);
        Assert.Equal(-4 + contrast.TCritical * sc, contrast.Interval.Upper, 10);
        Assert.True(contrast.IsSignificant);
    }

    [Fact]
    public void Contrast_OverlappingAlternativesAreNotSignificant()
    {
        var anova = OneWayAnova.Analyze(Example, 0.95);

        var contrast = OneWayAnova.Contrast(anova, 1, 2, 0.95);

        Assert.Equal(-1, contrast.Estimate, 10);
        Assert.True(contrast.Interval.Contains(0));
        Assert.False(contrast.IsSignificant);
    }

    [Fact]
    public void Contrast_IsAntisymmetric()
    {
        var anova = OneWayAnova.Analyze(Example, 0.95);

        var ab = OneWayAnova.Contrast(anova, 0, 2, 0.90);
        var ba = OneWayAnova.Contrast(anova, 2, 0, 0.90);

        Assert.Equal(-ab.Estimate, ba.Estimate, 10);
        Assert.Equal(-ab.Interval.Upper, ba.Interval.Lower, 10);
    }

    [Fact]
    public void Contrast_HigherConfidenceWidensInterval()
    {
        var anova = OneWayAnova.Analyze(Example, 0.95);

        var narrow = OneWayAnova.Contrast(anova, 0, 1, 0.90);
        var wide = OneWayAnova.Contrast(anova, 0, 1, 0.99);

        Assert.True(wide.Interval.Upper - wide.Interval.Lower > narrow.Interval.Upper - narrow.Interval.Lower);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-1, 1)]
    [InlineData(0, 3)]
    public void Contrast_RejectsInvalidAlternatives(int a, int b)
    {
        var anova = OneWayAnova.Analyze(Example, 0.95);

        Assert.ThrowsAny<ArgumentException>(() => OneWayAnova.Contrast(anova, a, b, 0.95));
    }

    private static Comparer Precision(double tolerance) => new(tolerance);

    private sealed class Comparer(double tolerance) : IEqualityComparer<double>
    {
        public bool Equals(double x, double y) => Math.Abs(x - y) <= tolerance;
        public int GetHashCode(double obj) => 0;
    }
}
