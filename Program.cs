using Avalonia;
using SevenZipExt.Services;

namespace SevenZipExt;

class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        if (args.Length > 0)
        {
            var cmd = args[0].ToLowerInvariant();

            if (cmd == "register")
            {
                var (ok, msg) = ContextMenuService.Register();
                Console.WriteLine(msg);
                return ok ? 0 : 1;
            }

            if (cmd == "unregister")
            {
                var (ok, msg) = ContextMenuService.Unregister();
                Console.WriteLine(msg);
                return ok ? 0 : 1;
            }
        }

        App.StartupArgs = args;

        return BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
