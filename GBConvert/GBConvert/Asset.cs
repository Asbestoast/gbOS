namespace GBConvert;
internal class Asset
{
    /// <summary>
    /// A path to the .meta file from which this Asset was loaded.
    /// </summary>
    public string MetaFilePath { get; internal set; } = string.Empty;
    public AssetType AssetType { get; set; }
    public string Bank { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public PixelFormat PixelFormat { get; set; } = PixelFormat.Ci2;
    public FontSettings FontSettings { get; set; } = new();

    public string GetSourceFilePath()
    {
        var name = Name;
        var directory = Path.GetDirectoryName(MetaFilePath);
        if (directory == null) throw new IOException("Invalid path.");
        foreach (var file in Directory.EnumerateFiles(directory))
        {
            var name2 = Path.GetFileNameWithoutExtension(file);
            if (name2 == name)
            {
                return file;
            }
        }
        throw new IOException($"Missing source file for asset '{MetaFilePath}'.");
    }

    public string Name => Path.GetFileNameWithoutExtension(MetaFilePath);
}
