using System;

namespace B.Temperature
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var n = int.Parse(Console.ReadLine());

            var temperature = new int[2 * n + 1];
            var tempStrings = Console.ReadLine().Split();
            for (var i = 0; i < temperature.Length; i++)
                temperature[i] = int.Parse(tempStrings[i]);

            var minMax = new MinMax[temperature.Length];
            minMax[n] = new MinMax() {Max = temperature[n], Min = temperature[n],};
            for (var i = n - 1; i >= 0; i--)
                minMax[i] = new MinMax(minMax[i + 1], temperature[i]);
            for (var i = n + 1; i < minMax.Length; i++)
                minMax[i] = new MinMax(minMax[i - 1], temperature[i]);
            
            var b = int.Parse(Console.ReadLine());
            for (var i = 0; i < b; i++)
            {
                var parts = Console.ReadLine().Split();
                
                var tmin = int.Parse(parts[0]);
                var tmax = int.Parse(parts[1]);
                var dmin = int.Parse(parts[2]) + n;
                var dmax = int.Parse(parts[3]) + n;

                var minTemp = Math.Min(minMax[dmin].Min, minMax[dmax].Min);
                var maxTemp = Math.Max(minMax[dmin].Max, minMax[dmax].Max);

                var yes = minTemp <= tmax && maxTemp >= tmin;
                Console.WriteLine(yes ? "yes" : "no");
            }
        }

        private struct MinMax
        {
            public MinMax(MinMax another, int temperature)
            {
                Min = Math.Min(another.Min, temperature);
                Max = Math.Max(another.Max, temperature);
            }
            
            public int Min { get; set; }
            public int Max { get; set; }
        }
    }
}