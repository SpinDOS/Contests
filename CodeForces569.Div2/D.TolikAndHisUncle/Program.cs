using System;
using System.IO;
using System.Text;

namespace D.TolikAndHisUncle
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var nm = Console.ReadLine().Split();
            var n = int.Parse(nm[0]);
            var m = int.Parse(nm[1]);

            const int BytesIn128Mb = 128 * 1024 * 1024;
            
            using (var stream = Console.OpenStandardOutput(BytesIn128Mb))
                using (var writer = new StreamWriter(stream, Encoding.ASCII))    
                    PrintSolution(n, m, writer);
        }

        private static void PrintSolution(int n, int m, StreamWriter writer)
        {
            var mPlus1 = m + 1;

            for (int rowHigh = 1, rowLow = n; rowHigh < rowLow; rowHigh++, rowLow--)
            {
                string 
                    rowHighStr = rowHigh + " ",
                    rowLowStr = rowLow + " ";
                
                for (var col = 1; col <= m; col++)
                {
                    writer.WriteLine(rowHighStr + col);
                    writer.WriteLine(rowLowStr + (mPlus1 - col));
                }
            }

            if (n % 2 == 0)
                return;

            var row = (n / 2 + 1) + " ";
            for (var col = 1; col <= m / 2; col++)
            {
                writer.WriteLine(row + col);
                writer.WriteLine(row + (mPlus1 - col));
            }

            if (m % 2 == 1)
                writer.WriteLine(row + (m / 2 + 1));
        }
    }
}