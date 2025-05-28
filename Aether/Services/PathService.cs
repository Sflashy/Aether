using Aether.Models;
using System.IO;

namespace Aether.Services;

public class PathService
{
    public static string FridaGadgetDirectory => "frida-gadget";
    public static string LibDirectory => "lib";
    public static string DecompiledPath => "decompiled";
    public static string ApkToolJarPath => Path.Combine(Directory.GetCurrentDirectory(), LibDirectory, "apktool_2.11.1.jar");
    public static string ApkToolPath => Path.Combine(Directory.GetCurrentDirectory(), LibDirectory, "apktool.bat");
    public static string CompiledPath => "compiled";
    public static string AdbPath => Path.Combine(Directory.GetCurrentDirectory(), LibDirectory, "adb.exe");
    public static string APKSignerPath => Path.Combine(Directory.GetCurrentDirectory(), LibDirectory, "apksigner.bat");
    public static string FridaConfigFile => Path.Combine(FridaGadgetDirectory, "frida-gadget.config");
    public static string KeyStoreFile => Path.Combine(Directory.GetCurrentDirectory(), LibDirectory, "roanoke.keystore");
}

