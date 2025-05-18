using Aether.Models;
using System.IO;

namespace Aether.Services;

public class PathService
{
    public static string FridaGadgetDirectory => "frida-gadget";
    public static string LibDirectory => "lib";
    public static string DecompiledPath => "decompiled";
    public static string ApkToolPath => Path.Combine(LibDirectory, "apktool.bat");
    public static string CompiledPath => "compiled";
    public static string AdbPath => Path.Combine(LibDirectory, "adb.exe");
    public static string FridaConfigFile => Path.Combine(FridaGadgetDirectory, "frida-gadget.config");
    public static string KeyStoreFile => Path.Combine(LibDirectory, "roanoke.keystore");
}

