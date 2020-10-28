using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace B.BigVova
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var t = int.Parse(Console.ReadLine());
            for (var i = 0; i < t; i++)
                HandleTestCase();
        }

        private static void HandleTestCase()
        {
            var n = int.Parse(Console.ReadLine());
            var a = Console.ReadLine().Split()
                .Select(int.Parse)
                .GroupBy(i => i)
                .ToDictionary(g => g.Key, g => g.Count());
            
            var result = new List<int>(n);
            var globalGcd = a.Keys.Max();

            while (a.Count > 0 && globalGcd > 1)
            {
                var (bestNum, bestGcd) = (0, 0);
                foreach (var curNum in a.Keys)
                {
                    var curGcd = Gcd(globalGcd, curNum);
                    if (curGcd > bestGcd)
                        (bestNum, bestGcd) = (curNum, curGcd);
                }

                globalGcd = bestGcd;
                result.AddRange(Enumerable.Repeat(bestNum, a[bestNum]));
                a.Remove(bestNum);
            }

            foreach (var kv in a)
                result.AddRange(Enumerable.Repeat(kv.Key, kv.Value));

            Console.WriteLine(string.Join(" ", result));
        }

        private static int Gcd(int a, int b)
        {
            while (b != 0)
            {
                var mod = a % b;
                a = b;
                b = mod;
            }

            return a;
        }

    }
}