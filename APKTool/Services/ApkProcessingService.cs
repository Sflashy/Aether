using APKTool.Models;
using static APKTool.Models.ConsoleOutput;

namespace APKTool.Services;

public interface IApkProcessingService
{
    Task DecompileApkAsync();
    Task CompileApkAsync();
    Task SignApkAsync();
    Task InstallApksAsync(Device device);
    void Initialize(string apkPath);
}

public class ApkProcessingService : IApkProcessingService
{
    private readonly IDecompilerService _decompiler;
    private readonly ICompilerService _compiler;
    private readonly ISignerService _signer;
    private readonly IAdbService _adb;
    private readonly INotifierService _notifier;
    private string _apkPath;
    public ApkProcessingService(
        IDecompilerService decompiler, 
        ICompilerService compiler,
        ISignerService signer, 
        IAdbService adbService,
        INotifierService notifier)
    {
        _decompiler = decompiler;
        _compiler = compiler;
        _signer = signer;
        _adb = adbService;
        _notifier = notifier;
    }

    public void Initialize(string apkPath)
    {
        _apkPath = apkPath;
    }
    public async Task DecompileApkAsync()
    {
        await _decompiler.DecompileApkAsync(_apkPath);
    }

    public async Task CompileApkAsync()
    {
        await _compiler.CompileApkAsync(_apkPath);
    }

    public async Task SignApkAsync()
    {
        await _signer.SignApkAsync(_apkPath);
    }

    public async Task InstallApksAsync(Device device)
    {
        if (!_adb.IsAdbConnected(device.Id))
        {
            throw new Exception("Failed to install APKs. ADB is not connected.");
        }
        await _adb.InstallApksAsync(_apkPath);
    }
}







