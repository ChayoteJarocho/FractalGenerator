## FractalGenerator

Tool to generate fractal images.

```
Usage: [-CIMAG][-CREAL][-D][-F][-H][-HELP][-I][-L][-M][-O][-P][-R][-W][-XMAX][-XMIN][-YMAX][-YMIN]

-CIMAG <double> : For Julia, you can set the imaginary value of C. Default: 0.75
-CREAL <double> : For Julia, you can set the real value of C. Default: -0.2
-D <int>        : An integer representing the color depth (bits) of the image. Acceptable values: 8, 24, 32. Default: 24
-F <string>     : Fractal variation. Acceptable values: Mandelbrot, Julia, Newton, Spiderweb. Default: Mandelbrot
-H <int>        : Height of the image, in pixels. Must be a positive integer number. Default: 200
-HELP           : Show this help message.
-I <int>        : Iterations inside the algorithm. Must be a positive integer number. The bigger the number, the higher the contrast. Default: 200
-L <double>     : Light management (contrast). Must be a double number between 0.0 and 1.0. Default: 0
-O <string>     : Output file. Must not have spaces, unless it is between quotation marks. Extension must be bmp. Default: output.bmp
-P <string>     : Mathlab base color palette file. Must ve a valid Mathlab palette, and must exist inside the palettes directory. Default:  hsv.txt
-M <string>     : Optional extra Mathlab color palette that will be mixed with the base palette file. Must ve a valid Mathlab palette, and must exist inside the palettes directory.
-R <double>     : Escaping radius limit. Must be a positive double number. If fractal is Newton, must be a float number < 1.0. Default: 1E+20
-W <int>        : Width of the image, in pixels, must be a positive integer number. Default: 320
-XMAX <double>  : Maximum x value of the plane, must be a positive double number. Default: 2.5
-XMIN <double>  : Minimum x value of the plane, must be a positive double number. Default: -3.5
-YMAX <double>  : Maximum y value of the plane, must be a positive double number. Default: 2.25
-YMIN <double>  : Minimum y value of the plane, must be a positive double number. Default: -2.25
```

Examples:

![Mandelbrot](mandelbrot.bmp)
![Julia](julia.bmp)