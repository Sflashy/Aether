using Aether.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using static Aether.Models.ConsoleOutput;

namespace Aether.Services;

public interface IFridaService
{
    Task DownloadAndExtractAsync(Frida frida, Architecture architecture);
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

    public async Task DownloadAndExtractAsync(Frida frida, Architecture architecture)
    {
        _notifier.NotifyDownloadStatus(1);
        _notifier.NotifyConsole($"Downloading frida-gadget version {frida.Version} for {architecture.ABIName}...", OutputType.Debug);

        try
        {
            var (fridaGadgetUrl, outputFile) = GetFridaGadgetDownloadUrlAndPath(architecture);

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
            frida.IsInstalled = true;
            architecture.IsInstalled = true;
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
        _notifier.NotifyDownloadStatus(1);
        //_notifier.NotifyConsole("Checking for latest frida-gadget version...", OutputType.Debug);
        HttpResponseMessage response = await _httpClient.GetAsync("https://api.github.com/repos/frida/frida/releases");
        if (!response.IsSuccessStatusCode)
        {
            _notifier.NotifyConsole($"Failed to check for frida-gadget versions due to {response.ReasonPhrase.ToLower()}", OutputType.Error);
            return [];
        }
        
        var data = await response.Content.ReadAsStringAsync();
        Frida[] fridas = JsonConvert.DeserializeObject<Frida[]>(data);
        foreach (var frida in fridas)
        {
            frida.Architectures = frida.Architectures.Where(arch => arch.Name.StartsWith($"frida-gadget-{frida.Version}-android")).ToArray();
        }


        fridas[0].Version = fridas[0].Version + " (latest)";
        //_notifier.NotifyConsole($"Latest frida-gadget version found: {fridas[0].Version}", OutputType.Info);
        _notifier.NotifyDownloadStatus(0);
        return fridas;
    }

    private (string fridaGadgetUrl, string outputFile) GetFridaGadgetDownloadUrlAndPath(Architecture architecture)
    {
        string fridaGadgetUrl = architecture.DownloadUrl;
        string fileName = Path.GetFileName(fridaGadgetUrl);
        string outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathService.FridaGadgetDirectory);

        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        string outputFile = Path.Combine(outputDir, fileName);
        return (fridaGadgetUrl, outputFile);
    }


}
