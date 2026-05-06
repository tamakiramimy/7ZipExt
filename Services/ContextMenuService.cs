using System.IO;
using Microsoft.Win32;

namespace SevenZipExt.Services;

public class ContextMenuService
{
    private const string MenuName = "7ZipExt";
    private const string MenuLabel = "7ZipExt";

    private static string GetExePath()
    {
        var assemblyLocation = typeof(ContextMenuService).Assembly.Location;
        if (!string.IsNullOrWhiteSpace(assemblyLocation))
        {
            var assemblyDir = Path.GetDirectoryName(assemblyLocation);
            if (!string.IsNullOrWhiteSpace(assemblyDir))
            {
                var siblingExe = Path.Combine(assemblyDir, "7ZipExt.exe");
                if (File.Exists(siblingExe))
                    return siblingExe;
            }
        }

        var processPath = Environment.ProcessPath;
        if (!string.IsNullOrWhiteSpace(processPath)
            && File.Exists(processPath)
            && !string.Equals(Path.GetFileName(processPath), "dotnet.exe", StringComparison.OrdinalIgnoreCase))
            return processPath;

        var currentExe = Path.Combine(AppContext.BaseDirectory, "7ZipExt.exe");
        if (File.Exists(currentExe))
            return currentExe;

        var windowsBuildExe = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "net10.0-windows", "7ZipExt.exe"));
        if (File.Exists(windowsBuildExe))
            return windowsBuildExe;

        return currentExe;
    }

    public static bool IsRegistered()
    {
        if (!OperatingSystem.IsWindows()) return false;
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                $@"Software\Classes\*\shell\{MenuName}\shell");
            return key != null;
        }
        catch { return false; }
    }

    public static (bool Success, string Message) Register()
    {
        if (!OperatingSystem.IsWindows())
            return (false, "Context menu registration is only supported on Windows.");

        try
        {
            var exePath = GetExePath();

            RegisterCascadeMenu(@"Software\Classes\*\shell", exePath, "%1", true);
            RegisterCascadeMenu(@"Software\Classes\Directory\shell", exePath, "%1", true);
            RegisterCascadeMenu(@"Software\Classes\Folder\shell", exePath, "%1", true);
            RegisterCascadeMenu(@"Software\Classes\Drive\shell", exePath, "%1", false);
            RegisterCascadeMenu(@"Software\Classes\Directory\Background\shell", exePath, "%V", false);

            return (true, "右键菜单注册成功！");
        }
        catch (Exception ex)
        {
            return (false, $"注册失败: {ex.Message}");
        }
    }

    public static (bool Success, string Message) Unregister()
    {
        if (!OperatingSystem.IsWindows())
            return (false, "Only supported on Windows.");

        try
        {
            foreach (var baseKey in new[]
                { @"*\shell", @"Directory\shell", @"Folder\shell", @"Drive\shell", @"Directory\Background\shell" })
            {
                Registry.CurrentUser.DeleteSubKeyTree(
                    $@"Software\Classes\{baseKey}\{MenuName}",
                    throwOnMissingSubKey: false);
            }

            return (true, "右键菜单已卸载。");
        }
        catch (Exception ex)
        {
            return (false, $"卸载失败: {ex.Message}");
        }
    }

    private static void RegisterCascadeMenu(string baseKey, string exePath, string targetArg, bool includeExtract)
    {
        var rootPath = $@"{baseKey}\{MenuName}";
        SetValue(rootPath, "MUIVerb", MenuLabel);
        SetValue(rootPath, "Icon", $"\"{exePath}\"");
        SetValue(rootPath, "SubCommands", "");

        RegisterMenuItem(
            $@"{rootPath}\shell\compress",
            "压缩文件",
            exePath,
            $"\"{exePath}\" compress \"{targetArg}\"");

        if (includeExtract)
        {
            RegisterMenuItem(
                $@"{rootPath}\shell\extract",
                "解压文件",
                exePath,
                $"\"{exePath}\" extract \"{targetArg}\"");
        }
    }

    private static void RegisterMenuItem(string keyPath, string label, string exePath, string command)
    {
        SetValue(keyPath, "MUIVerb", label);
        SetValue(keyPath, "Icon", $"\"{exePath}\"");
        SetValue($@"{keyPath}\command", "", command);
    }

    private static void SetValue(string keyPath, string name, string value)
    {
        if (!OperatingSystem.IsWindows()) return;
        using var key = Registry.CurrentUser.CreateSubKey(keyPath, true);
        key?.SetValue(name, value);
    }
}
