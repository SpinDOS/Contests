using System;

namespace F.MaximumSine
{
    internal sealed class Program
    {
        public static void Main()
        {
            var t = uint.Parse(Console.ReadLine());

            for (var i = 0; i < t; i++)
            {
                var strs = Console.ReadLine().Split();
                var result = Solver.Solve(
                    Parse(strs, 0),
                    Parse(strs, 1),
                    Parse(strs, 2),
                    Parse(strs, 3));

                Console.WriteLine(result);
            }
        }

        private static uint Parse(string[] strs, int ind) => uint.Parse(strs[ind]);
    }
    
    internal sealed class Solver
    {
        public static ulong Solve(uint a, uint b, uint p, uint q)
        {
            var gcd = Gcd(p, q, out var c, out var _);
            q /= gcd;
            if (2UL * (b - a) < q)
                return SolveByX(a, b, p / gcd, q);

            c %= q;
            var ulongC = (ulong) (c < 0? c + q : c);
            return SolveByQ(a, b, q, ulongC);
        }

        private static ulong SolveByQ(ulong a, ulong b, ulong q, ulong c)
        {
            ulong remLow = q / 2, 
                remHigh = remLow + (q % 2), 
                aStart = q * (a / q);
            
            var iterationCount = (int)remLow + 1;
            for (var i = 0; i < iterationCount; i++)
            {
                var xLow = aStart + ((remLow-- * c) % q);
                if (xLow < a)
                    xLow += q;
                
                var xHigh = aStart + ((remHigh++ * c) % q);
                if (xHigh < a)
                    xHigh += q;

                var x = Math.Min(xLow, xHigh);
                if (x <= b)
                    return x;
            }
            
            throw new Exception("Not found solution");
        }

        private static ulong SolveByX(ulong a, ulong b, ulong p, ulong q)
        {
            ulong target1 = q / 2, 
                target2 = target1 + (q % 2), 
                bestX = a,
                bestDist = ulong.MaxValue;

            for (var x = a; x <= b; x++)
            {
                var reminder = (p * x) % q;
                var dist = reminder >= target2? reminder - target2 : target1 - reminder;
                if (dist <= 1)
                    return x;

                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestX = x;
                }
            }

            return bestX;
        }

        private static uint Gcd(uint a, uint b, out long x, out long y)
        {
            // solve a * x + b * y = gcd(a, b)
            // http://e-maxx.ru/algo/export_extended_euclid_algorithm
            
            if (a == 0)
            {
                x = 0;
                y = 1;
                return b;
            }

            var gcd = Gcd(b % a, a, out var x1, out var y1);
            x = checked(y1  - (x1 * (b / a)));
            y = x1;
            
            return gcd;
        }
    }
}