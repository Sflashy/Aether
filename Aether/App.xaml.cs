using Aether.Services;
using Aether.ViewModels;
using Aether.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Threading;

namespace Aether;


public partial class App : Application
{
    [STAThread]
    public static void Main(string[] args)
    {
        InitializeServices();
        var app = new App();
        app.InitializeComponent();
        app.MainWindow = Ioc.Default.GetRequiredService<MainView>();
        app.MainWindow.Visibility = Visibility.Visible;
        app.Run();
    }
    
    public static void InitializeServices()
    {
        var services = new ServiceCollection();
        //views
        services.AddSingleton<MainView>();
        //viewModels
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<FridaViewModel>();
        services.AddSingleton<DeviceViewModel>();
        services.AddSingleton<WorkspaceViewModel>();
        services.AddSingleton<InjectionViewModel>();
        services.AddSingleton<FooterViewModel>();
        //services
        services.AddSingleton<INotifierService, NotifierService>();
        services.AddSingleton<IFridaService, FridaService>();
        services.AddSingleton<IApkProcessingService, ApkProcessingService>();
        services.AddSingleton<ISignerService, SignerService>();
        services.AddSingleton<IDecompilerService, DecompilerService>();
        services.AddSingleton<ICompilerService, CompilerService>();
        services.AddSingleton<IAdbService, AdbService>();
        services.AddSingleton<IInjectorService, InjectorService>();
        services.AddSingleton<IDeviceMonitorService, DeviceMonitorService>();
        services.AddSingleton<IManifestService, MetaDataService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IEnvironmentService, EnvironmentService>();
        services.AddSingleton(_ => Dispatcher.CurrentDispatcher);

        Ioc.Default.ConfigureServices(services.BuildServiceProvider());
    }
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        IAdbService adb = Ioc.Default.GetRequiredService<IAdbService>();
        adb.RunAdbCommand("kill-server");
    }
}
