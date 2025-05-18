using Aether.Models;

namespace Aether.Services;

public interface IDeviceMonitorService
{
    void ScanForDevices();
    void Start(Device device, int intervalInMilliseconds = 5000);
    void Stop();
}
public class DeviceMonitorService : IDeviceMonitorService
{
    private readonly IAdbService _adb;
    private CancellationTokenSource _cts;
    private List<Device> _devices;
    private readonly INotifierService _notifier;

    public DeviceMonitorService(IAdbService adbService, INotifierService notifier)
    {
        _adb = adbService;
        _notifier = notifier;
        _devices = new List<Device>();
    }

    public void ScanForDevices()
    {

        Task.Run(async () =>
        {
            while (true)
            {
                Device[] devices = await Task.Run(_adb.GetDevices);
                foreach (Device device in devices)
                {                    
                    if (_devices.Any(d => d.Id == device.Id)) continue;
                    _adb.UpdateDeviceInfo(device);
                    _devices.Add(device);
                    _notifier.NotifyDeviceFound(device);
                }
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        });

    }

    public void Start(Device device, int intervalInMilliseconds = 5000)
    {
        Stop();
        _cts = new CancellationTokenSource();

        Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    bool isConnected = _adb.IsAdbConnected(device.Id);
                    device.Status = isConnected ? "Connected" : "Disconnected";
                    device.USBDebuggingStatus = isConnected ? "Enabled" : "Disabled";

                    if (isConnected)
                    {
                        device.CpuUsage = _adb.GetCpuUsage(device);
                        device.RamUsage = _adb.GetRamUsage(device);
                        _notifier.NotifyDeviceUpdate(device);
                    }
                    else
                    {
                        device.CpuUsage = "N/A";
                        device.RamUsage = "N/A";
                    }
                }
                catch (Exception)
                {
                    device.Status = "Error";
                    device.CpuUsage = "N/A";
                    device.RamUsage = "N/A";
                }

                await Task.Delay(intervalInMilliseconds, _cts.Token);
            }
        }, _cts.Token);
    }

    public void Stop()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
