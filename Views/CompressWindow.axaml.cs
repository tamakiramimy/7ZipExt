using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using SevenZipExt.ViewModels;

namespace SevenZipExt.Views;

public partial class CompressWindow : Window
{
    public CompressWindow()
    {
        InitializeComponent();
    }

    public CompressWindow(CompressWindowViewModel vm) : this()
    {
        DataContext = vm;
        vm.CloseAction = Close;
        vm.ExitApplicationAction = CloseAndExitApplication;
        vm.BrowseArchiveFolderAction = BrowseForArchiveFolder;
        vm.PickFilesAction = BrowseForFiles;
        vm.PickFoldersAction = BrowseForFolders;

        this.Opened += (_, _) => UpdateCommandPreview();
        vm.PropertyChanged += (_, _) => UpdateCommandPreview();
        vm.Exclusions.CollectionChanged += (_, _) => UpdateCommandPreview();
        vm.SourceItems.CollectionChanged += (_, _) => UpdateCommandPreview();
    }

    private void UpdateCommandPreview()
    {
        if (DataContext is not CompressWindowViewModel vm) return;
        var preview = this.FindControl<TextBlock>("CommandPreview");
        if (preview != null) preview.Text = vm.GetCommandPreview();
    }

    private void CloseAndExitApplication()
    {
        Close();

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    private async Task<IStorageFolder?> BrowseForArchiveFolder()
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "选择压缩包保存目录",
            AllowMultiple = false
        });

        return folders?.Count > 0 ? folders[0] : null;
    }

    private async Task<IReadOnlyList<IStorageFile>?> BrowseForFiles(string title)
    {
        return await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = true,
            FileTypeFilter = new[] { FilePickerFileTypes.All }
        });
    }

    private async Task<IReadOnlyList<IStorageFolder>?> BrowseForFolders(string title)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = true
        });
        return folders?.Count > 0 ? folders : null;
    }

    private void OnSourceSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not CompressWindowViewModel vm || sender is not ListBox listBox) return;
        vm.SetSelectedSourceItems(listBox.SelectedItems?.OfType<string>() ?? Enumerable.Empty<string>());
    }

    private void OnExclusionSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not CompressWindowViewModel vm || sender is not ListBox listBox) return;
        vm.SetSelectedExclusions(listBox.SelectedItems?.OfType<Models.ExclusionItem>() ?? Enumerable.Empty<Models.ExclusionItem>());
    }
}
