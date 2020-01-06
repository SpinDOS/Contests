using System;

namespace A.TemporarilyUnavailable
{
    internal static class Program
    {
        public static void Main()
        {
            var t = int.Parse(Console.ReadLine());
            for (var i = 0; i < t; i++)
            {
                var abcr = Console.ReadLine().Split();
                
                var unavailableTime = Solve(
                    long.Parse(abcr[0]),
                    long.Parse(abcr[1]),
                    long.Parse(abcr[2]),
                    long.Parse(abcr[3]));

                Console.WriteLine(unavailableTime);
            }
        }

        private static long Solve(long a, long b, long c, long r)
        {
            if (a > b)
                Swap(ref a, ref b);
            
            var networkStart = Math.Max(a, c - r);
            var networkEnd = Math.Min(b, c + r);

            if (networkStart >= b || networkEnd <= a)
                return b - a;

            var unavailableTime = 0L;

            if (networkStart > a)
                unavailableTime += networkStart - a;
            
            if (networkEnd < b)
                unavailableTime += b - networkEnd;

            return unavailableTime;
        }

        private static void Swap(ref long a, ref long b)
        {
            var c = a;
            a = b;
            b = c;
        }
    }
}