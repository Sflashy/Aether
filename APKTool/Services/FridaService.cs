using APKTool.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using static APKTool.Models.ConsoleOutput;

namespace APKTool.Services;

public interface IFridaService
{
    Task DownloadAndExtractAsync(Frida frida);
    Task<Frida[]> GetFridaVersionsAsync();
}

public class FridaService : IFridaService
{
    private static readonly HttpClient _httpClient = new HttpClient
    {
        DefaultRequestHeaders = { { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:138.0) Gecko/20100101 Firefox/138.0" } }
    };

    private readonly INotifierService _notifier;

    public FridaService(INotifierService notifier)
    {
        _notifier = notifier;
    }

    public async Task DownloadAndExtractAsync(Frida fridaVersion)
    {
        _notifier.NotifyDownloadStatus(1);
        _notifier.NotifyConsole($"Downloading frida-gadget version {fridaVersion.TagName} for {fridaVersion.Architecture}...", OutputType.Debug);

        try
        {
            var (fridaGadgetUrl, outputFile) = GetFridaGadgetDownloadUrlAndPath(fridaVersion);

            using var response = await _httpClient.GetAsync(fridaGadgetUrl, HttpCompletionOption.ResponseHeadersRead);
            if (!response.IsSuccessStatusCode)
            {
                _notifier.NotifyConsole($"Failed to download frida-gadget: {response.ReasonPhrase}", OutputType.Error);
                return;
            }

            await using (var contentStream = await response.Content.ReadAsStreamAsync())
            await using (var fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await contentStream.CopyToAsync(fileStream);
            }

            var outputDir = Path.GetDirectoryName(outputFile)!;
            var extractedFilePath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(outputFile));

            using var xzStream = File.OpenRead(outputFile);
            using var xzInput = new SharpCompress.Compressors.Xz.XZStream(xzStream);
            using var extractedFile = File.Create(extractedFilePath);
            await xzInput.CopyToAsync(extractedFile);
            _notifier.NotifyConsole($"{Path.GetFileName(extractedFile.Name)} successfully downloaded", OutputType.Success);
            await xzStream.DisposeAsync();
            File.Delete(outputFile);
        }
        catch (Exception ex)
        {
            _notifier.NotifyConsole($"Download or extraction failed: {ex.Message}", OutputType.Error);
        }
        finally
        {
            _notifier.NotifyDownloadStatus(0);
        }
    }

    public async Task<Frida[]> GetFridaVersionsAsync()
    {
        _notifier.NotifyConsole("Checking for frida-gadget versions...", OutputType.Debug);
        HttpResponseMessage response = await _httpClient.GetAsync("https://api.github.com/repos/frida/frida/releases?per_page=10");
        if (!response.IsSuccessStatusCode)
        {
            _notifier.NotifyConsole($"Failed to check for frida-gadget versions due to {response.ReasonPhrase.ToLower()}", OutputType.Error);
            return [];
        }
        var data = await response.Content.ReadAsStringAsync();
        Frida[] fridaVersions = JsonConvert.DeserializeObject<Frida[]>(data);
        fridaVersions[0].TagName = fridaVersions[0].TagName + " (latest)";
        _notifier.NotifyConsole($"Latest frida-gadget version: {fridaVersions[0].TagName}", OutputType.Debug);
        return fridaVersions;
    }

    private (string fridaGadgetUrl, string outputFile) GetFridaGadgetDownloadUrlAndPath(Frida frida)
    {
        string fridaGadgetUrl = frida.Assets.First(x => x.Name.Contains("frida-gadget") && x.Name.Contains(frida.Architecture)).DownloadUrl;
        string fileName = Path.GetFileName(fridaGadgetUrl);
        string outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathService.FridaGadgetDirectory);

        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        string outputFile = Path.Combine(outputDir, fileName);
        return (fridaGadgetUrl, outputFile);
    }


}
