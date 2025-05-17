using APKTool.Utils;
using System.IO;
using static APKTool.Models.ConsoleOutput;
using Activity = APKTool.Models.Activity;

namespace APKTool.Services;

public interface IDecompilerService
{
    Task DecompileApkAsync(string apkPath);
    void Cleanup();
}
public class DecompilerService : IDecompilerService
{
    private readonly INotifierService _notifier;

    public DecompilerService(INotifierService notifier)
    {
        _notifier = notifier;
        _notifier.NotifyConsole($"Decompiler Initialized", OutputType.Debug);
    }

    public async Task DecompileApkAsync(string apkPath)
    {
        string[] apks = Directory.GetFiles(apkPath, "*.apk", SearchOption.AllDirectories);
        if (apks.Length == 0)
        {
            throw new Exception("No APK files found to decompile.");
        }
        _notifier.NotifyConsole($"Decompiling APK files...", OutputType.Debug);
        foreach (string apk in apks)
        {
            
            var (success, output) = await Task.Run(async () =>
            {
                return await ProcessHelper.RunAsync(PathService.ApkToolPath, $"d \"{apk}\" -f -o \"{Path.Combine(apkPath, PathService.DecompiledPath)}/{Path.GetFileNameWithoutExtension(apk)}\"");
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
