using System;
using System.Collections.Generic;
using System.Linq;

namespace B1.KForThePriceOfOne
{
    internal static class Program
    {
        public static void Main()
        {
            var t = int.Parse(Console.ReadLine());
            for (var i = 0; i < t; i++)
            {
                var npk = Console.ReadLine().Split();
                var a = Console.ReadLine().Split().Select(int.Parse);
                var m = Solve(
                    int.Parse(npk[0]),
                    int.Parse(npk[1]),
                    int.Parse(npk[2]),
                    a);

                Console.WriteLine(m);
            }
        }

        private static int Solve(int n, int p, int k, IEnumerable<int> rawA)
        {
            var a = rawA.OrderBy(it => it).ToArray();
            
            var maxM = 0;
            for (var buyWithoutK = 0; buyWithoutK < k; buyWithoutK++)
                maxM = Math.Max(maxM, GetM(p, k, a, buyWithoutK));

            return maxM;
        }

        private static int GetM(int p, int k, int[] a, int buyWithoutK)
        {
            var m = 0;
            var i = 0;

            for (; i < buyWithoutK && a[i] <= p; i++)
            {
                m++;
                p -= a[i];
            }
            
            while (i + k <= a.Length && a[i + k - 1] <= p)
            {
                i += k;
                m += k;
                p -= a[i - 1];
            }

            for (; i < a.Length && a[i] <= p; i++)
            {
                m++;
                p -= a[i];
            }

            return m;
        }
    }
}