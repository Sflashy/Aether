﻿using Aether.Services;
using System.Diagnostics;
using System.IO;

namespace Aether.Utils;

public static class ProcessHelper
{
    public static async Task<(bool success, string output)> RunAsync(string fileName, string arguments)
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), PathService.LibDirectory),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            await process.WaitForExitAsync();

            return (string.IsNullOrEmpty(error), error);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}

