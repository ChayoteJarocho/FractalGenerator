using System.Numerics;

namespace FractalGenerator
{
    public struct Calculation
    {
        public ulong Iterations;
        public Complex LastZ;

        // Used only for Newton
        public double AbsZR1;
        public double AbsZR2;
        public double AbsZR3;
    }
}
