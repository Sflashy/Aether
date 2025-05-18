using Aether.Models;
using Aether.Services;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Threading;
using static Aether.Models.ConsoleOutput;
using static Aether.Services.InjectorService;

namespace Aether.ViewModels;

public partial class InjectionViewModel : BaseViewModel
{
    private readonly IEnvironmentService _environment;
    private readonly IApkProcessingService _apkProcessig;
    private readonly IInjectorService _injector;
    public InjectionViewModel(IEnvironmentService environmentService,
        IApkProcessingService apkProcessingService,
        IInjectorService injectorService,
        INotifierService notifierService, 
        Dispatcher dispatcher) : base(notifierService, dispatcher)
    {
        _injector = injectorService;
        _apkProcessig = apkProcessingService;
        _environment = environmentService;
    }

    [RelayCommand]
    private async Task StartInjectionAsync()
    {
        _notifier.NotifyConsole("Starting injection process...", OutputType.Process);

        var pipeline = new Func<Task>[]
        {
            async () => await _environment.InitializeAsync(),
            async () => await _apkProcessig.DecompileApkAsync(),
            async () => await _injector.InjectFridaAsync(),
            async () => await _injector.PathchSmaliAsync(),
            async () => await _apkProcessig.CompileApkAsync(),
            async () => await _apkProcessig.SignApkAsync(),
            async () => await _apkProcessig.InstallApksAsync()
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
}
