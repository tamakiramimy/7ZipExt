using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using SevenZipExt.ViewModels;

namespace SevenZipExt.Views;

public partial class ExtractWindow : Window
{
    public ExtractWindow()
    {
        InitializeComponent();
    }

    public ExtractWindow(ExtractWindowViewModel vm) : this()
    {
        DataContext = vm;
        vm.CloseAction = Close;
        vm.BrowseArchiveAction = BrowseForArchive;
        vm.BrowseDestinationAction = BrowseForFolder;
    }

    private async Task<IStorageFile?> BrowseForArchive()
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择压缩包",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Archive Files")
                {
                    Patterns = new[] { "*.7z", "*.zip", "*.tar", "*.gz", "*.bz2", "*.rar", "*.xz" }
                },
                FilePickerFileTypes.All
            }
        });
        return files?.Count > 0 ? files[0] : null;
    }

    private async Task<IStorageFolder?> BrowseForFolder()
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "选择解压目标文件夹"
        });
        return folders?.Count > 0 ? folders[0] : null;
    }
}
