using APKTool.Messages;
using APKTool.Models;
using CommunityToolkit.Mvvm.Messaging;
using static APKTool.Models.ConsoleOutput;

namespace APKTool.Services;
public interface INotifierService
{
    void NotifyConsole(string message, OutputType type);
    void NotifyActivity(Activity activity);
    void NotifyDownloadStatus(double value);
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
        WeakReferenceMessenger.Default.Send(new DownloadStatusMessage(value));
    }
}