using SixLabors.ImageSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;

namespace FractalGenerator;

public abstract class Fractal
{
    protected const int ColorBitDepth = 255;

    private bool _isCalculated;
    private readonly FastBitmap _bitmap;
    protected Calculation[,] _calculationArray;
    protected ulong _largestIteration;
    protected double _largestIterationLog;

    public Fractal()
    {
        Log.Info($"Generating a '{Configuration.FractalVariation}' fractal...");

        _isCalculated = false;
        _bitmap = new FastBitmap();
        _calculationArray = new Calculation[Configuration.Width, Configuration.Height];
        _largestIteration = 1;
    }

    public void Generate()
    {
        Calculate();
        Paint();
    }

    public static void Open()
    {
        if (File.Exists(Configuration.OutputFilePath))
        {
            Log.Info($"Opening: {Configuration.OutputFilePath}");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo(Configuration.OutputFilePath)
                {
                    UseShellExecute = true
                }
            };
            process.Start();
        }
    }

    private void Calculate()
    {
        Log.Info("Calculating fractal: ");

        for (int x = 0; x < Configuration.Width; x++)
        {
            CalculateCallback(x);
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

        Log.Info("Painting fractal: ");

        _bitmap.Lock();

        for (int x = 0; x < Configuration.Width; x++)
        {
            PaintCallback(x);
        }

        _bitmap.Unlock();
        _bitmap.Save();

        Log.Line();
        Log.Success("Painting finished!");
    }

    private void CalculateCallback(int x)
    {
        double h = ConvertPixelToHorizontal(x);

        for (int y = 0; y < Configuration.Height; y++)
        {
            double v = ConvertPixelToVertical(y);
            Calculate(x, y, h, v);
        }

        FindLargestIteration();

        // Indicate the column was finished
        Console.Write(".");
    }

    private void PaintCallback(int x)
    {
        for (int y = 0; y < Configuration.Height; y++)
        {
            Color color = GetColorForPixel(x, y);
            _bitmap.SetPixel(x, y, color);
        }
        
        // One dot = one line
        Console.Write(".");
    }

    private void FindLargestIteration()
    {
        for (int y = 0; y < Configuration.Height; y++)
        {
            for (int x = 0; x < Configuration.Width; x++)
            {
                if (_calculationArray[x,y].Iterations > _largestIteration)
                {
                    _largestIteration = _calculationArray[x, y].Iterations;
                }
            }
        }
    }

    protected void CollectCommonCalculations(int x, int y, ulong iterations, Complex z)
    {
        _calculationArray[x, y].LastZ = z;
        _calculationArray[x, y].Iterations = iterations;
    }

    protected static double ConvertPixelToHorizontal(int x) =>
        (x * Configuration.PlaneWidth / (Configuration.Width)) + Configuration.XMin;

    protected static double ConvertPixelToVertical(int y) =>
        (y * Configuration.PlaneHeight / (Configuration.Height)) + Configuration.YMin;

    protected static int ConvertHorizontalToPixel(double h) =>
        (int)((h - Configuration.XMin) * Configuration.Width / Configuration.PlaneWidth);

    protected static int ConvertVerticalToPixel(double v) =>
        (int)((v - Configuration.YMin) * Configuration.Height / Configuration.PlaneHeight);

    protected abstract void Calculate(int x, int y, double h, double v);

    protected abstract Color GetColorForPixel(int x, int y);
}
