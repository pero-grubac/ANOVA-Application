namespace AnovaApp.Core;

/// <summary>
/// A one-factor experiment: <c>k</c> alternatives, each measured the same number of times (<c>n</c>).
/// </summary>
public sealed class Experiment
{
    private readonly double[][] _measurements;

    /// <param name="alternativeNames">One name per alternative.</param>
    /// <param name="measurements">One array per alternative, all of the same length.</param>
    public Experiment(IReadOnlyList<string> alternativeNames, IReadOnlyList<IReadOnlyList<double>> measurements)
    {
        ArgumentNullException.ThrowIfNull(alternativeNames);
        ArgumentNullException.ThrowIfNull(measurements);

        if (measurements.Count < 2)
            throw new ArgumentException("ANOVA needs at least two alternatives.", nameof(measurements));
        if (alternativeNames.Count != measurements.Count)
            throw new ArgumentException("Every alternative needs exactly one name.", nameof(alternativeNames));

        var n = measurements[0].Count;
        if (n < 2)
            throw new ArgumentException("Every alternative needs at least two measurements.", nameof(measurements));
        if (measurements.Any(m => m.Count != n))
            throw new ArgumentException("Every alternative must have the same number of measurements.", nameof(measurements));
        if (measurements.SelectMany(m => m).Any(v => !double.IsFinite(v)))
            throw new ArgumentException("Measurements must be finite numbers.", nameof(measurements));

        AlternativeNames = alternativeNames.ToArray();
        _measurements = measurements.Select(m => m.ToArray()).ToArray();
    }

    /// <summary>Number of alternatives (<c>k</c>).</summary>
    public int AlternativeCount => _measurements.Length;

    /// <summary>Measurements per alternative (<c>n</c>).</summary>
    public int MeasurementCount => _measurements[0].Length;

    public IReadOnlyList<string> AlternativeNames { get; }

    /// <summary>The measurements of one alternative.</summary>
    public IReadOnlyList<double> this[int alternative] => _measurements[alternative];

    /// <summary>Builds an experiment with default names (<c>A1</c>, <c>A2</c>, ...).</summary>
    public static Experiment FromColumns(params double[][] measurements) =>
        new(Enumerable.Range(1, measurements.Length).Select(i => $"A{i}").ToArray(), measurements);
}
