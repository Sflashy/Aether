using Aether.Models;
using Aether.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using static Aether.Models.ConsoleOutput;

namespace Aether.ViewModels;

public partial class WorkspaceViewModel : BaseViewModel
{
    [ObservableProperty]
    public partial string SelectedWorkspace { get; set; }
    
    [ObservableProperty]
    public partial Manifest Manifest { get; set; }

    private readonly IDialogService _dialogService;
    private readonly IManifestService _manifestService;

    public WorkspaceViewModel(
        IDialogService dialogService,
        IManifestService manifestService,
        INotifierService notifier,
        Dispatcher dispatcher) : base (notifier, dispatcher)
    {
        _dialogService = dialogService;
        _manifestService = manifestService;
    }

    [RelayCommand]
    private void WorkspaceSelectionDialog()
    {
        var folderName = _dialogService.OpenFolder("Select a Workspace");
        if (string.IsNullOrEmpty(folderName)) return;
        string[] apks = Directory.GetFiles(folderName, "*.apk", SearchOption.TopDirectoryOnly);
        if (apks.Length > 0)
        {
            SelectedWorkspace = folderName;
            _notifier.NotifyConsole("Analyzing APK files...", OutputType.Debug);
            _notifier.NotifyConsole("Analyzing Metadata...", OutputType.Debug);
            _notifier.NotifyConsole($"Found {apks.Length} APK files in '{Path.GetFileName(folderName)}'", OutputType.Debug);
            Manifest = _manifestService.GetMetaData(folderName);

            _notifier.NotifyConsole("Analysis completed. Ready to inject.", OutputType.Info);
        }
        else
        {
            _notifier.NotifyConsole("No APK files found in the selected directory", OutputType.Error);
        }
    }
}
