using System.Diagnostics;
using System.IO;
using System.Text;
using SevenZipExt.Models;

namespace SevenZipExt.Services;

public class SevenZipService
{
    private readonly SettingsService _settingsService;

    public SevenZipService(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    private string SevenZipPath => _settingsService.Settings.SevenZipPath;

    public string[] BuildCompressArgs(CompressionOptions opts)
    {
        bool useUpdateCommand = File.Exists(opts.ArchivePath);
        var args = new List<string> { useUpdateCommand ? "u" : "a" };

        args.Add($"\"{opts.ArchivePath}\"");

        foreach (var src in opts.SourceItems)
            args.Add($"\"{src}\"");

        if (!string.IsNullOrWhiteSpace(opts.Format))
            args.Add($"-t{opts.Format.ToLowerInvariant()}");

        args.Add($"-mx{opts.Level}");

        AppendMethodArgs(args, opts);

        if (!string.IsNullOrWhiteSpace(opts.DictionarySize) && ShouldEmitDictionaryOption(opts))
            args.Add($"-md={opts.DictionarySize}");

        if (!string.IsNullOrWhiteSpace(opts.WordSize))
            args.Add($"-mfb={opts.WordSize}");

        if (!string.IsNullOrWhiteSpace(opts.SolidBlockSize))
            args.Add($"-ms={opts.SolidBlockSize}");

        if (!string.IsNullOrWhiteSpace(opts.Threads))
            args.Add($"-mmt={opts.Threads}");

        if (opts.CreateSfx)
            args.Add("-sfx");

        if (opts.CompressSharedFiles)
            args.Add("-ssw");

        if (opts.DeleteAfterCompress)
            args.Add("-sdel");

        if (!string.IsNullOrWhiteSpace(opts.VolumeSize))
            args.Add($"-v{opts.VolumeSize}");

        if (!string.IsNullOrWhiteSpace(opts.Password))
        {
            args.Add($"-p{opts.Password}");

            if (string.Equals(opts.Format, "zip", StringComparison.OrdinalIgnoreCase))
            {
                var zipEncryption = string.Equals(opts.EncryptionMethod, "ZipCrypto", StringComparison.OrdinalIgnoreCase)
                    ? "ZipCrypto"
                    : "AES256";
                args.Add($"-mem={zipEncryption}");
            }

            if (opts.EncryptFileNames)
                args.Add("-mhe=on");
        }

        if (useUpdateCommand)
        {
            var updateFlag = opts.UpdateMethod switch
            {
                "Add and replace files" => "-up0q0r2x2y2z1w2",
                "Update and add files" => "-up1q0r2x2y2z1w2",
                "Freshen existing files" => "-up1q0r0x0y0z1w2",
                "Synchronize files" => "-up1q0r2x2y2z1w2!",
                _ => ""
            };

            if (!string.IsNullOrWhiteSpace(updateFlag))
            {
                args.Add("-u-");
                args.Add(updateFlag);
            }
        }

        // Path mode
        if (opts.PathMode == "Absolute pathnames")
            args.Add("-spf");
        else if (opts.PathMode == "No pathnames")
            args.Add("-spf2");

        // Exclusions
        foreach (var excl in opts.Exclusions)
        {
            var pattern = excl.IsFolder
                ? Path.GetFileName(excl.Path.TrimEnd('\\', '/', ' '))
                : Path.GetFileName(excl.Path);
            if (!string.IsNullOrWhiteSpace(pattern))
                args.Add(excl.IsRecursive ? $"-xr!\"{pattern}\"" : $"-x!\"{pattern}\"");
        }

        if (!string.IsNullOrWhiteSpace(opts.AdditionalParameters))
            args.Add(opts.AdditionalParameters);

        return args.ToArray();
    }

    private static void AppendMethodArgs(List<string> args, CompressionOptions opts)
    {
        if (string.IsNullOrWhiteSpace(opts.Method))
            return;

        string format = opts.Format.ToLowerInvariant();
        string method = opts.Method;

        if (format == "zip")
        {
            args.Add($"-mm={method}");
            return;
        }

        args.Add($"-m0={method}");
    }

    private static bool ShouldEmitDictionaryOption(CompressionOptions opts)
    {
        if (!string.Equals(opts.Format, "zip", StringComparison.OrdinalIgnoreCase))
            return true;

        return opts.Method switch
        {
            "Deflate" or "Deflate64" => false,
            _ => true
        };
    }

    public string[] BuildExtractArgs(ExtractionOptions opts)
    {
        var args = new List<string>();

        args.Add(opts.FullPaths ? "x" : "e");
        args.Add($"\"{opts.ArchivePath}\"");

        if (!string.IsNullOrWhiteSpace(opts.DestinationPath))
            args.Add($"-o\"{opts.DestinationPath}\"");

        if (!string.IsNullOrWhiteSpace(opts.Password))
            args.Add($"-p{opts.Password}");

        if (opts.OverwriteExisting)
            args.Add("-aoa");
        else
            args.Add("-aos");

        args.Add("-y");

        return args.ToArray();
    }

    public async Task<(bool Success, string Output, string Error)> RunAsync(
        string[] args,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(SevenZipPath))
            return (false, "", $"7-Zip not found: {SevenZipPath}");

        var psi = new ProcessStartInfo
        {
            FileName = SevenZipPath,
            Arguments = string.Join(" ", args),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var sb = new StringBuilder();
        var errSb = new StringBuilder();

        using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data == null) return;
            sb.AppendLine(e.Data);
            progress?.Report(e.Data);
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data == null) return;
            errSb.AppendLine(e.Data);
            progress?.Report($"ERR: {e.Data}");
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken);

        return (process.ExitCode == 0, sb.ToString(), errSb.ToString());
    }

    public async Task<(bool Success, string Output, string Error)> CompressAsync(
        CompressionOptions opts, IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var args = BuildCompressArgs(opts);
        return await RunAsync(args, progress, cancellationToken);
    }

    public async Task<(bool Success, string Output, string Error)> ExtractAsync(
        ExtractionOptions opts, IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var args = BuildExtractArgs(opts);
        return await RunAsync(args, progress, cancellationToken);
    }
}
