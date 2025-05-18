using Aether.Models;
using Newtonsoft.Json;
using System.IO;
using static Aether.Models.ConsoleOutput;

namespace Aether.Services;

public interface IManifestService
{
    Manifest GetMetaData(string folderPath);
}
public class MetaDataService : IManifestService
{
    private readonly INotifierService _notifier;
    public MetaDataService(INotifierService notifier)
    {
        _notifier = notifier;
    }
    public Manifest GetMetaData(string folderPath)
    {
        string manifestFile = Directory.GetFiles(folderPath, "manifest.json", SearchOption.TopDirectoryOnly).FirstOrDefault();
        if (manifestFile == null)
        {
            _notifier.NotifyConsole("Metadata not found", OutputType.Warning);
            return null;
        }
        Manifest manifest = JsonConvert.DeserializeObject<Manifest>(File.ReadAllText(manifestFile));
        _notifier.NotifyConsole($"Architecture detected: {manifest.Architecture}", OutputType.Debug);
        return manifest;
    }

}
