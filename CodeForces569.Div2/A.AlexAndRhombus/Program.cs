using System;

namespace A.AlexAndRhombus
{
    internal static class Program
    {
        public static void Main()
        {
            var n = int.Parse(Console.ReadLine());
            var cells = 2 * n * (n - 1) + 1;
            Console.WriteLine(cells);
        }
    }
}