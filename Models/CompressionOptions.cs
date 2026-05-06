namespace SevenZipExt.Models;

public class CompressionOptions
{
    public List<string> SourceItems { get; set; } = new();
    public string ArchivePath { get; set; } = "";
    public string Format { get; set; } = "7z";
    public int Level { get; set; } = 5;
    public string Method { get; set; } = "";
    public string DictionarySize { get; set; } = "";
    public string WordSize { get; set; } = "";
    public string SolidBlockSize { get; set; } = "";
    public string Threads { get; set; } = "";
    public string Password { get; set; } = "";
    public bool EncryptFileNames { get; set; }
    public string EncryptionMethod { get; set; } = "AES-256";
    public List<ExclusionItem> Exclusions { get; set; } = new();
    public string UpdateMethod { get; set; } = "Add and replace files";
    public string PathMode { get; set; } = "Relative pathnames";
    public bool CreateSfx { get; set; }
    public bool CompressSharedFiles { get; set; }
    public bool DeleteAfterCompress { get; set; }
    public string VolumeSize { get; set; } = "";
    public string AdditionalParameters { get; set; } = "";
}

public class ExtractionOptions
{
    public string ArchivePath { get; set; } = "";
    public string DestinationPath { get; set; } = "";
    public bool CreateSubfolder { get; set; } = true;
    public string Password { get; set; } = "";
    public bool OverwriteExisting { get; set; } = true;
    public bool FullPaths { get; set; } = true;
}
