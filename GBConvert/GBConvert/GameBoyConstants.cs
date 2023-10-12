using System.Drawing;

namespace GBConvert;
static class GameBoyConstants
{
    public const int MaxColors2Bpp = 1 << 2;
    public const int TileWidth = 8;
    public const int TileHeight = 8;
    public static Size TileSize { get; } = new(TileWidth, TileHeight);
    public const int TilePatternAlignment = 4;
}