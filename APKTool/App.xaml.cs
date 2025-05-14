using APKTool.Services;
using APKTool.ViewModels;
using APKTool.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Threading;

namespace APKTool;


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
        services.AddSingleton<MainView>();
        services.AddSingleton<MainViewModel>();
        services.AddTransient<INotifierService, NotifierService>();
        services.AddTransient<IFridaService, FridaService>();
        services.AddTransient<IApkProcessingService, ApkProcessingService>();
        services.AddTransient<ISignerService, SignerService>();
        services.AddTransient<IDecompilerService, DecompilerService>();
        services.AddTransient<ICompilerService, CompilerService>();
        services.AddTransient<IAdbService, AdbService>();
        services.AddTransient<IInjectorService, InjectorService>();
        services.AddTransient<IDeviceMonitorService, DeviceMonitorService>();
        services.AddTransient<IMetaDataService, MetaDataService>();
        services.AddSingleton(_ => Dispatcher.CurrentDispatcher);

        Ioc.Default.ConfigureServices(services.BuildServiceProvider());
    }
}
