using AnovaApp.Core;

namespace AnovaApp.Core.Tests;

public class CsvTests
{
    private static Experiment Read(string csv) => ExperimentCsv.Read(new StringReader(csv));

    [Fact]
    public void Read_ParsesHeaderAndColumns()
    {
        var experiment = Read("""
            Quicksort,Mergesort,Heapsort
            1.5,2,3
            2.5,3,4
            """);

        Assert.Equal(["Quicksort", "Mergesort", "Heapsort"], experiment.AlternativeNames);
        Assert.Equal([1.5, 2.5], experiment[0]);
        Assert.Equal([3, 4], experiment[2]);
    }

    [Fact]
    public void Read_WithoutHeader_UsesDefaultNames()
    {
        var experiment = Read("1,2\n3,4\n");

        Assert.Equal(["A1", "A2"], experiment.AlternativeNames);
        Assert.Equal([1, 3], experiment[0]);
    }

    [Fact]
    public void Read_SemicolonWithDecimalComma()
    {
        var experiment = Read("A;B\n1,5;2,25\n3;4\n");

        Assert.Equal([1.5, 3], experiment[0]);
        Assert.Equal([2.25, 4], experiment[1]);
    }

    [Fact]
    public void Read_TabSeparated_SkipsBlankLinesAndQuotes()
    {
        var experiment = Read("\"First\"\t\"Second\"\n\n1\t2\n\n3\t4\n");

        Assert.Equal(["First", "Second"], experiment.AlternativeNames);
        Assert.Equal([2, 4], experiment[1]);
    }

    [Fact]
    public void Read_ReportsLineOfInvalidNumber()
    {
        var ex = Assert.Throws<FormatException>(() => Read("A,B\n1,2\n3,x\n"));

        Assert.Contains("Line 3", ex.Message);
        Assert.Contains("\"x\"", ex.Message);
    }

    [Fact]
    public void Read_ReportsRaggedRow()
    {
        var ex = Assert.Throws<FormatException>(() => Read("A,B\n1,2\n3\n"));

        Assert.Contains("Line 3", ex.Message);
    }

    [Fact]
    public void Read_RejectsEmptyFile()
    {
        Assert.Throws<FormatException>(() => Read("\n\n"));
    }

    [Fact]
    public void Read_TurnsInvalidExperimentIntoFormatException()
    {
        var ex = Assert.Throws<FormatException>(() => Read("A,B\n1,2\n"));

        Assert.Contains("at least two measurements", ex.Message);
    }

    [Fact]
    public void Write_ThenRead_RoundTrips()
    {
        var original = new Experiment(["Fast, cached", "Slow"], [[0.1, 1e-7, 3], [4, 5.25, -6]]);
        var writer = new StringWriter();

        ExperimentCsv.Write(writer, original);
        var copy = Read(writer.ToString());

        Assert.Equal(original.AlternativeNames, copy.AlternativeNames);
        Assert.Equal(original[0], copy[0]);
        Assert.Equal(original[1], copy[1]);
    }

    [Fact]
    public void Report_ContainsTableAndContrast()
    {
        var experiment = Experiment.FromColumns([6, 8, 4, 5, 3, 4], [8, 12, 9, 11, 6, 8], [13, 9, 11, 8, 7, 12]);
        var anova = OneWayAnova.Analyze(experiment, 0.95);
        var contrast = OneWayAnova.Contrast(anova, 0, 1, 0.90);
        var writer = new StringWriter();

        AnovaReport.WriteCsv(writer, experiment, anova, contrast);
        var report = writer.ToString();

        Assert.Contains("Alternatives,84,2,42,", report);
        Assert.Contains("Error,68,15,", report);
        Assert.Contains("Total,152,17,,", report);
        Assert.Contains("Contrast,A1 - A2", report);
        Assert.Contains("Significant difference,yes", report);
    }
}
