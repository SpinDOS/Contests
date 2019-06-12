using System;

namespace F.SearchCommit
{
    class Program
    {
        static void Main(string[] args)
        {
            var n = int.Parse(Console.ReadLine());
            
            var maxPass = 0;
            var minFail = n;

            while (maxPass + 1 < minFail)
            {
                var mid = maxPass + (minFail - maxPass) / 2;
                Console.WriteLine(mid);
                if (Console.ReadLine() == "1")
                    maxPass = mid;
                else
                    minFail = mid;
            }

            Console.WriteLine("! " + minFail);
        }
    }
}