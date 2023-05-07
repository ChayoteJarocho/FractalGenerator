using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;

namespace FractalGenerator;

public abstract class Fractal
{
    protected const int ColorBitDepth = 255;
    private readonly Image<Rgb24> _image;
    protected Calculation[,] _calculationArray;

    public Fractal()
    {
        Log.Info($"Generating a '{Configuration.FractalVariation}' fractal...");

        _image = new Image<Rgb24>(Configuration.Width, Configuration.Height);
        _calculationArray = new Calculation[Configuration.Width, Configuration.Height];
    }

    public void Generate()
    {
        Calculate();
        Save();
    }

    public void Open()
    {
        if (!File.Exists(Configuration.OutputFilePath))
        {
            throw new FileNotFoundException($"File does not exist: {Configuration.OutputFilePath}");
        }

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

    private void Save()
    {
        Log.Info("Saving file...");
        _image.Save(Configuration.OutputFileName);
        _image.Dispose();
    }

    private void Calculate()
    {
        Log.Info("Calculating fractal: ");

        _image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                double v = ConvertPixelToVertical(y);
                Span<Rgb24> pixelRow = accessor.GetRowSpan(y);
                CalculateRow(y, v, pixelRow);
            }
        });
        Log.Line();
        Log.Success("Calculation finished!");
    }

    private void CalculateRow(int y, double v, Span<Rgb24> pixelRow)
    {
        for (int x = 0; x < pixelRow.Length; x++)
        {
            double h = ConvertPixelToHorizontal(x);
            Calculate(x, y, h, v);
            pixelRow[x] = GetColorForPixel(x, y).ToPixel<Rgb24>();
            
        }

        // Indicate the column was finished
        Console.Write(".");
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
