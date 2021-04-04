using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace FractalGenerator
{
    public static class Configuration
    {
        private enum ArgumentOption
        {
            Async,
            Depth,
            CImaginary,
            CReal,
            Fractal,
            Height,
            Initial,
            MaxIterations,
            Light,
            MixPaletteFile,
            OutputFile,
            PaletteFile,
            Radius,
            Width,
            XCenter,
            YCenter,
            Zoom
        }

        private const int DefaultPaletteArrayLength = 256;
        private const string FolderNamePalettes = "palettes";

        private static double XWidth => 8.0 / Zoom;
        private static double XMaxAtOrigin => XWidth / 2.0;
        private static double XMinAtOrigin => 0.0 - XMaxAtOrigin;
        private static double YHeight => (Height * XWidth) / Width;
        private static double YMaxAtOrigin => YHeight / 2.0;
        private static double YMinAtOrigin => 0.0 - YMaxAtOrigin;

        public static readonly Color[] ArrayPalette = new Color[DefaultPaletteArrayLength];
        public static readonly Color[] ArrayMixPalette = new Color[DefaultPaletteArrayLength];

        public static readonly double Log2 = Math.Log(2.0);
        public static readonly string AbsolutePathPalettes = Path.Combine(Directory.GetCurrentDirectory(), FolderNamePalettes);

        public static string PaletteFilePath => Path.Combine(AbsolutePathPalettes, PaletteFileName);
        public static string MixPaletteFilePath => (!string.IsNullOrWhiteSpace(MixPaletteFileName)) ?
            Path.Combine(AbsolutePathPalettes, MixPaletteFileName) :
            string.Empty;

        public static double XMin => XCenter + XMinAtOrigin;
        public static double XMax => XCenter + XMaxAtOrigin;
        public static double YMin => YCenter + YMinAtOrigin;
        public static double YMax => YCenter + YMaxAtOrigin;

        public static double PlaneHeight => Math.Abs(YMax) + Math.Abs(YMin);
        public static double PlaneWidth => Math.Abs(XMax) + Math.Abs(XMin);

        public static bool Async { get; private set; } = true;
        public static string OutputFilePath => Path.Combine(Directory.GetCurrentDirectory(), OutputFileName);
        public static double CImaginary { get; private set; } = 0.75;
        public static double CReal { get; private set; } = -0.2;
        public static Depths Depth { get; private set; } = Depths.Bits24;
        public static FractalVariations FractalVariation { get; private set; } = FractalVariations.Mandelbrot;
        public static int Height { get; private set; } = 200;
        public static int MaxIterations { get; private set; } = 256;
        public static double Light { get; private set; } = 0.0;
        public static string OutputFileName { get; private set; } = "output.bmp";
        public static string MixPaletteFileName { get; private set; } = string.Empty;
        public static string PaletteFileName { get; private set; } = "hsv.txt";
        public static double Radius { get; private set; } = 10e19;
        public static int Width { get; private set; } = 320;
        public static double XCenter { get; private set; } = -0.5;
        public static double YCenter { get; private set; } = 0.0;
        public static double Zoom { get; private set; } = 1.0;
        
        public static void Verify(string[] arguments)
        {
            AnalyzeArguments(arguments);
            PrintArgumentFinalSelections();
            CollectPalettes();
        }

        private static void AnalyzeArguments(string[] arguments)
        {
            ArgumentOption option = ArgumentOption.Initial;

            string arg;
            foreach (string argument in arguments)
            {
                if (option != ArgumentOption.Fractal)
                {
                    arg = argument.ToUpperInvariant();
                }
                else
                {
                    arg = argument;
                }

                switch (option)
                {
                    case ArgumentOption.Initial:
                        {
                            switch (arg.ToUpperInvariant())
                            {
                                case "-ASYNC":
                                    option = ArgumentOption.Async;
                                    break;

                                case "-CIMAG":
                                    option = ArgumentOption.CImaginary;
                                    break;

                                case "-CREAL":
                                    option = ArgumentOption.CReal;
                                    break;

                                case "-DEPTH":
                                    option = ArgumentOption.Depth;
                                    break;

                                case "-FRACTAL":
                                    option = ArgumentOption.Fractal;
                                    break;

                                case "-HEIGHT":
                                    option = ArgumentOption.Height;
                                    break;

                                case "-H":
                                case "-HELP":
                                    Log.HelpAndExit();
                                    break;

                                case "-ITERATIONS":
                                    option = ArgumentOption.MaxIterations;
                                    break;

                                case "-LIGHT":
                                    option = ArgumentOption.Light;
                                    break;

                                case "-MIX":
                                    option = ArgumentOption.MixPaletteFile;
                                    break;

                                case "-OUTPUT":
                                    option = ArgumentOption.OutputFile;
                                    break;

                                case "-PALETTE":
                                    option = ArgumentOption.PaletteFile;
                                    break;

                                case "-RADIUS":
                                    option = ArgumentOption.Radius;
                                    break;

                                case "-WIDTH":
                                    option = ArgumentOption.Width;
                                    break;

                                case "-XCENTER":
                                    option = ArgumentOption.XCenter;
                                    break;

                                case "-YCENTER":
                                    option = ArgumentOption.YCenter;
                                    break;

                                case "-ZOOM":
                                    option = ArgumentOption.Zoom;
                                    break;

                                default:
                                    throw new ArgumentException($"Unrecognized option: {arg}");
                            }

                            break;
                        }

                    case ArgumentOption.Async:
                        {
                            TryParseBool("Async", arg, out bool b);
                            Async = b;
                            option = ArgumentOption.Initial;
                            break;
                        }

                    case ArgumentOption.CImaginary:
                        {
                            TryParseDouble("C imaginary", arg, out double d);
                            CImaginary = d;
                            option = ArgumentOption.Initial;
                            break;
                        }

                    case ArgumentOption.CReal:
                        {
                            TryParseDouble("C real", arg, out double d);
                            CReal = d;
                            option = ArgumentOption.Initial;
                            break;
                        }

                    case ArgumentOption.Depth:
                        {
                            Depth = arg switch
                            {
                                "8"  => Depths.Bits8,
                                "24" => Depths.Bits24,
                                "32" => Depths.Bits32,
                                _    => throw new ArgumentException($"The passed Depth value is not supported: {arg}."),
                            };
                            option = ArgumentOption.Initial;
                            break;
                        }

                    case ArgumentOption.Fractal:
                        {
                            if (!Enum.TryParse(arg, out FractalVariations f) || f == FractalVariations.Unspecified)
                            {
                                throw new ArgumentException($"The passed Fractal Variation value is not supported: {arg}.");
                            }
                            FractalVariation = f;
                            option = ArgumentOption.Initial;
                            break;
                        }

                    case ArgumentOption.Height:
                        {
                            TryParseInt("Height", arg, out int i);
                            if (i <= 0)
                            {
                                throw new ArgumentException($"The passed Height value should be greater than zero: {arg}.");
                            }
                            Height = i;
                            option = ArgumentOption.Initial;
                            break;
                        }

                    case ArgumentOption.MaxIterations:
                        {
                            TryParseInt("Iterations", arg, out int i);
                            if (i <= 0)
                            {
                                throw new ArgumentException($"The passed Iterations value should be greater than zero: {arg}.");
                            }
                            MaxIterations = i;
                            option = ArgumentOption.Initial;
                            break;
                        }

                    case ArgumentOption.Light:
                        {
                            TryParseDouble("Light", arg, out double d);
                            if (d < 0.0 || d > 1.0)
                            {
                                throw new ArgumentException($"The passed Light value should be greater than zero and less than 1: {arg}.");
                            }
                            Light = d;
                            option = ArgumentOption.Initial;
                            break;
                        }

                    case ArgumentOption.MixPaletteFile:
                        {
                            string path = Path.Combine(AbsolutePathPalettes, arg);
                            if (!File.Exists(path))
                            {
                                throw new ArgumentException($"The passed Mix Palette File does not exist: {path}.");
                            }
                            if (PaletteFileName == arg)
                            {
                                throw new ArgumentException($"The passed Mix Palette File ({arg}) cannot be the same as the Palette File ({PaletteFileName}).");
                            }
                            MixPaletteFileName = arg;
                            option = ArgumentOption.Initial;
                            break;
                        }

                    case ArgumentOption.OutputFile:
                        {
                            if (string.IsNullOrWhiteSpace(arg))
                            {
                                throw new ArgumentException("The passed Output File Name cannot be empty.");
                            }
                            OutputFileName = arg;
                            option = ArgumentOption.Initial;
                            break;
                        }

                    case ArgumentOption.PaletteFile:
                        {
                            string path = Path.Combine(AbsolutePathPalettes, arg);
                            if (!File.Exists(path))
                            {
                                throw new ArgumentException($"The passed Palette File does not exist: {path}");
                            }
                            if (MixPaletteFileName != string.Empty)
                            {
                                if (MixPaletteFileName == arg)
                                {
                                    throw new ArgumentException($"The passed Palette File ({arg}) cannot be the same as the Mixed Palette File ({MixPaletteFileName}).");
                                }
                            }
                            PaletteFileName = arg;
                            option = ArgumentOption.Initial;
                            break;
                        }

                    case ArgumentOption.Radius:
                        {
                            TryParseDouble("Radius", arg, out double d);
                            Radius = d;
                            option = ArgumentOption.Initial;
                            break;
                        }

                    case ArgumentOption.Width:
                        {
                            TryParseInt("Width", arg, out int d);
                            if (d <= 0)
                            {
                                throw new ArgumentException($"The passed Width value should be greater than zero: {d}");
                            }
                            Width = d;
                            option = ArgumentOption.Initial;
                            break;
                        }

                    case ArgumentOption.XCenter:
                        {
                            TryParseDouble("X Center", arg, out double d);
                            XCenter = d;
                            option = ArgumentOption.Initial;
                            break;
                        }

                    case ArgumentOption.YCenter:
                        {
                            TryParseDouble("Y Center", arg, out double d);
                            YCenter = d;
                            option = ArgumentOption.Initial;
                            break;
                        }

                    case ArgumentOption.Zoom:
                        {
                            TryParseDouble("Zoom", arg, out double d);
                            Zoom = (d != 0.0) ? d : throw new ArgumentException("Cannot set zoom to zero.");
                            option = ArgumentOption.Initial;
                            break;
                        }
                }
            }

            if (option != ArgumentOption.Initial)
            {
                throw new ArgumentException($"An argument was specified without a value.");
            }
        }
        private static void CreatePaletteArray(string paletteFilePath, int length, Span<Color> palette)
        {
            if (paletteFilePath != string.Empty)
            {
                string pattern = @"\s+(?'rColor'\d+(\.\d+)?)\s+(?'gColor'\d+(\.\d+)?)\s+(?'bColor'\d+(\.\d+)?)\s*";

                Log.Info($"Opening palette file '{paletteFilePath}'...");

                using var streamReader = new StreamReader(paletteFilePath);

                int i = 0;
                string? line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (i < length)
                    {
                        Match match = Regex.Match(line, pattern);
                        if (match.Success)
                        {
                            float rColorRaw = float.Parse(match.Groups["rColor"].Value);
                            float gColorRaw = float.Parse(match.Groups["gColor"].Value);
                            float bColorRaw = float.Parse(match.Groups["bColor"].Value);

                            int rColor = (int)(rColorRaw * 255);
                            int gColor = (int)(gColorRaw * 255);
                            int bColor = (int)(bColorRaw * 255);

                            palette[i] = Color.FromArgb(rColor, gColor, bColor);
                        }
                    }

                    i++;
                }
            }
        }
        private static void CollectPalettes()
        {
            CreatePaletteArray(PaletteFilePath, DefaultPaletteArrayLength, ArrayPalette.AsSpan());
            CreatePaletteArray(MixPaletteFilePath, DefaultPaletteArrayLength, ArrayMixPalette.AsSpan());
        }
        private static void TryParseDouble(string name, string value, out double parsed)
        {
            if (!double.TryParse(value, out parsed))
            {
                throw new ArgumentException($"The passed {name} value is not a valid double: {value}.");
            }
        }
        private static void TryParseInt(string name, string value, out int parsed)
        {
            if (!int.TryParse(value, out parsed))
            {
                throw new ArgumentException($"The passed {name} value is not a valid integer: {value}.");
            }
        }
        private static void TryParseBool(string name, string value, out bool parsed)
        {
            if (!bool.TryParse(value, out parsed))
            {
                throw new ArgumentException($"The passed {name} value is not a valid boolean: {value}.");
            }
        }
        private static void PrintArgumentFinalSelections()
        {
            Log.Info($"    Fractal:            {FractalVariation}");
            Log.Info($"    Color Depth:        {Depth}");
            Log.Info($"    Iterations:         {MaxIterations}");
            Log.Info($"    Light:              {Light}");
            Log.Info($"    Radius:             {Radius}");
            Log.Info($"    Resolution (W x H): {Width}, {Height}");
            Log.Info($"    Center (X, Y):      {XCenter}, {YCenter}");
            Log.Info($"    Zoom:               {Zoom}");
            Log.Info($"    C (Real, Imag):     {CReal}, {CImaginary}");
            Log.Info($"    Output File:        {OutputFileName}");
            Log.Info($"    Palette:            {PaletteFileName}");
            Log.Info($"    Mix Palette:        {MixPaletteFileName}");
            Log.Info($"    X (Min, Max)        {XMin}, {XMax}");
            Log.Info($"    Y (Min, Max)        {YMin}, {YMax}");
            Log.Info($"    Plane (W x H)       {PlaneWidth} x {PlaneHeight}");
        }
    }
}
