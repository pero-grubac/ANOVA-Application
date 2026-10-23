using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AnovaApp.Desktop.ViewModels;

/// <summary>One alternative (a column of the measurement grid).</summary>
public sealed partial class AlternativeViewModel : ObservableObject
{
    public AlternativeViewModel(int index, string name)
    {
        Index = index;
        _name = name;
    }

    public int Index { get; }

    [ObservableProperty]
    private string _name;

    public override string ToString() => Name;
}

/// <summary>One measurement (a row of the measurement grid), with a cell per alternative.</summary>
public sealed class MeasurementRowViewModel(int number, IEnumerable<MeasurementCellViewModel> cells)
{
    public int Number { get; } = number;

    public ObservableCollection<MeasurementCellViewModel> Cells { get; } = new(cells);
}

/// <summary>
/// A single cell. It holds the raw text, so a half-typed value such as <c>3.</c> or <c>-</c>
/// stays in the box instead of being reformatted while the user types.
/// </summary>
public sealed partial class MeasurementCellViewModel : ObservableObject
{
    public MeasurementCellViewModel(string text = "") => _text = text;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Value), nameof(IsEmpty), nameof(HasError))]
    private string _text;

    public double? Value => Format.ParseMeasurement(Text);

    public bool IsEmpty => string.IsNullOrWhiteSpace(Text);

    public bool HasError => !IsEmpty && Value is null;
}
