using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace FractalGenerator
{
    public abstract class Fractal
    {
        protected readonly int _colorBitDepth = 255;

        private bool _isCalculated;
        private FastBitmap _bitmap;
        private object _lockCalculation;
        protected Calculation[,] _calculationArray;
        protected long _largestIteration;
        protected double _largestIterationLog;

        public Fractal()
        {
            Log.Info($"Generating a '{Configuration.FractalVariation.ToString()}' fractal...");

            _isCalculated = false;
            _bitmap = new FastBitmap();
            _calculationArray = new Calculation[Configuration.Width, Configuration.Height];
            _lockCalculation = new object();

            _largestIteration = 1;
        }

        public void Generate()
        {
            Calculate();
            Paint();
        }

        public void Open()
        {
            if (File.Exists(Configuration.OutputFilePath))
            {
                Log.Info($"Opening: {Configuration.OutputFilePath}");

                Process p = new Process();
                p.StartInfo = new ProcessStartInfo(Configuration.OutputFilePath)
                {
                    UseShellExecute = true
                };
                p.Start();
            }
        }

        private void Calculate()
        {
            Log.Info("Calculating fractal: ");

            if (Configuration.Async)
            {
                Parallel.For(0, Configuration.Width, CalculateCallback);
            }
            else
            {
                for (int x = 0; x < Configuration.Width; x++)
                {
                    CalculateCallback(x);
                }
            }

            Log.Line();
            Log.Success("Calculation finished!");

            _isCalculated = true;
        }

        private void Paint()
        {
            if (!_isCalculated)
            {
                throw new InvalidOperationException("Must call Calculate before calling Paint!");
            }

            Log.Info(false, "Painting fractal: ");

            _bitmap.Lock();
            
            if (Configuration.Async)
            {
                Parallel.For(0, Configuration.Width, PaintCallback);
            }
            else
            {
                for (int x = 0; x < Configuration.Width; x++)
                {
                    PaintCallback(x);
                }
            }
            
            _bitmap.Unlock();
            _bitmap.Save();

            Log.Line();
            Log.Success("Painting finished!");
        }

        private void CalculateCallback(int x)
        {
            double h = ConvertPixelToHorizontal(x);

            if (Configuration.Async)
            {
                Parallel.For(0, Configuration.Height, y =>
                {
                    double v = ConvertPixelToVertical(y);
                    Calculate(x, y, h, v);
                });
            }
            else
            {
                for (int y = 0; y < Configuration.Height; y++)
                {
                    double v = ConvertPixelToVertical(y);
                    Calculate(x, y, h, v);
                }
            }

            // Indicate the column was finished
            Console.Write(".");
        }

        private void PaintCallback(int x)
        {
            if (Configuration.Async)
            {
                Parallel.For(0, Configuration.Height, y =>
                {
                    Color color = GetColorForPixel(x, y);
                    _bitmap.SetPixel(x, y, color);
                });
            }
            else
            {
                for (int y = 0; y < Configuration.Height; y++)
                {
                    Color color = GetColorForPixel(x, y);
                    _bitmap.SetPixel(x, y, color);
                }
            }

            // One dot = one line
            Console.Write(".");
        }

        protected void CollectCommonCalculations(int x, int y, int iterations, Complex z)
        {
            _calculationArray[x, y].LastZ = z;
            _calculationArray[x, y].Iterations = iterations;

            lock (_lockCalculation)
            {
                if (iterations > _largestIteration)
                {
                    _largestIteration = iterations;
                }
            }
        }

        protected double ConvertPixelToHorizontal(int x)
        {
            return (x * Configuration.PlaneWidth / (Configuration.Width)) + Configuration.XMin;
        }

        protected double ConvertPixelToVertical(int y)
        {
            return (y * Configuration.PlaneHeight / (Configuration.Height)) + Configuration.YMin;
        }

        protected int ConvertHorizontalToPixel(double h)
        {
            return (int)((h - Configuration.XMin) * Configuration.Width / Configuration.PlaneWidth);
        }

        protected int ConvertVerticalToPixel(double v)
        {
            return (int)((v - Configuration.YMin) * Configuration.Height / Configuration.PlaneHeight);
        }

        protected abstract void Calculate(int x, int y, double h, double v);

        protected abstract Color GetColorForPixel(int x, int y);
    }
}