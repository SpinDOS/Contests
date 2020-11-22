using System;

namespace A.SubtractOrDivide
{
    internal sealed class Program
    {
        public static void Main(string[] args)
        {
            var t = int.Parse(Console.ReadLine());
            for (var i = 0; i < t; i++)
                TestCase();                
        }

        private static void TestCase()
        {
            var n = int.Parse(Console.ReadLine());
            Console.WriteLine(Solve(n));
        }

        private static int Solve(int n)
        {
            if (n <= 3)
                return n - 1;
            return n % 2 == 0? 2 : 3;
        }
    }
}