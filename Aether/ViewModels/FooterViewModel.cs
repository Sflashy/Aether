using Aether.Messages;
using Aether.Models;
using Aether.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace Aether.ViewModels;

public partial class FooterViewModel : BaseViewModel, IRecipient<DeviceUpdateMessage>
{
    [ObservableProperty]
    public partial Device Device { get; set; }

    public string AppVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    public string ADBVersion => GetADBVersion();

    private readonly IAdbService _adb;
    public FooterViewModel(IAdbService adbService, INotifierService notifierService, Dispatcher dispatcher) : base(notifierService, dispatcher)
    {
        WeakReferenceMessenger.Default.Register<DeviceUpdateMessage>(this);
        _adb = adbService;

    }

    private string GetADBVersion()
    {
        string output = _adb.RunAdbCommand("--version");
        string versionSection = output.Split("\n")[0].Trim();
        return Regex.Match(versionSection, @".*version (.*)").Groups[1].Value;
    }

    public void Receive(DeviceUpdateMessage message)
    {
        _dispatcher.Invoke(() => Device = message.Value);
    }
}
