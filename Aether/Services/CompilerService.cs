using Aether.Models;
using Aether.Utils;
using System.IO;
using static Aether.Models.ConsoleOutput;

namespace Aether.Services;

public interface ICompilerService
{
    Task CompileApkAsync(string apkPath);
}
public class CompilerService : ICompilerService
{
    private readonly INotifierService _notifier;

    public CompilerService(INotifierService notifier)
    {
        _notifier = notifier;
    }

    public async Task CompileApkAsync(string apkPath)
    {
        string[] directories = Directory.GetDirectories(Path.Combine(apkPath, PathService.DecompiledPath), "*", SearchOption.TopDirectoryOnly);
        if (directories.Length == 0)
        {
            throw new Exception("No decompiled apks found to compile.");
        }

        _notifier.NotifyConsole($"Recompiling APK files...", OutputType.Debug);

        foreach (string folder in directories)
        {
            string folderName = Path.GetFileName(folder);
            string outputFile = Path.Combine(apkPath, PathService.CompiledPath, $"{folderName}.apk");
            

            var (success, output) = await Task.Run(async () =>
            {
                return await ProcessHelper.RunAsync(PathService.ApkToolPath, $"b \"{folder}\" -o \"{outputFile}\"");
            });

            if (!success)
            {
                throw new Exception($"Failed to compile: {folderName}.apk | {output}");
            }

            Activity activity = new Activity($"{folderName}.apk", "apk compiled", Activity.ActivityType.APK);
            _notifier.NotifyActivity(activity);
        }
    }
}

