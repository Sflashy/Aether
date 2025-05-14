using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace APKTool.Models;

public class Manifest
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("package_name")]
    public string PackageName { get; set; }
    [JsonProperty("version_name")]
    public string VersionName { get; set; }
    [JsonProperty("min_sdk_version")]
    public string MinSDKVersion { get; set; }
    [JsonProperty("target_sdk_version")]
    public string TargetSDKVersion { get; set; }
    [JsonProperty("total_size")]
    public int TotalSize { get; set; }
    [JsonProperty("split_apks")]
    public SplitAPks[] APks { get; set; }

    [JsonIgnore]
    public string Architecture => APks.Select(a =>
    {
        var match = Regex.Match(a.Id, @"^config\.(.*?)_.*");
        return match.Success ? match.Groups[1].Value : null;
    })
    .Where(arch => arch != null).FirstOrDefault();

    public class SplitAPks
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("file")]
        public string File { get; set; }
    }


}

