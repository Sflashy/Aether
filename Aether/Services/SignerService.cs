﻿using Aether.Utils;
using System.IO;
using static Aether.Models.ConsoleOutput;

namespace Aether.Services;

public interface ISignerService
{
    Task SignApkAsync(string apkPath);
}
public class SignerService : ISignerService
{
    private const string KeyPassword = "tempkeypassword";
    private const string KeyStorePassword = "tempkeystorepassword";
    private const string Alias = "roanoke";

    private readonly INotifierService _notifier;
    public SignerService(INotifierService notifier)
    {
        _notifier = notifier;
    }

    public async Task SignApkAsync(string apkPath)
    {
        if (!File.Exists(PathService.KeyStoreFile))
        {
            _notifier.NotifyConsole($"Keystore not found, creating... {PathService.KeyStoreFile}", OutputType.Process);

            await CreateKeystoreAsync();
        }

        var apks = Directory.GetFiles(Path.Combine(apkPath, PathService.CompiledPath), "*.apk", SearchOption.AllDirectories);
        _notifier.NotifyConsole($"Signing APK files...", OutputType.Debug);
        foreach (var apkFile in apks)
        {
            _notifier.NotifyConsole($"Signing {apkFile}...", OutputType.Debug);
            var signCommand = $"{PathService.APKSignerPath} sign --ks \"{PathService.KeyStoreFile}\" --ks-key-alias {Alias} --ks-pass pass:{KeyStorePassword} --key-pass pass:{KeyPassword} --out \"{Path.Combine(apkPath, PathService.CompiledPath)}\\{Path.GetFileName(apkFile)}\" \"{apkFile}\"";

            var (success, output) = await Task.Run(() => ProcessHelper.RunAsync("cmd.exe",$"/C {signCommand}"));
            if (!success)
            {
                throw new Exception($"Error occurred while signing {Path.GetFileName(apkFile)}. {output}");
            }
        }
    }

    private async Task CreateKeystoreAsync()
    {
        var createCommand = new string[]
        {
            "keytool", "-genkey", "-v", "-keystore", PathService.KeyStoreFile, "-alias", Alias,
            "-keyalg", "RSA", "-keysize", "2048", "-validity", "10000",
            "-storepass", KeyStorePassword, "-keypass", KeyPassword,
            "-dname", "CN=Roanoke"
        };

        var (success, output) = await Task.Run(() => ProcessHelper.RunAsync("cmd.exe", $"/C {string.Join(" ", createCommand)}"));
        if (!success)
        {
            _notifier.NotifyConsole($"Failed to create keystore: {output}", OutputType.Error);
        }
        else
        {
            _notifier.NotifyConsole($"Keystore created at {PathService.KeyStoreFile}.", OutputType.Success);
        }
    }
}

