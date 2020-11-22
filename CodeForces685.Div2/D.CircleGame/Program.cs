using System;

namespace D.CircleGame
{
    internal sealed class Program
    {
        const int AlphabetSize = (int) 'z' - (int) 'a' + 1;

        public static void Main(string[] args)
        {
            var t = int.Parse(Console.ReadLine());
            for (var i = 0; i < t; i++)
                TestCase();
        }

        private static void TestCase()
        {
            var dk = Console.ReadLine().Split();
            var d = long.Parse(dk[0]);
            var k = long.Parse(dk[1]);
            Console.WriteLine(IsUtkarshWin(d, k) ? "Utkarsh" : "Ashish");
        }

        private static bool IsUtkarshWin(long d, long k)
        {
            // 2 * k^2 * nEdge^2 == d^2
            var nEdge = Math.Sqrt((double)Sqr(d) / (2 * Sqr(k)));
            var n = (long) nEdge;
            if (2 * Sqr(k * n) == Sqr(d))
                return true;
            // now n is so number when 2 * k^2 * n^2 < d^2 and 2 * k^2 * (n + 1)^2 > d^2
            return Sqr(k) * (Sqr(n) + Sqr(n + 1)) > Sqr(d);
        }

        private static long Sqr(long x)
        {
            return x * x;
        }
    }
}