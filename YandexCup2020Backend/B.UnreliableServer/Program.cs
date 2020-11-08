using System;

namespace B.UnreliableServer
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var n = int.Parse(Console.ReadLine());
            
            var maxSeen = 1;
            var cur = 2.0;

            var okRequests = 0;
            var totalRequests = 0;
            while (okRequests < n && totalRequests < 100_000)
            {
                Console.WriteLine(Math.Min((int)Math.Round(cur), 1000));
                var real = int.Parse(Console.ReadLine());
                totalRequests++;
                if (real == -1)
                {
                    cur *= 1.02;
                    continue;
                }

                okRequests++;
                maxSeen = Math.Max(maxSeen, real);
                cur = Math.Max(cur / 1.03, maxSeen * 1.05);
            }
        }
    }
}