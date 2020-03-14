#region Using directives

using System.Numerics;

#endregion

namespace FractalGenerator
{
    public struct Calculation
    {
        public double Iterations;
        public Complex LastZ;

        // Used only for Newton
        public double AbsZR1;
        public double AbsZR2;
        public double AbsZR3;
    }
}
