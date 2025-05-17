using APKTool.Models;
using Newtonsoft.Json;
using System.IO;
using static APKTool.Models.ConsoleOutput;

namespace APKTool.Services;

public interface IMetaDataService
{
    Manifest GetMetaData(string folderPath);
}
public class MetaDataService : IMetaDataService
{
    private readonly INotifierService _notifier;
    public MetaDataService(INotifierService notifier)
    {
        _notifier = notifier;
    }
    public Manifest GetMetaData(string folderPath)
    {
        string manifestFile = Directory.GetFiles(folderPath, "manifest.json", SearchOption.TopDirectoryOnly).FirstOrDefault();
        if (manifestFile == null) return null;
        Manifest manifest = JsonConvert.DeserializeObject<Manifest>(File.ReadAllText(manifestFile));
        _notifier.NotifyConsole($"Architecture found: {manifest.Architecture}", OutputType.Debug);
        return manifest;
    }

}
