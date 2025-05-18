using Aether.Exceptions;
using Aether.Models;
using Aether.ViewModels;
using System.IO;
using static Aether.Models.ConsoleOutput;

namespace Aether.Services;

public interface IEnvironmentService
{
    Task InitializeAsync();
}
public class EnvironmentService : IEnvironmentService
{
    private readonly INotifierService _notifier;
    private readonly IInjectorService _injector;
    private readonly IApkProcessingService _apkProcessing;
    private readonly FridaViewModel _fridaVm;
    private readonly WorkspaceViewModel _workspaceVm;
    private readonly DeviceViewModel _deviceVm;

    public EnvironmentService(INotifierService notifier, 
        FridaViewModel fridaVm, 
        WorkspaceViewModel workspaceVm,
        DeviceViewModel deviceVm,
        IApkProcessingService apkProcessingService,
        IInjectorService injectorService)
    {
        _notifier = notifier;
        _fridaVm = fridaVm;
        _workspaceVm = workspaceVm;
        _deviceVm = deviceVm;
        _apkProcessing = apkProcessingService;
        _injector = injectorService;
    }
    public async Task InitializeAsync()
    {
        _notifier.NotifyConsole("Initializing environment...", OutputType.Debug);
        EnsureAPKToolInstalled();
        EnsureFridaGadgetSelected();
        EnsureWorkspaceSelected();
        EnsureDeviceIsConnected();
        await EnsureCleanup();
        _apkProcessing.Initialize(_workspaceVm.SelectedWorkspace, _deviceVm.SelectedDevice);
        _injector.Initialize(_workspaceVm.SelectedWorkspace, _fridaVm.SelectedFridaGadget, new FridaConfig(_fridaVm.InjectionType, _fridaVm.SelectedScriptPath, _workspaceVm.Manifest));

    }

    private async Task EnsureCleanup()
    {
        await Task.Run(() =>
        {
            string decompiledPath = Path.Combine(_workspaceVm.SelectedWorkspace, PathService.DecompiledPath);
            string compiledPath = Path.Combine(_workspaceVm.SelectedWorkspace, PathService.CompiledPath);
            if (Directory.Exists(decompiledPath))
                Directory.Delete(decompiledPath, true);

            if (Directory.Exists(compiledPath))
                Directory.Delete(compiledPath, true);
        });
    }

    private void EnsureDeviceIsConnected()
    {
        if (_deviceVm.SelectedDevice == null)
        {
            throw new EnvironmentInitializeException($"Please choose a device to proceed!");
        }
        if(_deviceVm.SelectedDevice.Status == "Disconnected")
        {
            throw new EnvironmentInitializeException($"Please connect a device to proceed!");
        }
        
    }

    private void EnsureWorkspaceSelected()
    {
        if (!string.IsNullOrEmpty(_workspaceVm.SelectedWorkspace)) return;
        throw new EnvironmentInitializeException($"Please choose a workspace to proceed!");
    }

    private void EnsureAPKToolInstalled()
    {
        if (File.Exists(PathService.ApkToolPath)) return;
        throw new EnvironmentInitializeException($"Could not find Aether in {PathService.ApkToolPath}");

    }
    private void EnsureFridaGadgetSelected()
    {
        if (_fridaVm.SelectedFridaGadget != null) return;
        throw new EnvironmentInitializeException($"Please choose a frida-gadget version to proceed!");
    }

}
