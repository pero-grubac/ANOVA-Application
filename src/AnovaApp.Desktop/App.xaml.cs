using System.Windows;
using AnovaApp.Desktop.Services;
using AnovaApp.Desktop.ViewModels;

namespace AnovaApp.Desktop;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var viewModel = new MainViewModel(new DialogService(), new ClipboardService());
        viewModel.LoadSample();

        // A CSV passed on the command line (or by "Open with") is loaded on start.
        if (e.Args.Length > 0)
            viewModel.LoadFile(e.Args[0]);

        new MainWindow(viewModel).Show();
    }
}
