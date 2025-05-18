using Aether.Messages;
using Aether.Models;
using Aether.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Threading;
using static Aether.Services.InjectorService;

namespace Aether.ViewModels;

public partial class FridaViewModel : BaseViewModel, IRecipient<FridaDownloadStatusMessage>
{
    [ObservableProperty]
    public partial ObservableCollection<Frida> FridaGadgets { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<Architecture> FridaGadgetArchitectures { get; set; }

    [ObservableProperty]
    public partial double IsDownloading { get; set; } = 0;

    [ObservableProperty]
    public partial Frida SelectedFridaGadget { get; set; }

    [ObservableProperty]
    public partial Architecture SelectedFridaArchitecture { get; set; }


    [ObservableProperty]
    public partial string SelectedScriptPath { get; set; }

    [ObservableProperty]
    public partial InjectionType InjectionType { get; set; } = InjectionType.Debug;

    private readonly IFridaService _fridaService;
    private readonly IDialogService _dialogService;
    public FridaViewModel(
        IFridaService fridaService, 
        IDialogService dialogService,
        INotifierService notifier,
        Dispatcher dispatcher) : base(notifier, dispatcher)
    {
        _fridaService = fridaService;
        _dialogService = dialogService;
        WeakReferenceMessenger.Default.Register<FridaDownloadStatusMessage>(this);
        Initialize().ConfigureAwait(false);
    }
    private async Task Initialize()
    {
        Frida[] fridaVersions = await _fridaService.GetFridaVersionsAsync();
        FridaGadgets = [.. fridaVersions];
        SelectedFridaGadget = FridaGadgets.FirstOrDefault();
        CheckForInstalledGadgets();
    }

    private void CheckForInstalledGadgets()
    {
        string installDir = PathService.FridaGadgetDirectory;
        string[] installedGadgets = Directory.GetFiles(installDir, "*.so", SearchOption.TopDirectoryOnly);
        var installedFileNames = installedGadgets.Select(path => Path.GetFileNameWithoutExtension(path)).ToList();

        foreach (Frida frida in FridaGadgets)
        {
            string version = frida.Version.Replace(" (latest)", "");
            frida.IsInstalled = installedFileNames.Any(fileName => fileName.Contains(version));
            foreach (Architecture architecture in frida.Architectures)
            {
                architecture.IsInstalled = installedFileNames.Any(fileName => fileName.Contains(version) && fileName.Contains(architecture.ABIName));
            }
        }
    }


    private bool CanDownloadFrida() => SelectedFridaGadget != null;
    [RelayCommand(CanExecute = nameof(CanDownloadFrida))]
    private async Task DownloadFridaAsync()
    {
        if (SelectedFridaGadget == null || SelectedFridaArchitecture == null) return;
        await _fridaService.DownloadAndExtractAsync(SelectedFridaGadget, SelectedFridaArchitecture);

    }

    [RelayCommand]
    private void ScriptSelectionDialog()
    {
        var filePath = _dialogService.OpenFile("JavaScript Files (*.js)|*.js", "Select a JavaScript file");
        if (string.IsNullOrEmpty(filePath)) return;
        SelectedScriptPath = filePath;
    }

    public void Receive(FridaDownloadStatusMessage message)
    {
        _dispatcher.Invoke(() => IsDownloading = message.Value);
    }

    partial void OnSelectedFridaGadgetChanged(Frida value)
    {
        _dispatcher.Invoke(() =>
        {
            DownloadFridaCommand.NotifyCanExecuteChanged();
            SelectedFridaArchitecture = SelectedFridaArchitecture == null
            ? SelectedFridaGadget.Architectures.FirstOrDefault(x => x.Name.Contains("arm64"))
            : SelectedFridaGadget.Architectures.FirstOrDefault(x => x.ABIName == SelectedFridaArchitecture.ABIName);
        });
    }


}
