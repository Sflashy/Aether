using APKTool.Models;
using APKTool.Utils;
using System.IO;
using System.Text.RegularExpressions;
using static APKTool.Models.ConsoleOutput;

namespace APKTool.Services;

public interface IAdbService
{
    bool IsAdbConnected(string deviceId);
    Device[] GetDevices();
    string RunAdbCommand(string arguments);
    Device UpdateDeviceInfo(Device device);
    string GetCpuUsage(Device device);
    string GetRamUsage(Device device);
    Task InstallApksAsync(string apkPath);
}
public class AdbService : IAdbService
{
    private readonly INotifierService _notifier;
    public AdbService(INotifierService notifier)
    {
        _notifier = notifier;
    }
    public bool IsAdbConnected(string deviceId)
    {
        var output = RunAdbCommand("devices");
        return output.Contains($"{deviceId}\tdevice");
    }

    public  Device[] GetDevices()
    {
        _notifier.NotifyConsole("Scanning for connected devices...", OutputType.Debug);
        string devices = RunAdbCommand("devices");
        string[] deviceLines = devices.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Device[] devicesList = new Device[deviceLines.Length - 1];
        for (int i = 1; i < deviceLines.Length; i++)
        {
            string[] deviceInfo = deviceLines[i].Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            devicesList[i - 1] = new Device { Id = deviceInfo[0] };
        }
        return devicesList;
    }

    public string RunAdbCommand(string arguments)
    {
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "adb",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output.Trim();
    }

    public Device UpdateDeviceInfo(Device device)
    {
        if (!IsAdbConnected(device.Id))
        {
            device.Status = "Disconnected";
            device.USBDebuggingStatus = "Disabled";
            return device;

        }
        device.Model = RunAdbCommand($"shell getprop ro.product.model");
        device.Architecture = RunAdbCommand($"shell getprop ro.product.cpu.abi");
        device.API = RunAdbCommand($"shell getprop ro.build.version.sdk");
        device.Version = RunAdbCommand($"shell getprop ro.build.version.release");
        device.Id = RunAdbCommand($"shell getprop ro.serialno");
        device.Status = "Connected";
        device.USBDebuggingStatus = "Enabled";
        return device;
    }

    public async Task InstallApksAsync(string apkPath)
    {
        var apks = Directory.GetFiles(Path.Combine(apkPath, PathService.CompiledPath), "*.apk", SearchOption.AllDirectories);
        if (apks.Length == 0)
        {
            _notifier.NotifyConsole($"No APK files found in directory: {PathService.CompiledPath}", OutputType.Error);
            return;
        }

        _notifier.NotifyConsole($"Starting APK installation process...", OutputType.Debug);
        _notifier.NotifyConsole($"Installing {apks.Length} APK files...", OutputType.Debug);
        var installCommand = $"adb install-multiple {string.Join(" ", apks.Select(apk => $"\"{apk}\""))}";

        var (success, output) = await Task.Run(() => ProcessHelper.RunAsync("cmd.exe", $"/C {installCommand}"));

        if (!success)
        {
            _notifier.NotifyConsole($"Failed to install APKs. {output}", OutputType.Error);
        }
        else
        {
            _notifier.NotifyActivity(new Activity(string.Join(" | ", apks.Select(apk => Path.GetFileName(apk))), "APKs installed", Activity.ActivityType.APK));
        }
    }


    public string GetCpuUsage(Device device)
    {
        var output = RunAdbCommand("shell dumpsys cpuinfo");
        if (string.IsNullOrWhiteSpace(output))
            return "N/A";

        // Çıktıyı satırlara ayır
        var lines = output.Split('\n');

        // İlk satırı al ve 'TOTAL' kısmındaki yüzdeyi bul
        var totalLine = lines.FirstOrDefault(line => line.Contains("TOTAL"));
        if (totalLine != null)
        {
            var match = Regex.Match(totalLine, @"(\d+(\.\d+)?)%"); // Yüzdeyi al
            if (match.Success)
            {
                return $"{match.Groups[1].Value}%";
            }
        }

        return "N/A";
    }


    public string GetRamUsage(Device device)
    {
        var output = RunAdbCommand("shell cat /proc/meminfo");
        if (string.IsNullOrWhiteSpace(output))
            return "N/A";

        var lines = output.Split('\n');
        var memTotalLine = lines.FirstOrDefault(line => line.StartsWith("MemTotal"));
        var memAvailableLine = lines.FirstOrDefault(line => line.StartsWith("MemAvailable"));

        if (memTotalLine == null || memAvailableLine == null)
            return "N/A";

        var totalMatch = Regex.Match(memTotalLine, @"\d+");
        var availableMatch = Regex.Match(memAvailableLine, @"\d+");

        if (!totalMatch.Success || !availableMatch.Success)
            return "N/A";

        var total = double.Parse(totalMatch.Value) / 1024;      // kB → MB
        var available = double.Parse(availableMatch.Value) / 1024;
        var used = (total - available) / 1024;

        return $"{used:0.0} GB";
    }

}
