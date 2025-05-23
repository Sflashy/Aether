using Aether.Models;
using static Aether.Models.ConsoleOutput;

namespace Aether.Services;

public interface IApkProcessingService
{
    Task DecompileApkAsync();
    Task CompileApkAsync();
    Task SignApkAsync();
    Task InstallApksAsync();
    void Initialize(string workspace, Device device);
}

public class ApkProcessingService : IApkProcessingService
{
    private readonly IDecompilerService _decompiler;
    private readonly ICompilerService _compiler;
    private readonly ISignerService _signer;
    private readonly IAdbService _adb;
    private string _workspace;
    private Device _device;
    public ApkProcessingService(
        IDecompilerService decompiler, 
        ICompilerService compiler,
        ISignerService signer, 
        IAdbService adbService)
    {
        _decompiler = decompiler;
        _compiler = compiler;
        _signer = signer;
        _adb = adbService;
    }

    public void Initialize(string workspace, Device device)
    {
        _workspace = workspace;
        _device = device;
    }
    public async Task DecompileApkAsync()
    {
        await _decompiler.DecompileApkAsync(_workspace);
    }

    public async Task CompileApkAsync()
    {
        await _compiler.CompileApkAsync(_workspace);
    }

    public async Task SignApkAsync()
    {
        await _signer.SignApkAsync(_workspace);
    }

    public async Task InstallApksAsync()
    {
        bool isAdbConnected = _adb.IsAdbConnected(_device.Id);
        if (!isAdbConnected)
        {
            throw new Exception("Failed to install APKs. ADB is not connected.");
        }
        await _adb.InstallApksAsync(_workspace);
    }
}







