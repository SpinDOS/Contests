using System;
using System.Collections.Generic;
using System.Linq;

namespace B2.KForThePriceOfOne
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
            for (var rollingStartGroup = 0; rollingStartGroup < k; rollingStartGroup++)
            {
                var m = rollingStartGroup + k * CountKGroups(a, rollingStartGroup, p, k);
                maxM = Math.Max(maxM, m);
                
                p -= a[rollingStartGroup];
                if (p < 0)
                    break;
            }

            return maxM;
        }

        private static int CountKGroups(int[] arr, int offset, int p, int k)
        {
            var count = 0;
            for (var i = offset + k - 1; i < arr.Length && p >= arr[i]; i += k)
            {
                count++;
                p -= arr[i];
            }

            return count;
        }
    }
}