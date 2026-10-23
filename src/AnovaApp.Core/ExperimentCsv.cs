using System.Globalization;

namespace AnovaApp.Core;

/// <summary>
/// Reads and writes experiments as CSV: one column per alternative, one row per measurement,
/// with an optional header row of alternative names.
/// </summary>
/// <remarks>
/// The delimiter is detected from the first line (<c>,</c> <c>;</c> or tab). With <c>;</c> or tab,
/// a decimal comma is accepted as well (<c>3,5</c>), as spreadsheets in many locales export it.
/// </remarks>
public static class ExperimentCsv
{
    private static readonly char[] Delimiters = [',', ';', '\t'];

    public static Experiment Read(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var lines = new List<(int Number, string Text)>();
        int lineNumber = 0;
        while (reader.ReadLine() is { } line)
        {
            lineNumber++;
            if (!string.IsNullOrWhiteSpace(line))
                lines.Add((lineNumber, line));
        }

        if (lines.Count == 0)
            throw new FormatException("The file is empty.");

        char delimiter = DetectDelimiter(lines[0].Text);
        var rows = lines.Select(l => (l.Number, Cells: Split(l.Text, delimiter))).ToList();

        int columnCount = rows[0].Cells.Length;
        bool hasHeader = rows[0].Cells.Any(c => !TryParse(c, delimiter, out _));
        var names = hasHeader
            ? rows[0].Cells.Select((c, i) => string.IsNullOrWhiteSpace(c) ? $"A{i + 1}" : c).ToArray()
            : Enumerable.Range(1, columnCount).Select(i => $"A{i}").ToArray();

        var columns = Enumerable.Range(0, columnCount).Select(_ => new List<double>()).ToArray();
        foreach (var (number, cells) in rows.Skip(hasHeader ? 1 : 0))
        {
            if (cells.Length != columnCount)
                throw new FormatException($"Line {number} has {cells.Length} values, expected {columnCount}.");

            for (int j = 0; j < columnCount; j++)
            {
                if (!TryParse(cells[j], delimiter, out double value))
                    throw new FormatException($"Line {number}, column {j + 1}: \"{cells[j]}\" is not a number.");
                columns[j].Add(value);
            }
        }

        try
        {
            return new Experiment(names, columns);
        }
        catch (ArgumentException ex)
        {
            throw new FormatException(ex.Message.Split(" (Parameter")[0], ex);
        }
    }

    public static void Write(TextWriter writer, Experiment experiment)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(experiment);

        writer.WriteLine(string.Join(',', experiment.AlternativeNames.Select(Escape)));
        for (int i = 0; i < experiment.MeasurementCount; i++)
        {
            var row = Enumerable.Range(0, experiment.AlternativeCount)
                .Select(j => experiment[j][i].ToString("R", CultureInfo.InvariantCulture));
            writer.WriteLine(string.Join(',', row));
        }
    }

    internal static string Escape(string value) =>
        value.IndexOfAny([',', '"', '\n', '\r']) >= 0 ? $"\"{value.Replace("\"", "\"\"")}\"" : value;

    private static char DetectDelimiter(string firstLine) =>
        Delimiters.OrderByDescending(d => firstLine.Count(c => c == d)).First();

    private static string[] Split(string line, char delimiter)
    {
        var cells = new List<string>();
        var cell = new System.Text.StringBuilder();
        bool quoted = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (quoted)
            {
                if (c == '"' && i + 1 < line.Length && line[i + 1] == '"')
                {
                    cell.Append('"');
                    i++;
                }
                else if (c == '"')
                {
                    quoted = false;
                }
                else
                {
                    cell.Append(c);
                }
            }
            else if (c == '"')
            {
                quoted = true;
            }
            else if (c == delimiter)
            {
                cells.Add(cell.ToString().Trim());
                cell.Clear();
            }
            else
            {
                cell.Append(c);
            }
        }

        cells.Add(cell.ToString().Trim());
        return cells.ToArray();
    }

    private static bool TryParse(string text, char delimiter, out double value)
    {
        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            return true;

        return delimiter != ','
            && double.TryParse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}
