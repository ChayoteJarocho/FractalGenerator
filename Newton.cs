﻿using System;
using System.Drawing;
using System.Numerics;

namespace FractalGenerator
{
    public class Newton : Fractal
    {
        private readonly float _tol = .0001f;
        private readonly Complex _r1 = new Complex(1.0, 0.0);
        private readonly Complex _r2 = new Complex(-0.5, Math.Sin(2 * Math.PI / 3.0));
        private readonly Complex _r3 = new Complex(-0.5, -Math.Sin(2 * Math.PI / 3.0));

        public Newton() : base()
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
            Complex z = new Complex(0.0, 0.0);
            int iterations = 0;

            do
            {
                if (z.Magnitude > 0)
                {
                    z -= (z * z * z - 1.0) / (z * z * 3.0);
                }
                iterations++;

                _calculationArray[x, y].AbsZR1 = Complex.Abs(z - _r1);
                _calculationArray[x, y].AbsZR2 = Complex.Abs(z - _r2);
                _calculationArray[x, y].AbsZR3 = Complex.Abs(z - _r3);

            }
            while (
                iterations < Configuration.MaxIterations &&
                _calculationArray[x, y].AbsZR1 >= _tol &&
                _calculationArray[x, y].AbsZR2 >= _tol &&
                _calculationArray[x, y].AbsZR2 >= _tol
            );

            CollectCommonCalculations(x, y, iterations, z);
        }

        /// <summary>
        /// Returns the color for the desired pixel.
        /// For the Newton fractal, the index of the color in the palette is obtained with:
        /// ColorIndex = Iterations * ColorBitDepth / LargestIteration
        /// But depending on the value of AbsZR1 compared to Tol, we invert one of the 3 RGB colors.
        /// </summary>
        /// <param name="x">The horizontal position of the pixel.</param>
        /// <param name="y">The vertical position of the pixel.</param>
        /// <returns>A Color instance representing the RGB color of the pixel.</returns>
        protected override Color GetColorForPixel(int x, int y)
        {
            Color color;

            int colorIndex = (int)(_calculationArray[x, y].Iterations * _colorBitDepth / _largestIteration);

            if (_calculationArray[x, y].AbsZR1 < _tol)
            {
                color = Color.FromArgb(_colorBitDepth - colorIndex, 0, 0);
            }
            else if (_calculationArray[x, y].AbsZR2 <= _tol)
            {
                color = Color.FromArgb(0, _colorBitDepth - colorIndex, 0);
            }
            else if (_calculationArray[x, y].AbsZR3 <= _tol)
            {
                color = Color.FromArgb(0, 0, _colorBitDepth - colorIndex);
            }
            else
            {
                color = Color.Black;
            }

            return color;
        }
    }
}
