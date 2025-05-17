using APKTool.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace APKTool.Models;

public partial class Frida : ObservableObject
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("tag_name")]
    public string Version { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("assets")]
    public FridaAsset[] Assets { get; set; }

    [JsonIgnore]
    public string[] Architectures => Assets
    .Select(a =>
    {
        var match = Regex.Match(a.Name, @"frida-gadget-[^-\s]+-android-(.+?)\.so");
        return match.Success ? match.Groups[1].Value : null;
    })
    .Where(arch => arch != null)
    .Distinct()
    .ToArray();

    [ObservableProperty]
    public partial string Architecture { get; set; }


}

public class FridaAsset
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("browser_download_url")]
    public string DownloadUrl { get; set; }
}
