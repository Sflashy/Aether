using Aether.Messages;
using Aether.Models;
using CommunityToolkit.Mvvm.Messaging;
using static Aether.Models.ConsoleOutput;

namespace Aether.Services;
public interface INotifierService
{
    void NotifyConsole(string message, OutputType type);
    void NotifyActivity(Activity activity);
    void NotifyDownloadStatus(double value);
    void NotifyDeviceUpdate(Device device);
    void NotifyDeviceFound(Device device);
}

public class NotifierService : INotifierService
{
    public void NotifyConsole(string message, OutputType type)
    {
        WeakReferenceMessenger.Default.Send(new ConsoleMessage(new ConsoleOutput(message, type)));
    }
    public void NotifyActivity(Activity activity)
    {
        WeakReferenceMessenger.Default.Send(new ActivityMessage(activity));
    }
    public void NotifyDownloadStatus(double value)
    {
        WeakReferenceMessenger.Default.Send(new FridaDownloadStatusMessage(value));
    }
    public void NotifyDeviceFound(Device device)
    {
        WeakReferenceMessenger.Default.Send(new DeviceFoundMessage(device));
    }
    public void NotifyDeviceUpdate(Device device)
    {
        WeakReferenceMessenger.Default.Send(new DeviceUpdateMessage(device));
    }
}