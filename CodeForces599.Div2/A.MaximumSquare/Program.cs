using System;
using System.Collections.Generic;
using System.Linq;

namespace B1.CharacterSwap
{
    internal static class Program
    {
        public static void Main()
        {
            var k = int.Parse(Console.ReadLine());
            for (var i = 0; i < k; i++)
            {
                var planks = ReadPlanks();
                Console.WriteLine(Solve(planks));
            }
        }

        private static List<int> ReadPlanks()
        {
            Console.ReadLine(); // n
            return Console.ReadLine().Split().Select(int.Parse).ToList();
        }

        private static int Solve(List<int> planks)
        {
            planks.Sort();
            planks.Reverse();
            
            var countOfLongEnoughPlanks = 0;
            foreach (var plankLength in planks)
            {
                if (plankLength <= countOfLongEnoughPlanks || plankLength == ++countOfLongEnoughPlanks)
                    break;
            }

            return countOfLongEnoughPlanks;
        }
    }
}