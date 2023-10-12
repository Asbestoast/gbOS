using System.Text.Json;
using System.Text.Json.Serialization;

namespace GBConvert;
internal static class AssetUtility
{
    public const string AssetFileExtension = ".meta";

    public static IEnumerable<string> GetAssetFilesEnumerator(string directory)
    {
        foreach (var file in Directory.EnumerateFiles(directory))
        {
            var fileExtension = Path.GetExtension(file);
            if (fileExtension != AssetFileExtension) continue;
            yield return file;
        }

        foreach (var subdirectory in Directory.EnumerateDirectories(directory))
        {
            var subdirectoryName = Path.GetDirectoryName(subdirectory);
            if (subdirectoryName == null) continue;
            if (subdirectoryName.StartsWith('.')) continue;

            foreach (var file in GetAssetFilesEnumerator(subdirectory))
            {
                yield return file;
            }
        }
    }

    public static Asset LoadAsset(string path)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            IncludeFields = true,
        };
        using var file = File.OpenRead(path);
        var asset = JsonSerializer.Deserialize<Asset>(file, options);
        if (asset == null) throw new IOException();
        asset.MetaFilePath = path;

        return asset;
    }
}
