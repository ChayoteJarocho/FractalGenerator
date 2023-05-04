using System;
using System.Drawing;

namespace FractalGenerator;

public class Spiderweb : Fractal
{
    public Spiderweb() : base()
    {
    }

    /// <summary>
    /// Calculates the fractal value for each pixel using:
    /// </summary>
    /// <param name="x">The horizontal pixel.</param>
    /// <param name="y">The vertical pixel.</param>
    /// <param name="h">The horizontal point in the complex plane.</param>
    /// <param name="v">The vertical point in the complex plane.</param>
    protected override void Calculate(int x, int y, double h, double v)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the color for the desired pixel.
    /// For the Spiderweb fractal, the index of the color in the palette is obtained with:
    /// </summary>
    /// <param name="x">The horizontal position of the pixel.</param>
    /// <param name="y">The vertical position of the pixel.</param>
    /// <returns>A Color instance representing the RGB color of the pixel.</returns>
    protected override Color GetColorForPixel(int x, int y)
    {
        throw new NotImplementedException();
    }
}
