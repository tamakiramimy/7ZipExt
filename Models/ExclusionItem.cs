namespace SevenZipExt.Models;

public class ExclusionItem
{
    public string Path { get; set; } = "";
    public bool IsFolder { get; set; }
    public bool IsRecursive { get; set; } = true;

    public string DisplayIcon => IsFolder ? "📁" : "📄";
    public string Pattern => IsFolder
        ? System.IO.Path.GetFileName(Path.TrimEnd('\\', '/'))
        : System.IO.Path.GetFileName(Path);
    public string Flag => IsRecursive ? "-xr!" : "-x!";
    public string FullFlag => $"{Flag}{Pattern}";

    public override string ToString() => Path;
}
