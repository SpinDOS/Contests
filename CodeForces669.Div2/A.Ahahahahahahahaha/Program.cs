using System;
using System.Collections.Generic;

namespace A.Ahahahahahahahaha
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
            Console.ReadLine(); // n
            var a = Console.ReadLine().Split();
            var newA = Solve(a);
            Console.WriteLine(newA.Count);
            Console.WriteLine(string.Join(" ", newA));
        }

        private static List<string> Solve(string[] a)
        {
            var result = new List<string>(a.Length);
            for (var i = 0; i < a.Length; i += 2)
            {
                if (a[i] == a[i + 1])
                {
                    result.Add(a[i]);
                    result.Add(a[i]);
                }
                else
                {
                    result.Add("0");
                }
            }

            return result;
        }
    }
}