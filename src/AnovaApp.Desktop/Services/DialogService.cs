using System.Windows;
using Microsoft.Win32;

namespace AnovaApp.Desktop.Services;

public interface IDialogService
{
    /// <returns>The chosen path, or <see langword="null"/> if the user cancelled.</returns>
    string? PickFileToOpen(string filter);

    /// <returns>The chosen path, or <see langword="null"/> if the user cancelled.</returns>
    string? PickFileToSave(string filter, string defaultFileName);

    void ShowError(string title, string message);
}

public interface IClipboardService
{
    string? GetText();
}

public sealed class DialogService : IDialogService
{
    public string? PickFileToOpen(string filter)
    {
        var dialog = new OpenFileDialog { Filter = filter };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string? PickFileToSave(string filter, string defaultFileName)
    {
        var dialog = new SaveFileDialog { Filter = filter, FileName = defaultFileName };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public void ShowError(string title, string message) =>
        MessageBox.Show(Application.Current.MainWindow, message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
}

public sealed class ClipboardService : IClipboardService
{
    public string? GetText() => Clipboard.ContainsText() ? Clipboard.GetText() : null;
}
