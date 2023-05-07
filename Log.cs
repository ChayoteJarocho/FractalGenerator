using System;
using System.Diagnostics.CodeAnalysis;

namespace FractalGenerator;

public class Log
{
    public static void Line() => Console.WriteLine();

    public static void Info(string format, params object[] args) => Info(true, format, args);

    public static void Info(bool endline, string format, params object[] args) => Print(endline, ConsoleColor.White, format, args);

    public static void Success(string format, params object[] args) => Success(true, format, args);

    public static void Success(bool endline, string format, params object[] args) => Print(endline, ConsoleColor.Green, format, args);

    public static void Warning(string format, params object[] args) => Warning(true, format, args);

    public static void Warning(bool endline, string format, params object[] args) => Print(endline, ConsoleColor.Yellow, format, args);

    public static void Error(string format, params object[] args) => Error(true, format, args);

    public static void Error(bool endline, string format, params object[] args) => Print(endline, ConsoleColor.Red, format, args);

    public static void Assert(bool condition, string format, params object[] args) => Assert(true, condition, format, args);

    public static void Assert(bool endline, bool condition, string format, params object[] args)
    {
        if (condition)
        {
            Success(endline, format, args);
        }
        else
        {
            Error(endline, format, args);
        }
    }

    [DoesNotReturn]
    public static void ExceptionAndExit(Exception e)
    {
        string s = string.Empty;

        s += "\r\n";
        s += $"  Exception message:";
        s += $"    {e.Message}\r\n";
        s += $"    {e.StackTrace}\r\n";

        if (e.InnerException != null)
        {
            s += "\r\n";
            s += "  Inner exception:\r\n";
            s += $"    {e.InnerException.Message}\r\n";
            s += $"    {e.InnerException.StackTrace}\r\n";
        }

        s += "\r\n";

        Error(s);
        Goodbye();
    }

    [DoesNotReturn]
    public static void ErrorHelpAndExit(string error)
    {
        Error(error);
        Line();
        HelpAndExit();
    }

    [DoesNotReturn]
    public static void HelpAndExit()
    {
        Help();
        Goodbye();
    }

    private static void Help()
    {
        Warning($@"Usage:
[-CIMAG][-CREAL][-FRACTAL][-H | HELP][-HEIGHT][-ITERATIONS][-LIGHT][-MIX][-OUTPUT][-PALETTE][-RADIUS][-WIDTH][-XCENTER][-YCENTER][-ZOOM]

-H | HELP         : Show this help message.

-CIMAG <double>   : For Julia, you can set the imaginary value of C. Default: {Configuration.CImaginary}
-CREAL <double>   : For Julia, you can set the real value of C. Default: {Configuration.CReal}
-FRACTAL <string> : Fractal variation. Acceptable values: Mandelbrot, Julia, Newton, Spiderweb. Default: {Configuration.FractalVariation}
-HEIGHT <int>     : Height of the image, in pixels. Must be a positive integer number. Default: {Configuration.Height}
-ITERATIONS <int> : Iterations inside the algorithm. Must be a positive integer number. The bigger the number, the higher the contrast. Default: {Configuration.MaxIterations}
-LIGHT <double>   : Light management (contrast). Must be a double number between 0.0 and 1.0. Default: {Configuration.Light}
-OUTPUT <string>  : Output file. Must not have spaces, unless it is between quotation marks. Extension must be bmp. Default: {Configuration.OutputFileName}
-PALETTE <string> : Mathlab base color palette file. Must ve a valid Mathlab palette, and must exist inside the palettes directory. Default:  {Configuration.PaletteFileName}
-MIX <string>     : Optional extra Mathlab color palette that will be mixed with the base palette file. Must ve a valid Mathlab palette, and must exist inside the palettes directory. Default is empty.
-RADIUS <double>  : Escaping radius limit. Must be a positive double number. If fractal is Newton, must be a float number < 1.0. Default: {Configuration.Radius}
-WIDTH <int>      : Width of the image, in pixels, must be a positive integer number. Default: {Configuration.Width}
-XCENTER <double> : The x coordinate of the point to zoom in the plane. Default: {Configuration.YCenter}.
-YCENTER <double> : The y coordinate of the point to zoom in the plane. Default: {Configuration.YCenter}.
-ZOOM <double>    : The amount of zoom on the image. The default is {Configuration.Zoom}.
");
    }

    [DoesNotReturn]
    private static void Goodbye()
    {
        Line();
        Error("Goodbye!");
        Line();
        Environment.Exit(0);
    }

    private static void Print(bool endline, ConsoleColor foregroundColor, string format, params object[] args)
    {
        ConsoleColor initialColor = Console.ForegroundColor;
        Console.ForegroundColor = foregroundColor;
        if (endline)
        {
            Console.WriteLine(format, args);
        }
        else
        {
            Console.Write(format, args);
        }
        Console.ForegroundColor = initialColor;
    }
}
