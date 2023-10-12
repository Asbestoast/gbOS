using System.Drawing;
using System.Text;

namespace GBConvert;
public static class Program
{
    public static void Main(string[] args)
    {
        try
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("A project directory must be specified.");
                Environment.ExitCode = (int)ExitCode.Error;
                return;
            }

            var projectDirectory = Path.GetFullPath(args[0]);

            if (args.Length < 2)
            {
                Console.Error.WriteLine("An output directory must be specified.");
                Environment.ExitCode = (int)ExitCode.Error;
                return;
            }

            var outputDirectory = Path.GetFullPath(args[1]);

            EnsureDirectoryExists(outputDirectory);
            EnsureDirectoryExists(Path.Join(outputDirectory, SourceFileDirectoryName));

            var context = new ConversionContext();
            var assetPaths = AssetUtility.GetAssetFilesEnumerator(projectDirectory);

            foreach (var assetPath in assetPaths)
            {
                Console.WriteLine($"Converting '{assetPath}'");
                var asset = AssetUtility.LoadAsset(assetPath);
                if (asset.AssetType == AssetType.Tileset)
                {
                    ConvertTileset(outputDirectory, asset, context);
                }
                else if (asset.AssetType == AssetType.Font)
                {
                    ConvertFont(outputDirectory, asset, context);
                }
                else
                {
                    throw new IOException($"Unknown asset type '{asset.AssetType}'");
                }
            }

            Console.WriteLine("Finalizing");
            CreateAssetsSourceFile(outputDirectory, context);

            Console.WriteLine("Done");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            Environment.ExitCode = (int)ExitCode.Error;
            return;
        }

        Environment.ExitCode = (int)ExitCode.Success;
    }

    private static void EnsureDirectoryExists(string directory)
    {
        if (Directory.Exists(directory)) return;
        Directory.CreateDirectory(directory);
    }

    private static void CheckImageFormat(IndexedImage image, PixelFormat targetPixelFormat, Size tileSize)
    {
        bool supportsAlpha;
        int colorIndices;

        if (targetPixelFormat == PixelFormat.Ci2)
        {
            colorIndices = 4;
            supportsAlpha = false;
        }
        else if (targetPixelFormat == PixelFormat.Ci2Alpha1)
        {
            colorIndices = 4;
            supportsAlpha = true;
        }
        else if (targetPixelFormat == PixelFormat.Alpha)
        {
            colorIndices = 255;
            supportsAlpha = true;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(targetPixelFormat), targetPixelFormat, null);
        }

        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                var pixel = image.GetPixel(x, y);

                if (pixel == TransparentColorIndex)
                {
                    if (!supportsAlpha)
                    {
                        throw new FormatException("The color index for transparency may only be used with pixel formats containing an alpha component.");
                    }
                }
                else if (pixel >= colorIndices)
                {
                    throw new FormatException($"Color indices must be in the range 0-{colorIndices - 1}.");
                }
            }
        }

        if (image.Width % tileSize.Width != 0)
        {
            throw new FormatException($"Image width must be a multiple of {tileSize.Width}.");
        }

        if (image.Height % tileSize.Height != 0)
        {
            throw new FormatException($"Image height must be a multiple of {tileSize.Height}.");
        }
    }

    private static void ConvertTileset(string outputDirectory, Asset asset, ConversionContext context)
    {
        var image = IndexedImage.FromFile(asset.GetSourceFilePath());
        CheckImageFormat(image, asset.PixelFormat, GameBoyConstants.TileSize);
        var tileImages = image.BreakIntoTiles(GameBoyConstants.TileSize).ToList();

        var name = asset.Name;

        var sourceFilePath = Path.Join(outputDirectory, SourceFileDirectoryName, $"{name}.z80");
        using var file = File.Open(sourceFilePath, FileMode.Create);

        WriteTilesetSourceFile(name, asset.Section, asset.Bank, tileImages, asset.PixelFormat, file);

        context.SourceFiles.Add(sourceFilePath);
    }

    private class GlyphInfo
    {
        public int Width { get; set; }
        public IndexedImage Image { get; set; }

        public GlyphInfo(int width, IndexedImage image)
        {
            Width = width;
            Image = image;
        }
    }

    private static void ConvertFont(string outputDirectory, Asset asset, ConversionContext context)
    {
        var pixelFormat = PixelFormat.Alpha;

        var image = IndexedImage.FromFile(asset.GetSourceFilePath());
        CheckImageFormat(image, pixelFormat, GameBoyConstants.TileSize);
        var tileImages = image.BreakIntoTiles(GameBoyConstants.TileSize).ToList();

        var name = asset.Name;

        var sourceFilePath = Path.Join(outputDirectory, SourceFileDirectoryName, $"{name}.z80");
        using var file = File.Open(sourceFilePath, FileMode.Create);

        var glyphs = new List<GlyphInfo>();
        foreach (var tile in tileImages.SelectMany(i => i.BreakIntoTiles(GameBoyConstants.TileSize)))
        {
            var width = 0;
            for (var x = 0; x < tile.Width; x++)
            {
                if (tile.GetPixel(x, 0) == TransparentColorIndex) break;
                width++;
            }

            var glyph = new GlyphInfo(width, tile);
            glyphs.Add(glyph);
        }

        WriteFontSourceFile(name, asset.Section, asset.Bank, asset, glyphs, pixelFormat, file);

        context.SourceFiles.Add(sourceFilePath);
    }

    private static void WriteFontSourceFile(
        string tilesetName, string section, string bank, Asset asset, List<GlyphInfo> glyphs, PixelFormat pixelFormat, Stream stream)
    {
        using var writer = new StreamWriter(stream, CreateEncoding(), -1, true);

        var tilesetLabel = tilesetName;
        var graphicsCharacters = TileGraphicsCharacters;

        WriteCodeFileHeader(writer);

        writer.WriteLine("pusho");
        writer.WriteLine($"opt g{graphicsCharacters}");
        writer.WriteLine();

        writer.WriteLine($"section fragment \"{GetSectionName(section, $"Tileset - {tilesetLabel}")}\", romx{GetBankDirectiveString(bank)}, align[{GameBoyConstants.TilePatternAlignment}]");
        writer.WriteLine($"{tilesetName}::");
        writer.WriteLine($"{Indent}db {FormatHex(checked((byte)asset.FontSettings.LineHeight))}");
        foreach (var glyph in glyphs)
        {
            WriteTile(glyph.Image, pixelFormat, writer, graphicsCharacters, Indent);
            writer.WriteLine($"{Indent}db {FormatHex(checked((byte)glyph.Width))}");
        }
        writer.WriteLine($"{Indent}.end::");

        writer.WriteLine();
        writer.WriteLine("popo");
    }

    private static string GetBankDirectiveString(string bank)
    {
        if (string.IsNullOrEmpty(bank)) return string.Empty;
        return $", bank[{bank}]";
    }

    private static string GetSectionName(string sectionName, string defaultSectionName)
    {
        return string.IsNullOrEmpty(sectionName) ? defaultSectionName : sectionName;
    }

    private const byte TransparentColorIndex = 0xFF;

    private static void WriteTile(
        IndexedImage image,
        PixelFormat pixelFormat,
        TextWriter writer,
        string graphicsCharacters = "0123",
        string indent = "")
    {
        if (graphicsCharacters.Length != GameBoyConstants.MaxColors2Bpp)
        {
            throw new ArgumentException("Invalid format.", nameof(graphicsCharacters));
        }

        if (image.Width != GameBoyConstants.TileWidth)
        {
            throw new ArgumentException("Unsupported image size.", nameof(image));
        }

        for (var y = 0; y < image.Height; y++)
        {
            if (pixelFormat == PixelFormat.Ci2)
            {
                writer.Write(indent + "dw `");
                for (var x = 0; x < image.Width; x++)
                {
                    var pixel = image.GetPixel(x, y);
                    writer.Write(graphicsCharacters[pixel]);
                }
                writer.WriteLine();
            }
            else if (pixelFormat == PixelFormat.Ci2Alpha1)
            {
                writer.Write(indent + "dw `");
                for (var x = 0; x < image.Width; x++)
                {
                    var pixel = image.GetPixel(x, y);
                    if (pixel == TransparentColorIndex)
                    {
                        pixel = 0;
                    }
                    writer.Write(graphicsCharacters[pixel]);
                }
                writer.WriteLine();

                var alpha = (byte)0;
                for (var x = 0; x < image.Width; x++)
                {
                    var pixel = image.GetPixel(x, y);
                    alpha <<= 1;
                    alpha |= (byte)(pixel == TransparentColorIndex ? 1 : 0);
                }
                writer.WriteLine($"{indent}db {FormatHex(alpha)}");
            }
            else if (pixelFormat == PixelFormat.Alpha)
            {
                var alpha = (byte)0;
                for (var x = 0; x < image.Width; x++)
                {
                    var pixel = image.GetPixel(x, y);
                    alpha <<= 1;
                    alpha |= (byte)(pixel == 0 || pixel == TransparentColorIndex ? 1 : 0);
                }
                writer.WriteLine($"{indent}db {FormatHex(alpha)}");
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(pixelFormat), pixelFormat, null);
            }
        }
    }

    private static void WriteTilesetSourceFile(
        string tilesetName, string section, string bank, IEnumerable<IndexedImage> tiles, PixelFormat pixelFormat, Stream stream)
    {
        using var writer = new StreamWriter(stream, CreateEncoding(), -1, true);
        
        var tilesetLabel = tilesetName;
        var graphicsCharacters = TileGraphicsCharacters;

        WriteCodeFileHeader(writer);

        writer.WriteLine("pusho");
        writer.WriteLine($"opt g{graphicsCharacters}");
        writer.WriteLine();

        writer.WriteLine($"section fragment \"{GetSectionName(section, $"Tileset - {tilesetLabel}")}\", romx{GetBankDirectiveString(bank)}, align[{GameBoyConstants.TilePatternAlignment}]");
        writer.WriteLine($"{tilesetName}::");
        foreach (var tile in tiles.SelectMany(i => i.BreakIntoTiles(GameBoyConstants.TileSize)))
        {
            WriteTile(tile, pixelFormat, writer, graphicsCharacters, Indent);
        }
        writer.WriteLine($"{Indent}.end::");

        writer.WriteLine();
        writer.WriteLine("popo");
    }

    private static void WriteCodeFileHeader(TextWriter writer)
    {
        writer.WriteLine(";");
        writer.WriteLine("; Auto-generated file. Do not edit.");
        writer.WriteLine(";");
        writer.WriteLine();
    }

    private static void CreateAssetsSourceFile(
        string outputDirectory, ConversionContext context)
    {
        var dstSourceFileDirectory = Path.Join(outputDirectory, SourceFileDirectoryName);
        var dstSourceFilePath = Path.Join(dstSourceFileDirectory, "assets.z80");

        using var file = File.Open(dstSourceFilePath, FileMode.Create);
        using var writer = new StreamWriter(file, CreateEncoding());

        WriteCodeFileHeader(writer);

        foreach (var sourceFilePath in context.SourceFiles)
        {
            var relativeSourceFilePath = Path.GetRelativePath(dstSourceFileDirectory, sourceFilePath);
            writer.WriteLine($"include \"{NormalizePath(relativeSourceFilePath)}\"");
        }
    }

    private static Encoding CreateEncoding()
    {
        // A custom UTF8Encoding is created to prevent the creation of a BOM (Byte-Order Mark).
        return new UTF8Encoding(false);
    }

    private static string NormalizePath(string path)
    {
        path = path.Replace('\\', '/');
        return path;
    }

    private static string FormatHex(sbyte value) => FormatHex((byte)value);
    private static string FormatHex(byte value) => $"${value:X2}";
    private static string FormatHex(short value) => FormatHex((ushort)value);
    private static string FormatHex(ushort value) => $"${value:X4}";

    private const string Indent = "    ";
    private const string TileGraphicsCharacters = "2013";
    private const string SourceFileDirectoryName = "src";
}