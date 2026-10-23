using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using AnovaApp.Core;
using AnovaApp.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AnovaApp.Desktop.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    public const int MinAlternatives = 2;
    public const int MaxAlternatives = 12;
    public const int MinMeasurements = 2;
    public const int MaxMeasurements = 200;

    private const string CsvFilter = "CSV files (*.csv;*.txt)|*.csv;*.txt|All files (*.*)|*.*";

    private readonly IDialogService _dialogs;
    private readonly IClipboardService _clipboard;
    private bool _suspendRecalculation;

    public MainViewModel(IDialogService dialogs, IClipboardService clipboard)
    {
        _dialogs = dialogs;
        _clipboard = clipboard;
        Resize(3, 5);
    }

    /// <summary>Raised when alternatives are added or removed, so the view can rebuild the grid columns.</summary>
    public event EventHandler? ColumnsChanged;

    public ObservableCollection<AlternativeViewModel> Alternatives { get; } = [];

    public ObservableCollection<MeasurementRowViewModel> Rows { get; } = [];

    /// <summary>Suggested levels in the drop-downs; any other value can be typed.</summary>
    public IReadOnlyList<string> ConfidencePresets { get; } = ["90%", "95%", "99%", "99.9%"];

    public static string ConfidenceHint => $"Enter a confidence from {Format.Confidence(Format.MinConfidence)} to below 100%, for example 97.5 or 0.975.";

    public int AlternativeCount => Alternatives.Count;

    public int MeasurementCount => Rows.Count;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasAnovaConfidenceError))]
    private string _anovaConfidenceText = "95%";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasContrastConfidenceError))]
    private string _contrastConfidenceText = "90%";

    // Last valid levels: while a typed value is invalid, the results keep using these.
    private double _anovaConfidence = 0.95;
    private double _contrastConfidence = 0.90;

    public bool HasAnovaConfidenceError => Format.ParseConfidence(AnovaConfidenceText) is null;

    public bool HasContrastConfidenceError => Format.ParseConfidence(ContrastConfidenceText) is null;

    [ObservableProperty]
    private AlternativeViewModel? _contrastA;

    [ObservableProperty]
    private AlternativeViewModel? _contrastB;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasResult))]
    [NotifyCanExecuteChangedFor(nameof(ExportReportCommand))]
    private AnovaSummary? _summary;

    [ObservableProperty]
    private ContrastSummary? _contrast;

    /// <summary>Why there is no result yet, for example empty or invalid cells.</summary>
    [ObservableProperty]
    private string? _status;

    [ObservableProperty]
    private string? _fileName;

    public bool HasResult => Summary is not null;

    partial void OnAnovaConfidenceTextChanged(string value)
    {
        if (Format.ParseConfidence(value) is { } confidence)
        {
            _anovaConfidence = confidence;
            Recalculate();
        }
    }

    partial void OnContrastConfidenceTextChanged(string value)
    {
        if (Format.ParseConfidence(value) is { } confidence)
        {
            _contrastConfidence = confidence;
            Recalculate();
        }
    }
    partial void OnContrastAChanged(AlternativeViewModel? value) => Recalculate();
    partial void OnContrastBChanged(AlternativeViewModel? value) => Recalculate();

    [RelayCommand(CanExecute = nameof(CanAddAlternative))]
    private void AddAlternative() => Resize(AlternativeCount + 1, MeasurementCount);
    private bool CanAddAlternative() => AlternativeCount < MaxAlternatives;

    [RelayCommand(CanExecute = nameof(CanRemoveAlternative))]
    private void RemoveAlternative() => Resize(AlternativeCount - 1, MeasurementCount);
    private bool CanRemoveAlternative() => AlternativeCount > MinAlternatives;

    [RelayCommand(CanExecute = nameof(CanAddMeasurement))]
    private void AddMeasurement() => Resize(AlternativeCount, MeasurementCount + 1);
    private bool CanAddMeasurement() => MeasurementCount < MaxMeasurements;

    [RelayCommand(CanExecute = nameof(CanRemoveMeasurement))]
    private void RemoveMeasurement() => Resize(AlternativeCount, MeasurementCount - 1);
    private bool CanRemoveMeasurement() => MeasurementCount > MinMeasurements;

    [RelayCommand]
    public void LoadSample()
    {
        // Response times (ms) of three sorting algorithms, six runs each.
        Load(new Experiment(
            ["Quicksort", "Mergesort", "Heapsort"],
            [
                [6, 8, 4, 5, 3, 4],
                [8, 12, 9, 11, 6, 8],
                [13, 9, 11, 8, 7, 12],
            ]));
        FileName = "Sample data";
    }

    [RelayCommand]
    private void Clear()
    {
        _suspendRecalculation = true;
        foreach (var cell in Rows.SelectMany(r => r.Cells))
            cell.Text = "";
        _suspendRecalculation = false;
        FileName = null;
        Recalculate();
    }

    [RelayCommand]
    private void Open()
    {
        if (_dialogs.PickFileToOpen(CsvFilter) is { } path)
            LoadFile(path);
    }

    /// <summary>Loads measurements from a CSV file; shows an error instead of throwing.</summary>
    public void LoadFile(string path)
    {
        try
        {
            using var reader = new StreamReader(path);
            Load(ExperimentCsv.Read(reader));
            FileName = Path.GetFileName(path);
        }
        catch (Exception ex) when (ex is IOException or FormatException or UnauthorizedAccessException)
        {
            _dialogs.ShowError("Could not open the file", ex.Message);
        }
    }

    [RelayCommand]
    private void Paste()
    {
        var text = _clipboard.GetText();
        if (string.IsNullOrWhiteSpace(text))
            return;

        try
        {
            Load(ExperimentCsv.Read(new StringReader(text)));
            FileName = "Pasted data";
        }
        catch (FormatException ex)
        {
            _dialogs.ShowError("Could not paste the data", $"Copy a block of cells with one column per alternative.\n\n{ex.Message}");
        }
    }

    [RelayCommand]
    private void SaveData()
    {
        if (TryBuildExperiment(out var experiment, out var problem) is false)
        {
            _dialogs.ShowError("Nothing to save yet", problem);
            return;
        }

        if (_dialogs.PickFileToSave(CsvFilter, "measurements.csv") is not { } path)
            return;

        WriteFile(path, writer => ExperimentCsv.Write(writer, experiment));
    }

    [RelayCommand(CanExecute = nameof(HasResult))]
    private void ExportReport()
    {
        if (Summary is null || !TryBuildExperiment(out var experiment, out _))
            return;

        if (_dialogs.PickFileToSave(CsvFilter, "anova-report.csv") is not { } path)
            return;

        WriteFile(path, writer => AnovaReport.WriteCsv(writer, experiment, Summary.Result, Contrast?.Result));
    }

    private void WriteFile(string path, Action<TextWriter> write)
    {
        try
        {
            using var writer = new StreamWriter(path);
            write(writer);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            _dialogs.ShowError("Could not save the file", ex.Message);
        }
    }

    private void Load(Experiment experiment)
    {
        _suspendRecalculation = true;
        Resize(experiment.AlternativeCount, experiment.MeasurementCount);

        for (int j = 0; j < experiment.AlternativeCount; j++)
        {
            Alternatives[j].Name = experiment.AlternativeNames[j];
            for (int i = 0; i < experiment.MeasurementCount; i++)
                Rows[i].Cells[j].Text = Format.Measurement(experiment[j][i]);
        }

        ContrastA = Alternatives[0];
        ContrastB = Alternatives[1];
        _suspendRecalculation = false;
        Recalculate();
    }

    /// <summary>Changes the grid to k × n, keeping the values that still fit.</summary>
    private void Resize(int alternatives, int measurements)
    {
        bool columnsChanged = alternatives != AlternativeCount;
        var old = Rows.Select(r => r.Cells.Select(c => c.Text).ToArray()).ToArray();

        foreach (var alternative in Alternatives)
            alternative.PropertyChanged -= OnAlternativeChanged;
        foreach (var cell in Rows.SelectMany(r => r.Cells))
            cell.PropertyChanged -= OnCellChanged;

        var names = Alternatives.Select(a => a.Name).ToArray();
        Alternatives.Clear();
        for (int j = 0; j < alternatives; j++)
        {
            var alternative = new AlternativeViewModel(j, j < names.Length ? names[j] : $"A{j + 1}");
            alternative.PropertyChanged += OnAlternativeChanged;
            Alternatives.Add(alternative);
        }

        Rows.Clear();
        for (int i = 0; i < measurements; i++)
        {
            var cells = Enumerable.Range(0, alternatives).Select(j =>
            {
                var cell = new MeasurementCellViewModel(i < old.Length && j < old[i].Length ? old[i][j] : "");
                cell.PropertyChanged += OnCellChanged;
                return cell;
            });
            Rows.Add(new MeasurementRowViewModel(i + 1, cells));
        }

        // Keep the contrast selection when the chosen alternatives still exist.
        bool suspended = _suspendRecalculation;
        _suspendRecalculation = true;
        ContrastA = ContrastA is { Index: var a } && a < alternatives ? Alternatives[a] : Alternatives[0];
        ContrastB = ContrastB is { Index: var b } && b < alternatives && b != ContrastA.Index ? Alternatives[b] : Alternatives[ContrastA.Index == 1 ? 0 : 1];
        _suspendRecalculation = suspended;

        OnPropertyChanged(nameof(AlternativeCount));
        OnPropertyChanged(nameof(MeasurementCount));
        AddAlternativeCommand.NotifyCanExecuteChanged();
        RemoveAlternativeCommand.NotifyCanExecuteChanged();
        AddMeasurementCommand.NotifyCanExecuteChanged();
        RemoveMeasurementCommand.NotifyCanExecuteChanged();

        if (columnsChanged)
            ColumnsChanged?.Invoke(this, EventArgs.Empty);

        Recalculate();
    }

    private void OnCellChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MeasurementCellViewModel.Text))
            Recalculate();
    }

    private void OnAlternativeChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AlternativeViewModel.Name))
            Recalculate();
    }

    private bool TryBuildExperiment(out Experiment experiment, out string problem)
    {
        experiment = null!;
        var cells = Rows.SelectMany(r => r.Cells.Select((c, j) => (Row: r.Number, Column: j, Cell: c))).ToList();

        if (cells.FirstOrDefault(c => c.Cell.HasError) is { Cell: not null } invalid)
        {
            problem = $"\"{invalid.Cell.Text}\" in {Alternatives[invalid.Column].Name}, row {invalid.Row} is not a number.";
            return false;
        }

        int empty = cells.Count(c => c.Cell.IsEmpty);
        if (empty > 0)
        {
            problem = empty == cells.Count
                ? "Enter the measurements, paste them from a spreadsheet, or open a CSV file."
                : $"Fill in {empty} more {(empty == 1 ? "cell" : "cells")} to see the results.";
            return false;
        }

        var columns = Enumerable.Range(0, AlternativeCount)
            .Select(j => (IReadOnlyList<double>)Rows.Select(r => r.Cells[j].Value!.Value).ToArray())
            .ToArray();
        experiment = new Experiment(Alternatives.Select(a => a.Name).ToArray(), columns);
        problem = "";
        return true;
    }

    private void Recalculate()
    {
        if (_suspendRecalculation || Alternatives.Count == 0)
            return;

        if (!TryBuildExperiment(out var experiment, out var problem))
        {
            Summary = null;
            Contrast = null;
            Status = problem;
            return;
        }

        var anova = OneWayAnova.Analyze(experiment, _anovaConfidence);
        Summary = new AnovaSummary(anova, experiment.AlternativeNames);
        Status = null;

        Contrast = ContrastA is { } a && ContrastB is { } b && a.Index != b.Index
            ? new ContrastSummary(OneWayAnova.Contrast(anova, a.Index, b.Index, _contrastConfidence), a.Name, b.Name)
            : null;
    }
}
