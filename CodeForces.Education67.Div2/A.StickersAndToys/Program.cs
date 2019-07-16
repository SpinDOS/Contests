using System;

namespace A.StickersAndToys
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var T = int.Parse(Console.ReadLine());
            for (var i = 0; i < T; i++)
            {
                var nst = Console.ReadLine().Split();
                var n = int.Parse(nst[0]);
                var s = int.Parse(nst[1]);
                var t = int.Parse(nst[2]);
                Console.WriteLine(n + 1 - Math.Min(s, t));
            }
        }
    }
}