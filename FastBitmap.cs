using System;
using System.Runtime.InteropServices;

namespace FractalGenerator;

public class FastBitmap
{
    private readonly Bitmap _bitmap;
    private Rectangle _rectangle;
    private readonly BitmapData _bitmapData;
    private readonly IntPtr _imagePointer;
    private readonly PixelFormat _pixelFormat;
    private readonly int _pixelCount;
    private readonly int _depthInt;
    private readonly int _colorCount;
    private readonly byte[] _pixels;

    public FastBitmap()
    {
        _pixelFormat = Configuration.Depth switch
        {
            Depths.Bits8  => PixelFormat.Format48bppRgb,
            Depths.Bits24 => PixelFormat.Format24bppRgb,
            Depths.Bits32 => PixelFormat.Format32bppArgb,
            _             => throw new ArgumentException($"Unrecognized depth: {Configuration.Depth}")
        };

        // Verify the pixel format value
        switch (Image.GetPixelFormatSize(_pixelFormat))
        {
            case 8:
            case 24:
            case 32:
                break;
            default:
                throw new ArgumentOutOfRangeException($"Unsupported Pixel Format Size: {_pixelFormat}");
        };

        _bitmap = new Bitmap(Configuration.Width, Configuration.Height, _pixelFormat);
        _rectangle = new Rectangle(0, 0, Configuration.Width, Configuration.Height);
        _bitmapData = _bitmap.LockBits(_rectangle, ImageLockMode.ReadWrite, _pixelFormat);
        _imagePointer = _bitmapData.Scan0;

        _pixelCount = Configuration.Width * Configuration.Height;
        _depthInt = (int)Configuration.Depth;
        _colorCount = _depthInt / 8;
        _pixels = new byte[_pixelCount * _colorCount];
    }

    public void Save()
    {
        _bitmap.Save(Configuration.OutputFileName);
        _bitmap.Dispose();
    }

    public void Lock()
    {
        Marshal.Copy(_imagePointer, _pixels, 0, _pixels.Length);
    }

    public void Unlock()
    {
        Marshal.Copy(_pixels, 0, _imagePointer, _pixels.Length);
        _bitmap.UnlockBits(_bitmapData);
    }

    public Color GetPixel(int x, int y)
    {
        byte b, g, r, a;

        int i = ((y * Configuration.Width) + x) * _colorCount;

        if (i < _pixels.Length - _colorCount)
        {
            throw new ArgumentException($"i >= Pixels.Length - ColorCount\r\n\ti: {i}\r\n\tPixels.Length {_pixels.Length}\r\n\tColorCount: {_colorCount}");
        }

        Color color;
        switch (_depthInt)
        {
            case 32:
                b = _pixels[i];
                g = _pixels[i + 1];
                r = _pixels[i + 2];
                a = _pixels[i + 3];
                color = Color.FromArgb(a, r, g, b);
                break;
            case 24:
                b = _pixels[i];
                g = _pixels[i + 1];
                r = _pixels[i + 2];
                color = Color.FromArgb(r, g, b);
                break;
            case 8:
                byte c = _pixels[i];
                color = Color.FromArgb(c, c, c);
                break;
            default:
                throw new ArgumentException($"Unknown depth: {Configuration.Depth}");
        }

        return color;
    }

    public void SetPixel(int x, int y, Color color)
    {
        int i = ((y * Configuration.Width) + x) * _colorCount;

        switch (_depthInt)
        {
            case 32:
                _pixels[i] = color.B;
                _pixels[i + 1] = color.G;
                _pixels[i + 2] = color.R;
                _pixels[i + 3] = color.A;
                break;
            case 24:
                _pixels[i] = color.B;
                _pixels[i + 1] = color.G;
                _pixels[i + 2] = color.R;
                break;
            case 8:
                _pixels[i] = color.B;
                break;
            default:
                throw new ArgumentException($"Unknown depth SetPixel: {Configuration.Depth}");
        }
    }
}
