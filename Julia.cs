using System;
using System.Drawing;
using System.Numerics;

namespace FractalGenerator
{
    public class Julia : Fractal
    {
        public Julia() : base()
        {
        }

        /// <summary>
        /// Calculates the fractal value for each pixel using:
        /// Zn = Z(n-1)^2 + C
        /// As long as Abs(Z) &lt; MaxRadius and Iterations &lt; MaxIterations
        /// </summary>
        /// <param name="x">The horizontal pixel.</param>
        /// <param name="y">The vertical pixel.</param>
        /// <param name="h">The horizontal point in the complex plane.</param>
        /// <param name="v">The vertical point in the complex plane.</param>
        protected override void Calculate(int x, int y, double h, double v)
        {
            Complex z = new Complex(h, v);
            Complex c = new Complex(Configuration.CReal, Configuration.CImaginary);

            int iterations = 0;
            do
            {
                z = z * z + c;
                iterations++;
            }
            while (z.Magnitude < Configuration.Radius && iterations < Configuration.MaxIterations);

            CollectCommonCalculations(x, y, iterations, z);
        }

        /// <summary>
        /// Returns the color for the desired pixel.
        /// For the Julia fractal, the index of the color in the palette is obtained with:
        /// ColorIndex = Iterations * ColorBitDepth / LargestIteration
        /// </summary>
        /// <param name="x">The horizontal position of the pixel.</param>
        /// <param name="y">The vertical position of the pixel.</param>
        /// <returns>A Color instance representing the RGB color of the pixel.</returns>
        protected override Color GetColorForPixel(int x, int y)
        {
            int colorIndex;

            if (double.IsInfinity(_calculationArray[x, y].Iterations))
            {
                colorIndex = 0;
            }
            else
            {
                colorIndex = (int)(_calculationArray[x, y].Iterations * ColorBitDepth / Configuration.MaxIterations);
            }

            return Configuration.ArrayPalette[colorIndex];
        }
    }
}
