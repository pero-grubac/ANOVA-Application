using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AnovaApp.Desktop.ViewModels;

namespace AnovaApp.Desktop;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = _viewModel = viewModel;

        _viewModel.ColumnsChanged += (_, _) => BuildColumns();
        BuildColumns();

        MeasurementGrid.PreviewKeyDown += OnGridKeyDown;
        Drop += OnDrop;
        DragOver += OnDragOver;
    }

    /// <summary>One column per alternative; the header is a text box that renames it.</summary>
    private void BuildColumns()
    {
        MeasurementGrid.Columns.Clear();
        MeasurementGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "#",
            Binding = new Binding(nameof(MeasurementRowViewModel.Number)),
            IsReadOnly = true,
            Width = 44,
            ElementStyle = CenteredText(secondary: true),
        });

        var headerStyle = (Style)FindResource("HeaderTextBox");
        foreach (var alternative in _viewModel.Alternatives)
        {
            var header = new TextBox { Style = headerStyle };
            header.SetBinding(TextBox.TextProperty, new Binding(nameof(AlternativeViewModel.Name))
            {
                Source = alternative,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });
            AutomationProperties.SetName(header, $"Name of alternative {alternative.Index + 1}");

            string cell = $"{nameof(MeasurementRowViewModel.Cells)}[{alternative.Index}]";
            var cellStyle = new Style(typeof(DataGridCell), (Style)FindResource(typeof(DataGridCell)));
            var invalid = new DataTrigger { Binding = new Binding($"{cell}.{nameof(MeasurementCellViewModel.HasError)}"), Value = true };
            invalid.Setters.Add(new Setter(BackgroundProperty, FindResource("SystemFillColorCriticalBackgroundBrush")));
            invalid.Setters.Add(new Setter(ForegroundProperty, FindResource("SystemFillColorCriticalBrush")));
            cellStyle.Triggers.Add(invalid);

            MeasurementGrid.Columns.Add(new DataGridTextColumn
            {
                Header = header,
                Binding = new Binding($"{cell}.{nameof(MeasurementCellViewModel.Text)}")
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                MinWidth = 84,
                ElementStyle = CenteredText(secondary: false),
                CellStyle = cellStyle,
            });
        }
    }

    private static Style CenteredText(bool secondary)
    {
        var style = new Style(typeof(TextBlock));
        style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center));
        style.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
        if (secondary)
            style.Setters.Add(new Setter(TextBlock.ForegroundProperty, new DynamicResourceExtension("TextFillColorSecondaryBrush")));
        return style;
    }

    // Ctrl+V on a selected (not edited) cell pastes a block copied from a spreadsheet.
    private void OnGridKeyDown(object sender, KeyEventArgs e)
    {
        bool editing = e.OriginalSource is TextBox;
        if (!editing && e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
        {
            _viewModel.PasteCommand.Execute(null);
            e.Handled = true;
        }
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(DataFormats.FileDrop) is string[] { Length: > 0 } files)
            _viewModel.LoadFile(files[0]);
    }
}

