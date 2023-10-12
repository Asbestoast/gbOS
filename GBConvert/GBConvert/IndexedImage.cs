using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GBConvert;
public sealed class IndexedImage
{
    public byte[] Pixels { get; }
    public int Offset { get; }
    public int Stride { get; }

    public int Width { get; }
    public int Height { get; }

    public Rectangle Bounds => new(0, 0, Width, Height);

    public byte GetPixel(int x, int y)
    {
        CheckContainsPoint(x, y);
        return Pixels[Offset + x + y * Stride];
    }

    public void SetPixel(int x, int y, byte value)
    {
        CheckContainsPoint(x, y);
        Pixels[Offset + x + y * Stride] = value;
    }

    /// <summary>
    /// Initializes an <see cref="IndexedImage"/> from an <see cref="IntPtr"/>.
    /// The resulting <see cref="IndexedImage"/> will contain a copy of the data pointed to by the <see cref="IntPtr"/>.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="scan0"></param>
    /// <param name="stride"></param>
    public IndexedImage(int width, int height, IntPtr scan0, int stride) : this(width, height, stride)
    {
        if (scan0 != IntPtr.Zero)
        {
            Marshal.Copy(scan0, Pixels, 0, Pixels.Length);
        }
    }

    public IndexedImage(int width, int height, int stride)
    {
        if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height < 0) throw new ArgumentOutOfRangeException(nameof(height));
        if (stride < 0) throw new ArgumentOutOfRangeException(nameof(stride));
        Width = width;
        Height = height;
        Stride = stride;
        Pixels = new byte[height * stride];
    }

    public IndexedImage(int width, int height, byte[] pixels, int offset, int stride)
    {
        if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height < 0) throw new ArgumentOutOfRangeException(nameof(height));
        if (stride < 0) throw new ArgumentOutOfRangeException(nameof(stride));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
        Width = width;
        Height = height;
        Pixels = pixels;
        Offset = offset;
        Stride = stride;
    }

    public IndexedImage(int width, int height) : this(width, height, width)
    {
    }

    /// <summary>
    /// Gets a subimage that shares the same underlying pixel buffer.
    /// </summary>
    /// <param name="subimageBounds"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public IndexedImage GetSubimage(Rectangle subimageBounds)
    {
        if (!Bounds.Contains(subimageBounds))
            throw new ArgumentException("Subimage must be contained within the source image.");
        return new IndexedImage(
            subimageBounds.Width,
            subimageBounds.Height,
            Pixels,
            Offset + subimageBounds.X + subimageBounds.Y * Stride,
            Stride);
    }

    private void CheckContainsPoint(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            throw new ArgumentException("The image does not contain the specified point.");
        }
    }

    public static IndexedImage FromFile(string path)
    {
        if (!OperatingSystem.IsWindows())
            throw new NotSupportedException("Unsupported operating system.");

        using var image = new Bitmap(path);
        var bitmapData = image.LockBits(Rectangle.FromLTRB(0, 0, image.Width, image.Height),
            ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
        var indexedImage = new IndexedImage(bitmapData.Width, bitmapData.Height, bitmapData.Scan0,
            bitmapData.Stride);
        image.UnlockBits(bitmapData);

        return indexedImage;
    }
}