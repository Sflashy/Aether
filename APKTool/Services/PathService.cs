using APKTool.Models;
using System.IO;

namespace APKTool.Services;

public class PathService
{

    public static string ApkOutputDir(string packageName)
    {
        return Path.Combine("decompiled", packageName);
    }

    public static string FridaGadgetDirectory => "frida-gadget";
    public static string LibDirectory => "lib";
    public static string DecompiledPath => "decompiled";
    public static string ApkToolPath => Path.Combine("lib", "apktool.bat");
    public static string CompiledPath => "compiled";
    public static string FridaConfigFile => Path.Combine(FridaGadgetDirectory, "frida-gadget.config");
    public static string KeyStoreFile => Path.Combine(LibDirectory, "roanoke.keystore");
}

