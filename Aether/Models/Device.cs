using Aether.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.RegularExpressions;

namespace Aether.Models;

public partial class Device : ObservableObject
{
    public string Id { get; set; }
    public string Model { get; set; }
    public string Version { get; set; }
    public string API { get; set; }
    public string Android => $"{Version} (API {API})";
    public string Architecture { get; set; }

    [ObservableProperty]
    public partial string Status { get; set; } = "Disconnected";
    [ObservableProperty]
    public partial string USBDebuggingStatus { get; set; } = "Disabled";

    [ObservableProperty]
    public partial string CpuUsage { get; set; }

    [ObservableProperty]
    public partial string RamUsage { get; set; }
}
