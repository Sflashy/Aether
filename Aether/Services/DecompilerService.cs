using Aether.Utils;
using System.IO;
using static Aether.Models.ConsoleOutput;
using Activity = Aether.Models.Activity;

namespace Aether.Services;

public interface IDecompilerService
{
    Task DecompileApkAsync(string workspace);
    void Cleanup();
}
public class DecompilerService : IDecompilerService
{
    private readonly INotifierService _notifier;

    public DecompilerService(INotifierService notifier)
    {
        _notifier = notifier;
    }

    public async Task DecompileApkAsync(string workspace)
    {
        string[] apks = Directory.GetFiles(workspace, "*.apk", SearchOption.AllDirectories);
        if (apks.Length == 0)
        {
            throw new Exception("No APK files found to decompile.");
        }
        _notifier.NotifyConsole($"Decompiling APK files...", OutputType.Debug);
        foreach (string apk in apks)
        {
            _notifier.NotifyConsole($"Decompiling {Path.GetFileName(apk)}...", OutputType.Debug);
            
            var (success, output) = await Task.Run(async () =>
            {
                return await ProcessHelper.RunAsync($"java", $"-Xmx4G -jar {PathService.ApkToolJarPath} d \"{apk}\" -f -o \"{Path.Combine(workspace, PathService.DecompiledPath)}/{Path.GetFileNameWithoutExtension(apk)}\"");
            });
            if(!success)
            {
                throw new Exception($"Failed to decompile: {Path.GetFileName(apk)} | {output}");
            }
            Activity activity = new Activity(Path.GetFileName(apk), "apk decompiled", Activity.ActivityType.APK);
            _notifier.NotifyActivity(activity);
        }
    }

    public void Cleanup()
    {
        _notifier.NotifyConsole($"Cleaning decompiled apks...", OutputType.Process);
        Directory.Delete(PathService.DecompiledPath, true);
    }
}
