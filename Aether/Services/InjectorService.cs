using Aether.Models;
using System.IO;
using System.Text.Json;
using System.Windows;
using static Aether.Models.ConsoleOutput;
using static Aether.Services.InjectorService;

namespace Aether.Services;

public interface IInjectorService
{
    void Initialize(string workspace, Frida frida, FridaConfig config);
    Task InjectFridaAsync();
    Task PathchSmaliAsync();
}
public class InjectorService : IInjectorService
{
    public enum InjectionType
    {
        Debug,
        Persistent
    }
   
    private string _workspace;
    private FridaConfig _config;
    private Frida _frida;
    private readonly INotifierService _notifier;
    private readonly IAdbService _adbService;
    public InjectorService(INotifierService notifier, IAdbService adbService)
    {
        _notifier = notifier;
        _adbService = adbService;

    }
    public void Initialize(string workspace, Frida frida, FridaConfig config)
    {
        _frida = frida;
        _workspace = workspace;
        _config = config;
    }
    public async Task InjectFridaAsync()
    {
        _notifier.NotifyConsole("Injecting frida gadget libraries...", OutputType.Info);
        var decompiledDirs = Directory.GetDirectories(Path.Combine(_workspace, PathService.DecompiledPath));

        foreach (var dir in decompiledDirs)
        {
            var libDir = Path.Combine(dir, "lib");
            if (!Directory.Exists(libDir)) continue;
            var abis = Directory.GetDirectories(libDir)
                                .Select(Path.GetFileName)
                                .Where(abi => !string.IsNullOrWhiteSpace(abi))
                                .ToList();

            if (abis.Count == 0)
            {
                _notifier.NotifyConsole($"No ABI folders found in {libDir}", OutputType.Warning);
                continue;
            }

            foreach (var abi in abis)
            {
                _notifier.NotifyConsole($"Detecting support for ABI: {abi}", OutputType.Debug);

                var gadgetSource = GetFridaGadgetForAbi(_frida, abi);

                if (string.IsNullOrEmpty(gadgetSource))
                {
                    _notifier.NotifyConsole($"Missing frida-gadget for {abi}. Please download manually.", OutputType.Warning);
                    continue;
                }

                var destPath = Path.Combine(libDir, abi, "libfrida-gadget.so");
                var fridaGadgetPath = Path.Combine(PathService.FridaGadgetDirectory, gadgetSource);
                if (File.Exists(fridaGadgetPath))
                {
                    File.Copy(fridaGadgetPath, destPath, true);
                    await CreateFridaConfigFileAsync(destPath);
                    //TODO: if the game has not installed this will fail. (no game folder)
                    InjectFridaScriptAsync();
                }
                else
                {
                    throw new Exception($"frida-gadget for {abi} not found.");
                }
            }
        }
    }
    private void InjectFridaScriptAsync()
    {
        if (_config.InjectType != InjectionType.Persistent) return;
        _notifier.NotifyConsole($"Injecting {Path.GetFileName(_config.ScriptFilePath)}...", OutputType.Info);
        _adbService.PushFile(_config.ScriptFilePath, _config.ScriptFileDest);

    }
    private async Task CreateFridaConfigFileAsync(string destPath)
    {
        try
        {
            _notifier.NotifyConsole("Generating frida-gadget-config.so file...", OutputType.Debug);
            string directory = Path.GetDirectoryName(destPath);
            string configFilePath = Path.Combine(directory, "libfrida-gadget.config.so");

            string json = JsonSerializer.Serialize(_config.Script, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(configFilePath, json);

        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create Frida config file: {ex.Message}");
        }
    }
    public async Task PathchSmaliAsync()
    {
        _notifier.NotifyConsole("Modifying UnityPlayerActivity.smali...", OutputType.Debug);
        string projectDir = Path.Combine(_workspace, PathService.DecompiledPath);
        string filePath = Directory.EnumerateFiles(projectDir, "UnityPlayerActivity.smali", SearchOption.AllDirectories)
                                   .FirstOrDefault();

        if (filePath == null)
        {
            var msgBox = MessageBox.Show("UnityPlayerActivity.smali not found. Do you want to continue building?", "File Not Found", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msgBox == MessageBoxResult.Yes)
            {
                _notifier.NotifyConsole("UnityPlayerActivity.smali not found. Continuing without modification.", OutputType.Warning);
                return;
            }
            else
            {
                throw new Exception("File UnityPlayerActivity.smali not found.");
            }
            
        }

        string[] lines = await File.ReadAllLinesAsync(filePath);
        var newLines = new List<string>
        {
            "    const-string v0, \"frida-gadget\"",
            "    invoke-static {v0}, Ljava/lang/System;->loadLibrary(Ljava/lang/String;)V"
        };

        string startLine = ".locals 2";
        string endLine = "const/4 v0, 0x1";

        using StreamWriter writer = new(filePath, false);
        bool inTargetSection = false;

        foreach (string line in lines)
        {
            if (line.Contains(startLine))
            {
                inTargetSection = true;
                await writer.WriteLineAsync(line);
                continue;
            }
            else if (inTargetSection && line.Contains(endLine))
            {
                foreach (var newLine in newLines)
                    await writer.WriteLineAsync(newLine);

                inTargetSection = false;
            }

            await writer.WriteLineAsync(line);
        }
    }
    private string GetFridaGadgetForAbi(Frida fridaVersion, string abi)
    {
        var normalizedAbi = NormalizeAbi(abi);

        var asset = fridaVersion.Architectures.FirstOrDefault(a =>
            a.Name.Contains("frida-gadget") &&
            a.Name.Contains(normalizedAbi));

        if (asset != null)
        {
            return asset.Name.Replace(".xz", "");
        }

        return null;
    }
    private string NormalizeAbi(string abi)
    {
        var abiMap = new Dictionary<string, string>
        {
            { "arm64-v8a", "arm64" },
            { "armeabi-v7a", "arm" },
            { "x86", "x86" },
            { "x86_64", "x86_64" },
        };

        if (abiMap.TryGetValue(abi, out string value))
        {
            return value;
        }

        return abi;
    }

}
