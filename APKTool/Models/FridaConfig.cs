using System.IO;
using static APKTool.Services.InjectorService;

namespace APKTool.Models;

public class FridaConfig
{
    public object Script { get; set; }
    public string ScriptFilePath { get; set; }
    public string ScriptFileDest { get; set; }
    public InjectType InjectType { get; set; }
    public FridaConfig(InjectType injectType, string scriptFile, Manifest manifest)
    {
        ScriptFilePath = scriptFile;
        InjectType = injectType;
        ScriptFileDest = $"/sdcard/Android/data/{manifest.PackageName}/{Path.GetFileName(scriptFile)}";
        switch (injectType)
        {
            case InjectType.Debug:
                GenerateListenConfig();
                break;
            case InjectType.Persistent:
                GenerateScriptConfig();
                break;
            default:
                break;
        }
    }
    private void GenerateScriptConfig()
    {
        var config = new
        {
            interaction = new
            {
                type = "script",
                path = ScriptFileDest,
                delay = 5000
            }
        };
        Script = config;
    }

    private void GenerateListenConfig()
    {
        var config = new
        {
            interaction = new
            {
                type = "listen",
                address = "127.0.0.1",
                port = 27042,
                on_load = "wait",
            }
        };
        Script = config;
    }
}