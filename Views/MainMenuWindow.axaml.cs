using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using SevenZipExt.Services;
using SevenZipExt.ViewModels;

namespace SevenZipExt.Views;

public partial class MainMenuWindow : Window
{
    private readonly SettingsService _settings = null!;
    private readonly SevenZipService _sevenZip = null!;

    public MainMenuWindow()
    {
        InitializeComponent();
    }

    public MainMenuWindow(SettingsService settings, SevenZipService sevenZip) : this()
    {
        _settings = settings;
        _sevenZip = sevenZip;

        var pathBox = this.FindControl<TextBox>("SevenZipPathBox")!;
        pathBox.Text = _settings.Settings.SevenZipPath;

        UpdateContextMenuStatus();

        this.FindControl<Button>("CompressBtn")!.Click += OnCompressClick;
        this.FindControl<Button>("ExtractBtn")!.Click += OnExtractClick;
        this.FindControl<Button>("BrowseSevenZipBtn")!.Click += OnBrowseSevenZip;
        this.FindControl<Button>("SaveSettingsBtn")!.Click += OnSaveSettings;
        this.FindControl<Button>("RegisterBtn")!.Click += OnRegisterContextMenu;
        this.FindControl<Button>("UnregisterBtn")!.Click += OnUnregisterContextMenu;
    }

    private void UpdateContextMenuStatus()
    {
        var statusText = this.FindControl<TextBlock>("ContextMenuStatus");
        if (statusText == null) return;
        bool registered = ContextMenuService.IsRegistered();
        statusText.Text = registered ? "✅ 右键菜单已注册" : "❌ 右键菜单未注册";
    }

    private void OnCompressClick(object? sender, RoutedEventArgs e)
    {
        var vm = new CompressWindowViewModel(_sevenZip, _settings);
        var win = new CompressWindow(vm);
        win.ShowDialog(this);
    }

    private void OnExtractClick(object? sender, RoutedEventArgs e)
    {
        var vm = new ExtractWindowViewModel(_sevenZip);
        var win = new ExtractWindow(vm);
        win.ShowDialog(this);
    }

    private async void OnBrowseSevenZip(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择 7z.exe",
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Executable") { Patterns = new[] { "7z.exe", "*.exe" } }
            }
        });
        if (files?.Count > 0)
        {
            var pathBox = this.FindControl<TextBox>("SevenZipPathBox")!;
            pathBox.Text = files[0].Path.LocalPath;
        }
    }

    private void OnSaveSettings(object? sender, RoutedEventArgs e)
    {
        var pathBox = this.FindControl<TextBox>("SevenZipPathBox")!;
        _settings.Settings.SevenZipPath = pathBox.Text ?? _settings.Settings.SevenZipPath;
        _settings.Save();
        var status = this.FindControl<TextBlock>("RegStatusText")!;
        status.Text = "✅ 设置已保存";
    }

    private void OnRegisterContextMenu(object? sender, RoutedEventArgs e)
    {
        var (ok, msg) = ContextMenuService.Register();
        var status = this.FindControl<TextBlock>("RegStatusText")!;
        status.Text = msg;
        UpdateContextMenuStatus();
    }

    private void OnUnregisterContextMenu(object? sender, RoutedEventArgs e)
    {
        var (ok, msg) = ContextMenuService.Unregister();
        var status = this.FindControl<TextBlock>("RegStatusText")!;
        status.Text = msg;
        UpdateContextMenuStatus();
    }
}
