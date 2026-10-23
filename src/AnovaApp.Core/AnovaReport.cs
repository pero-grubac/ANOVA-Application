using System.Globalization;

namespace AnovaApp.Core;

/// <summary>Writes the results of an analysis as a CSV report that opens cleanly in a spreadsheet.</summary>
public static class AnovaReport
{
    public static void WriteCsv(TextWriter writer, Experiment experiment, AnovaResult anova, ContrastResult? contrast)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(experiment);
        ArgumentNullException.ThrowIfNull(anova);

        static string N(double value) => value.ToString("G10", CultureInfo.InvariantCulture);
        void Row(params object[] cells) =>
            writer.WriteLine(string.Join(',', cells.Select(c => c is double d ? N(d) : ExperimentCsv.Escape(Convert.ToString(c, CultureInfo.InvariantCulture) ?? ""))));

        Row("Analysis of variance");
        Row("Alternatives (k)", anova.AlternativeCount);
        Row("Measurements per alternative (n)", anova.MeasurementCount);
        Row("Confidence", anova.Confidence);
        writer.WriteLine();

        Row("Source", "Sum of squares", "Degrees of freedom", "Mean square", "F");
        Row("Alternatives", anova.Ssa, anova.DfAlternatives, anova.MsAlternatives, anova.F);
        Row("Error", anova.Sse, anova.DfError, anova.MsError, "");
        Row("Total", anova.Sst, anova.DfTotal, "", "");
        writer.WriteLine();

        Row("F critical", anova.FCritical);
        Row("p-value", anova.PValue);
        Row("Explained variation", anova.ExplainedVariation);
        Row("Significant difference", anova.IsSignificant ? "yes" : "no");
        writer.WriteLine();

        Row("Alternative", "Mean", "Effect");
        for (int j = 0; j < anova.AlternativeCount; j++)
            Row(experiment.AlternativeNames[j], anova.Means[j], anova.Effects[j]);
        Row("Grand mean", anova.GrandMean, "");

        if (contrast is null)
            return;

        writer.WriteLine();
        Row("Contrast", $"{experiment.AlternativeNames[contrast.AlternativeA]} - {experiment.AlternativeNames[contrast.AlternativeB]}");
        Row("Confidence", contrast.Confidence);
        Row("Estimate", contrast.Estimate);
        Row("Standard error", contrast.StandardError);
        Row("t critical", contrast.TCritical);
        Row("Interval lower", contrast.Interval.Lower);
        Row("Interval upper", contrast.Interval.Upper);
        Row("Significant difference", contrast.IsSignificant ? "yes" : "no");
    }
}
