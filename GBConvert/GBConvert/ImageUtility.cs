using System.Drawing;

namespace GBConvert;
public static class ImageUtility
{
    public static IEnumerable<IndexedImage> BreakIntoTiles(this IndexedImage image, Size tileSize)
    {
        if (image.Width % tileSize.Width != 0)
        {
            throw new ArgumentException($"Image height must be a multiple of {tileSize.Width}.", nameof(image));
        }

        if (image.Height % tileSize.Height != 0)
        {
            throw new ArgumentException($"Image height must be a multiple of {tileSize.Height}.", nameof(image));
        }

        var widthTiles = image.Width / tileSize.Width;
        var heightTiles = image.Height / tileSize.Height;

        for (var yT = 0; yT < heightTiles; yT++)
        {
            for (var xT = 0; xT < widthTiles; xT++)
            {
                var x = xT * tileSize.Width;
                var y = yT * tileSize.Height;
                var subimage = image.GetSubimage(
                    new Rectangle(x, y, tileSize.Width, tileSize.Height));
                yield return subimage;
            }
        }
    }
}
