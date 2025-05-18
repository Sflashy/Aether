using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Aether.Models;

public partial class Frida : ObservableObject
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("tag_name")]
    public string Version { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("assets")]
    public Architecture[] Architectures { get; set; }

    [ObservableProperty]
    public partial bool IsInstalled { get; set; }
}

public partial class Architecture : ObservableObject
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("browser_download_url")]
    public string DownloadUrl { get; set; }
    [JsonPropertyName("size")]
    public int Size { get; set; }
    public string ABIName => Regex.Match(Name, @".*android-(.*?)\.").Groups[1].Value;
    [ObservableProperty]
    public partial bool IsInstalled { get; set; }
}
