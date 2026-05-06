using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SevenZipExt.Services;
using SevenZipExt.ViewModels;
using SevenZipExt.Views;

namespace SevenZipExt;

public partial class App : Application
{
    public static string[] StartupArgs { get; set; } = Array.Empty<string>();

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var settingsService = new SettingsService();
            settingsService.Load();
            var sevenZipService = new SevenZipService(settingsService);

            var args = StartupArgs;
            var cmd = args.Length > 0 ? args[0].ToLowerInvariant() : "";
            var files = args.Skip(1).ToArray();

            if (cmd == "compress")
            {
                var vm = new CompressWindowViewModel(sevenZipService, settingsService);
                vm.AddSourceItems(files);
                desktop.MainWindow = new CompressWindow(vm);
            }
            else if (cmd == "extract")
            {
                var vm = new ExtractWindowViewModel(sevenZipService);
                if (files.Length > 0) vm.SetArchive(files[0]);
                desktop.MainWindow = new ExtractWindow(vm);
            }
            else
            {
                desktop.MainWindow = new MainMenuWindow(settingsService, sevenZipService);
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
