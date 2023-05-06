using System;

/*
    Created by Carlos Sánchez López
    Sources:

    - pgm/ppm/pbm formats:
    http://en.wikipedia.org/wiki/Netpbm_format

    - bmp format
    http://en.wikipedia.org/wiki/BMP_file_format

    - matlab colormap:
    http://www.mathworks.com/access/helpdesk/help/techdoc/ref/colormap.html

    - Mandelbrot set, programming, coloring and smoothing
    http://en.wikipedia.org/wiki/Mandelbrot_set

    - Smooth shading for the Mandelbrot exterior
    http://linas.org/art-gallery/escape/smooth.html

    - Coloring the Mandelbrot set
    http://yozh.org/mset_index/

    - Mandelbulb (3D Mandelbrot)
    http://www.skytopia.com/project/fractal/mandelbulb.html

    - Fast BMPs in C#:
    https://www.codeproject.com/Tips/240428/Work-with-bitmap-faster-with-Csharp

    fraction: number between {0,1}
    the bigger the radius, the farther fraction is from 0
    k: number between 0 and iterations
    k+1-fraccion:                   minvalue=0, maxvalue=11
    colors_palette/(iterations+1):  minvalue=1, maxvalue=colors_palette
    everything:                     minvalue=0, maxvalue=11*colors_palette
*/

namespace FractalGenerator;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Success("-------------------------");
        Log.Success("--- FRACTAL GENERATOR ---");
        Log.Success("-------------------------");



        try
        {
            Configuration.Verify(args);

            Fractal fractal = Configuration.FractalVariation switch
            {
                FractalVariations.Julia =>      new Julia(),
                FractalVariations.Mandelbrot => new Mandelbrot(),
                FractalVariations.Newton =>     new Newton(),
                FractalVariations.Spiderweb =>  new Spiderweb(),
                _ => throw new ArgumentException($"Unknown fractal variation: {Configuration.FractalVariation}"),
            };

            fractal.Generate();
            Fractal.Open();
        }
        catch(Exception e)
        {
            Log.ExceptionAndExit(e);
        }
    }
}
