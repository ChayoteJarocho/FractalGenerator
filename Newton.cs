using System;
using System.Drawing;
using System.Numerics;

namespace FractalGenerator;

// Inspiration: https://www.mitchr.me/SS/newton/
public class Newton : Fractal
{
    private const float TOL = .0001f;

    private readonly Complex _r1;
    private readonly Complex _r2;
    private readonly Complex _r3;

    public Newton() : base()
    {
        _r1 = new Complex(1, 0);
        _r2 = new Complex(-0.5, Math.Sin(2.0 * Math.PI / 3.0));
        _r3 = new Complex(-0.5, -Math.Sin(2.0 * Math.PI / 3.0));
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
        var z = new Complex(h, v);
        ulong iterations = 0;
        do
        {
            if (Complex.Abs(z) > 0)
            {
                z -= (Complex.Pow(z, 3) - 1.0) / (Complex.Pow(z, 2) * 3.0);
            }
            iterations++;
        }
        while (iterations < Configuration.MaxIterations &&
                Complex.Abs(z - _r1) >= TOL &&
                Complex.Abs(z - _r2) >= TOL &&
                Complex.Abs(z - _r3) >= TOL);

        _calculationArray[x, y].AbsZR1 = Complex.Abs(z - _r1);
        _calculationArray[x, y].AbsZR2 = Complex.Abs(z - _r2);
        _calculationArray[x, y].AbsZR3 = Complex.Abs(z - _r3);

        CollectCommonCalculations(x, y, iterations, z);
    }

    /// <summary>
    /// Returns the color for the desired pixel.
    /// </summary>
    /// <param name="x">The horizontal position of the pixel.</param>
    /// <param name="y">The vertical position of the pixel.</param>
    /// <returns>A Color instance representing the RGB color of the pixel.</returns>
    protected override Color GetColorForPixel(int x, int y)
    {
        int r = 0;
        int g = 0;
        int b = 0;

        if (_calculationArray[x, y].AbsZR1 < TOL)
        {
            r = 255 - (int)_calculationArray[x, y].Iterations * 15;
        }
        if (_calculationArray[x, y].AbsZR2 <= TOL)
        {
            g = 255 - (int)_calculationArray[x, y].Iterations * 15;
        }
        if (_calculationArray[x, y].AbsZR3 <= TOL)
        {
            b = 255 - (int)_calculationArray[x, y].Iterations * 15;
        }

        return Color.FromArgb(
            r >= 0 ? r : 0,
            g >= 0 ? g : 0,
            b >= 0 ? b : 0);
    }
}
