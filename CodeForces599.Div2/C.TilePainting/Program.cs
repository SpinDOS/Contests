using System;

namespace C.TilePainting
{
    internal static class Program
    {
        public static void Main()
        {
            var n = ulong.Parse(Console.ReadLine());
            Console.WriteLine(GetColorsCount(n));
        }

        private static ulong GetColorsCount(ulong n)
        {
            if (n == 1)
                return 1;
            
            var minDivisor = GetMinDivisor(n);
            while (n % minDivisor == 0)
                n /= minDivisor;

            return n == 1? minDivisor : 1;
        }

        private static ulong GetMinDivisor(ulong n)
        {
            var maxPossibleDivisor = (ulong) Math.Sqrt(n) + 1;
            for (ulong i = 2; i <= maxPossibleDivisor; i++)
                if (n % i == 0)
                    return i;

            return n;
        }
    }
}