using System;
using System.Linq;

namespace A.ChooseTwoNumbers
{
    internal static class Program
    {
        public static void Main()
        {
            Console.WriteLine(ReadMax() + " " + ReadMax());
        }

        private static int ReadMax()
        {
            Console.ReadLine(); // n/m
            return Console.ReadLine().Split()
                .Select(int.Parse)
                .Max();
        }
    }
}