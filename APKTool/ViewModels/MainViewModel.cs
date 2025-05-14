using APKTool.Messages;
using APKTool.Models;
using APKTool.Services;
using APKTool.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static APKTool.Models.ConsoleOutput;


namespace APKTool.ViewModels;

public partial class MainViewModel : ObservableObject, IRecipient<ConsoleMessage>, IRecipient<ActivityMessage>, IRecipient<DownloadStatusMessage>
{
    [ObservableProperty]
    public partial ObservableCollection<ConsoleOutput> ConsoleOutputs { get; set; } = new ObservableCollection<ConsoleOutput>();
    [ObservableProperty]
    public partial ObservableCollection<Frida> AvailableFridaVersions { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<Activity> Activities { get; set; } = new ObservableCollection<Activity>();
    [ObservableProperty]
    public partial double IsDownloading { get; set; } = 0;
    [ObservableProperty]
    public partial Frida SelectedFridaVersion { get; set; }
    [ObservableProperty]
    public partial double DownloadProgress { get; set; }
    [ObservableProperty]
    public partial FridaAsset FridaAsset { get; set; }
    [ObservableProperty]
    public partial string ApkDirectory { get; set; }
    [ObservableProperty]
    public partial Device[] Devices { get; set; }
    [ObservableProperty]
    public partial Device SelectedDevice { get; set; }
    [ObservableProperty]
    public partial bool IsCleanupEnabled { get; set; }
    [ObservableProperty]
    public partial Manifest Manifest { get; set; }

    private Dispatcher _dispatcher;

    private readonly IApkProcessingService _apkProcessingService;
    private readonly IFridaService _fridaService;
    private readonly IAdbService _adb;
    private readonly IInjectorService _injectorService;
    private readonly INotifierService _notifier;
    private readonly IDeviceMonitorService _deviceMonitorService;
    private readonly IMetaDataService _metadataService;
    public MainViewModel(
        IApkProcessingService apkProcessingService,
        IFridaService fridaService,
        IAdbService adbService,
        IInjectorService injectorService,
        INotifierService notifier,
        IDeviceMonitorService deviceMonitorService,
        IMetaDataService metadataService,
        Dispatcher dispatcher)
    {
        _apkProcessingService = apkProcessingService;
        _fridaService = fridaService;
        _injectorService = injectorService;
        _adb = adbService;
        _notifier = notifier;
        _dispatcher = dispatcher;
        _deviceMonitorService = deviceMonitorService;
        _metadataService = metadataService;
        InitializeMessengers();
        InitializeAsync().ConfigureAwait(false);

    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        _deviceMonitorService.Stop();
        ConsoleOutputs.Clear();
        _notifier.NotifyConsole("Frida APK Injector v1.0", OutputType.Debug);
        await InitializeEnvironmentAsync();
    }

    private async Task InitializeEnvironmentAsync()
    {
        _notifier.NotifyConsole("Initializing environment...", OutputType.Debug);
        await InitializeDevices();
        await InitializeFridaGadget();
        _notifier.NotifyConsole("Environment initialized successfully", OutputType.Success);
        _notifier.NotifyConsole("Please select an APK folder to continue.", OutputType.Warning);
        
    }
    private async Task InitializeDevices()
    {
        Devices = await Task.Run(_adb.GetDevices);
        foreach (var device in Devices)
        {
            _adb.UpdateDeviceInfo(device);
        }
        if (Devices.Length > 0)
        {
            SelectedDevice = Devices.FirstOrDefault();
            _notifier.NotifyConsole($"Device found: {SelectedDevice.Id}", OutputType.Success);
            _notifier.NotifyConsole($"Device Information:\n     - Serial: {SelectedDevice.Id}\n     - Model: {SelectedDevice.Model}\n     - Android: {SelectedDevice.Android}", OutputType.Debug);

            _deviceMonitorService.Start(SelectedDevice);
        }
        else
        {
            _notifier.NotifyConsole("Environment initialization failed", OutputType.Error);
            _notifier.NotifyConsole("No device connected, please connect a device", OutputType.Warning);
        }
    }
    private void InitializeMessengers()
    {
        WeakReferenceMessenger.Default.Register<ConsoleMessage>(this);
        WeakReferenceMessenger.Default.Register<ActivityMessage>(this);
        WeakReferenceMessenger.Default.Register<DownloadStatusMessage>(this);
    }


    private bool CanExecuteInject => !string.IsNullOrEmpty(ApkDirectory);
    [RelayCommand(CanExecute = nameof(CanExecuteInject))]
    private async Task StartInjectionAsync()
    {
        _notifier.NotifyConsole("Starting injection proces...", OutputType.Process);
        var pipeline = new Func<Task>[]
        {
            async () => await _apkProcessingService.DecompileApkAsync(),
            async () => await _injectorService.InjectFridaAsync(SelectedFridaVersion),
            async () => await _injectorService.PathchSmaliAsync(),
            async () => await _apkProcessingService.CompileApkAsync(),
            async () => await _apkProcessingService.SignApkAsync(),
            async () => await _apkProcessingService.InstallApksAsync(SelectedDevice)
        };

        foreach (var step in pipeline)
        {
            try
            {
                await step();
            }
            catch (Exception ex)
            {
                _notifier.NotifyConsole($"Error during {step.Method.Name}: {ex.Message}", OutputType.Error);
                return;
            }
        }

        _notifier.NotifyConsole("Injection completed successfully!", OutputType.Success);
    }

    private async Task InitializeFridaGadget()
    {
        Frida[] fridaVersions = await _fridaService.GetFridaVersionsAsync();
        AvailableFridaVersions = [.. fridaVersions]; 
        SelectedFridaVersion = AvailableFridaVersions.FirstOrDefault();
        SelectedFridaVersion.Architecture = SelectedFridaVersion.Architectures.FirstOrDefault(x => x.Contains("arm64"));
    }


    #region Events
    public void Receive(ConsoleMessage message)
    {
        if (message.Value == null) return;

        _dispatcher.Invoke(() => ConsoleOutputs.Add(message.Value));
    }

    public void Receive(ActivityMessage message)
    {
        if (message.Value == null) return;
        _dispatcher.Invoke(() => Activities.Insert(0, message.Value));
    }

    public void Receive(DownloadStatusMessage message)
    {
        _dispatcher.Invoke(() => IsDownloading = message.Value);
    }
    partial void OnApkDirectoryChanged(string value)
    {
        StartInjectionCommand.NotifyCanExecuteChanged();

    }
    partial void OnSelectedFridaVersionChanged(Frida value)
    {
        _dispatcher.Invoke(() => DownloadFridaCommand.NotifyCanExecuteChanged());
    }

    [RelayCommand]
    private void BrowseFolder()
    {

        var dialog = new OpenFolderDialog();
        if (dialog.ShowDialog() == false) return;
        
        string[] apks = Directory.GetFiles(dialog.FolderName, "*.apk", SearchOption.AllDirectories);
        if (apks.Length > 0)
        {
            ApkDirectory = dialog.FolderName;
            _notifier.NotifyConsole($"Selected directory: {Path.GetFileName(ApkDirectory)}", OutputType.Debug);
            _notifier.NotifyConsole($"Found {apks.Length} APK files", OutputType.Debug);
            Manifest = _metadataService.GetMetaData(dialog.FolderName);
            _apkProcessingService.Initialize(ApkDirectory);
            _injectorService.Initialize(ApkDirectory);
            _notifier.NotifyConsole("Ready to inject.", OutputType.Info);
        }
        else
        {
            _notifier.NotifyConsole("No APK files found in the selected directory", OutputType.Error);
        }
    }

    private bool CanDownloadFrida() => SelectedFridaVersion != null;
    [RelayCommand(CanExecute = nameof(CanDownloadFrida))]

    private async Task DownloadFridaAsync(Frida frida)
    {
        if (frida == null) return;
        FridaAsset = frida.Assets.FirstOrDefault(x => x.Name.Contains("frida-gadget") && x.Name.Contains("arm64"));
        await _fridaService.DownloadAndExtractAsync(frida);

    }

    [RelayCommand]
    private void MinimizeWindow(Window window) => window.WindowState = WindowState.Minimized;

    [RelayCommand]
    private void CloseWindow(Window window) => Application.Current.Shutdown();

    [RelayCommand]
    private void DragWindow(Window window) => window.DragMove();
    #endregion



}
