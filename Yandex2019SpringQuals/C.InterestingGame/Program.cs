using System;
using System.Linq;

namespace C.InterestingGame
{
    class Program
    {
        static void Main(string[] args)
        {
            var kn = Console.ReadLine().Split();
            var k = int.Parse(kn[0]);

            var petya = 0;
            var vasya = 0;

            bool? petyaWins = null;

            foreach (var x in Console.ReadLine().Split().Select(int.Parse))
            {
                if (x % 3 == 0 && x % 5 == 0)
                    continue;
                
                if (x % 3 == 0)
                {
                    if (++petya == k)
                    {
                        petyaWins = true;
                        break;
                    }
                }
                else if (x % 5 == 0)
                {
                    if (++vasya == k)
                    {
                        petyaWins = false;
                        break;
                    }
                }
            }

            if (petyaWins == null && petya != vasya)
                petyaWins = petya > vasya;
            
            if (petyaWins == null)
                Console.WriteLine("Draw");
            else if (petyaWins.Value)
                Console.WriteLine("Petya");
            else
                Console.WriteLine("Vasya");
        }
    }
}