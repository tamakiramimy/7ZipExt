using System.Text.Json.Serialization;

namespace SevenZipExt.Models;

public class AppSettings
{
    public string SevenZipPath { get; set; } = @"C:\Program Files\7-Zip\7z.exe";
    public string DefaultFormat { get; set; } = "7z";
    public int DefaultCompressionLevel { get; set; } = 5;
    public string DefaultEncryptionMethod { get; set; } = "AES-256";
    public string DefaultUpdateMethod { get; set; } = "Add and replace files";
    public string DefaultPathMode { get; set; } = "Relative pathnames";
    public int DefaultThreads { get; set; } = 0;
}
