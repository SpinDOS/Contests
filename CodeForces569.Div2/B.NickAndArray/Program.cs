using System;
using System.Linq;

namespace B.NickAndArray
{
    internal static class Program
    {
        public static void Main()
        {
            var n = int.Parse(Console.ReadLine());
            var arr = Console.ReadLine().Split().Select(int.Parse).ToArray();
            Solve(arr);
            Console.WriteLine(string.Join(" ", arr));
        }

        private static void Solve(int[] arr)
        {
            var minIndex = 0;
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] >= 0)
                    arr[i] = -arr[i] - 1;
                
                if (arr[i] < arr[minIndex])
                    minIndex = i;
            }

            if (arr.Length % 2 == 1)
                arr[minIndex] = -arr[minIndex] - 1;
        }
    }
}