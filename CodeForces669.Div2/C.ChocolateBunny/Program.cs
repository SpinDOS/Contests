using System;

namespace C.ChocolateBunny
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var n = int.Parse(Console.ReadLine());
            try
            {
                Console.WriteLine("! " + string.Join(" ", Solve(n)));
            }
            catch (GotMinusOneException)
            {
                // ignore
            }
        }

        private static int[] Solve(int n)
        {
            var result = new int[n];
            var leftIndex = 0;
            for (var rightIndex = 1; rightIndex < n; rightIndex++)
            {
                var lModR = Ask(leftIndex, rightIndex);
                var rModL = Ask(rightIndex, leftIndex);
                if (lModR < rModL)
                {
                    result[rightIndex] = rModL;
                }
                else
                {
                    result[leftIndex] = lModR;
                    leftIndex = rightIndex;
                }
            }

            result[leftIndex] = n;
            return result;
        }

        private static int Ask(int x, int y)
        {
            Console.WriteLine($"? {x + 1} {y + 1}");
            var ans = int.Parse(Console.ReadLine());
            return ans == -1 ? throw new GotMinusOneException() : ans;
        }
        
        private sealed class GotMinusOneException : Exception { }
    }
}