using APKTool.Models;

namespace APKTool.Services;

public interface IDeviceMonitorService
{
    void Start(Device device, int intervalInMilliseconds = 5000);
    void Stop();
}
public class DeviceMonitorService : IDeviceMonitorService
{
    private readonly IAdbService _adbService;
    private CancellationTokenSource _cts;
    private Device _device;

    public DeviceMonitorService(IAdbService adbService)
    {
        _adbService = adbService;
    }

    public void Start(Device device, int intervalInMilliseconds = 5000)
    {
        Stop();

        _device = device;
        _cts = new CancellationTokenSource();

        Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    bool isConnected = _adbService.IsAdbConnected(device.Id);
                    device.Status = isConnected ? "Connected" : "Disconnected";
                    device.USBDebuggingStatus = isConnected ? "Enabled" : "Disabled";

                    if (isConnected)
                    {
                        device.CpuUsage = _adbService.GetCpuUsage(device);
                        device.RamUsage = _adbService.GetRamUsage(device);
                    }
                    else
                    {
                        device.CpuUsage = "N/A";
                        device.RamUsage = "N/A";
                    }
                }
                catch (Exception ex)
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
