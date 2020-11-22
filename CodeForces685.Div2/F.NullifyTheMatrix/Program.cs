using System;

namespace F.NullifyTheMatrix
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
            var nm = Console.ReadLine().Split();
            var n = int.Parse(nm[0]);
            var m = int.Parse(nm[1]);
            var matrix = ReadMatrix(n, m);
            Console.WriteLine(IsAshishWin(n, m, matrix) ? "Ashish" : "Jeel");
        }

        private static uint[,] ReadMatrix(int n, int m)
        {
            var result = new uint[n, m];
            for (var i = 0; i < n; i++)
            {
                var row = Console.ReadLine().Split();
                for (var j = 0; j < m; j++)
                    result[i, j] = uint.Parse(row[j]);
            }

            return result;
        }

        private static bool IsAshishWin(int n, int m, uint[,] matrix)
        {
            for (var d = 0; d < n + m - 1; d++)
                if (GetDiagXor(n, m, matrix, d) != 0)
                    return true;
            return false;
        }

        private static uint GetDiagXor(int n, int m, uint[,] matrix, int d)
        {
            var firstRow = Math.Max(d - (m - 1), 0);
            var lastRow = Math.Min(d, n - 1);
            
            var xor = 0U;
            for (var row = firstRow; row <= lastRow; row++)
                xor ^= matrix[row, d - row];
            return xor;
        }
    }
}