using AnovaApp.Core;

namespace AnovaApp.Core.Tests;

public class ExperimentTests
{
    [Fact]
    public void FromColumns_UsesDefaultNames()
    {
        var experiment = Experiment.FromColumns([1, 2], [3, 4], [5, 6]);

        Assert.Equal(["A1", "A2", "A3"], experiment.AlternativeNames);
        Assert.Equal(3, experiment.AlternativeCount);
        Assert.Equal(2, experiment.MeasurementCount);
        Assert.Equal([3, 4], experiment[1]);
    }

    [Fact]
    public void Constructor_CopiesInput()
    {
        double[] column = [1, 2];
        var experiment = Experiment.FromColumns(column, [3, 4]);

        column[0] = 100;

        Assert.Equal(1, experiment[0][0]);
    }

    [Fact]
    public void Constructor_RejectsSingleAlternative()
    {
        Assert.Throws<ArgumentException>(() => Experiment.FromColumns([1, 2, 3]));
    }

    [Fact]
    public void Constructor_RejectsSingleMeasurement()
    {
        Assert.Throws<ArgumentException>(() => Experiment.FromColumns([1], [2]));
    }

    [Fact]
    public void Constructor_RejectsUnequalSampleSizes()
    {
        Assert.Throws<ArgumentException>(() => Experiment.FromColumns([1, 2], [3, 4, 5]));
    }

    [Fact]
    public void Constructor_RejectsNonFiniteValues()
    {
        Assert.Throws<ArgumentException>(() => Experiment.FromColumns([1, double.NaN], [3, 4]));
        Assert.Throws<ArgumentException>(() => Experiment.FromColumns([1, 2], [double.PositiveInfinity, 4]));
    }

    [Fact]
    public void Constructor_RejectsNameCountMismatch()
    {
        Assert.Throws<ArgumentException>(() => new Experiment(["Only one"], [[1.0, 2], [3.0, 4]]));
    }
}
