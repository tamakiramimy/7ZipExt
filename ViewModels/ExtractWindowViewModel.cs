using System.IO;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SevenZipExt.Models;
using SevenZipExt.Services;

namespace SevenZipExt.ViewModels;

public partial class ExtractWindowViewModel : ViewModelBase
{
    private readonly SevenZipService _sevenZip;

    [ObservableProperty] private string _archivePath = "";
    [ObservableProperty] private string _destinationPath = "";
    [ObservableProperty] private bool _createSubfolder = true;
    [ObservableProperty] private bool _overwriteExisting = true;
    [ObservableProperty] private bool _fullPaths = true;
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private bool _showPassword;
    [ObservableProperty] private bool _isRunning;
    [ObservableProperty] private string _progressText = "";

    public Action? CloseAction { get; set; }
    public Func<Task<IStorageFile?>>? BrowseArchiveAction { get; set; }
    public Func<Task<IStorageFolder?>>? BrowseDestinationAction { get; set; }

    public ExtractWindowViewModel(SevenZipService sevenZip)
    {
        _sevenZip = sevenZip;
    }

    public void SetArchive(string path)
    {
        ArchivePath = path;
        var dir = Path.GetDirectoryName(path) ?? "";
        var name = Path.GetFileNameWithoutExtension(path);
        DestinationPath = CreateSubfolder ? Path.Combine(dir, name) : dir;
    }

    partial void OnArchivePathChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        var dir = Path.GetDirectoryName(value) ?? "";
        var name = Path.GetFileNameWithoutExtension(value);
        DestinationPath = CreateSubfolder ? Path.Combine(dir, name) : dir;
    }

    partial void OnCreateSubfolderChanged(bool value)
    {
        if (string.IsNullOrWhiteSpace(ArchivePath)) return;
        var dir = Path.GetDirectoryName(ArchivePath) ?? "";
        var name = Path.GetFileNameWithoutExtension(ArchivePath);
        DestinationPath = value ? Path.Combine(dir, name) : dir;
    }

    [RelayCommand]
    private async Task BrowseArchive()
    {
        if (BrowseArchiveAction == null) return;
        var file = await BrowseArchiveAction();
        if (file != null) ArchivePath = file.Path.LocalPath;
    }

    [RelayCommand]
    private async Task BrowseDestination()
    {
        if (BrowseDestinationAction == null) return;
        var folder = await BrowseDestinationAction();
        if (folder != null) DestinationPath = folder.Path.LocalPath;
    }

    [RelayCommand]
    private async Task Extract()
    {
        if (string.IsNullOrWhiteSpace(ArchivePath)) return;

        IsRunning = true;
        ProgressText = "正在解压...";

        try
        {
            var opts = new ExtractionOptions
            {
                ArchivePath = ArchivePath,
                DestinationPath = DestinationPath,
                CreateSubfolder = CreateSubfolder,
                Password = Password,
                OverwriteExisting = OverwriteExisting,
                FullPaths = FullPaths,
            };

            var progress = new Progress<string>(line => ProgressText = line);
            var (success, _, error) = await _sevenZip.ExtractAsync(opts, progress);
            ProgressText = success ? "✅ 解压完成！" : $"❌ 失败: {error}";
        }
        finally
        {
            IsRunning = false;
        }
    }

    [RelayCommand]
    private void Cancel() => CloseAction?.Invoke();
}
