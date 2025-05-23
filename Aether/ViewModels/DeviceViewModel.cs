using Aether.Messages;
using Aether.Models;
using Aether.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using static Aether.Models.ConsoleOutput;

namespace Aether.ViewModels;

public partial class DeviceViewModel : BaseViewModel, IRecipient<DeviceFoundMessage>
{

    [ObservableProperty]
    public partial ObservableCollection<Device> Devices { get; set; }
    
    [ObservableProperty]
    public partial Device SelectedDevice { get; set; }

    private readonly IDeviceMonitorService _deviceMonitorService;
    private readonly IAdbService _adb;

    public DeviceViewModel(IAdbService adbService, IDeviceMonitorService deviceMonitorService, INotifierService notifierService, Dispatcher dispatcher) : base(notifierService, dispatcher)
    {
        _deviceMonitorService = deviceMonitorService;
        _adb = adbService;
        Initialize();
    }

    private void Initialize()
    {
        Devices = new ObservableCollection<Device>();
        WeakReferenceMessenger.Default.Register<DeviceFoundMessage>(this);
        _deviceMonitorService.ScanForDevices();
    }

    partial void OnSelectedDeviceChanged(Device value)
    {
        _deviceMonitorService.Start(value);
    }
    public void Receive(DeviceFoundMessage message)
    {
        if (message == null || message.Value == null) return;
        Device device = message.Value;
        _notifier.NotifyConsole($"Device found: {device.Id}", OutputType.Success);
        _notifier.NotifyConsole($"Device Information:\n     - Serial: {device.Id}\n     - Model: {device.Model}\n     - Android: {device.Android}", OutputType.Debug);
        _dispatcher.Invoke(() =>
        {
            Devices.Add(device);
            if (Devices.Count == 1) SelectedDevice = Devices.FirstOrDefault();
        });
    }

}
