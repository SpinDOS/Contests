using System;
using System.Collections.Generic;
using System.Linq;

namespace B.Chess
{
    class Program
    {
        private const string NoSolution = "NO SOLUTION";
        static void Main(string[] args)
        {
            var n = int.Parse(Console.ReadLine());
            var maxGameCount = (int) Math.Round(Math.Log(n + 1, 2));

            var playedWith = new Dictionary<string, List<string>>(n + 1);

            for (var i = 0; i < n; i++)
            {
                var players = Console.ReadLine().Split();
                var a = players[0];
                var b = players[1];
                
                List<string> opponents;
                
                if (!playedWith.TryGetValue(a, out opponents))
                    playedWith.Add(a, opponents = new List<string>(maxGameCount));
                opponents.Add(b);
                
                if (!playedWith.TryGetValue(b, out opponents))
                    playedWith.Add(b, opponents = new List<string>(maxGameCount));
                opponents.Add(a);
            }

            if (playedWith.Count != n + 1)
            {
                Console.WriteLine(NoSolution);
                return;
            }

            var winners = new List<string>(2);
            var hasGamesCountBuffer = new bool[maxGameCount];
            
            foreach (var playerOpponents in playedWith)
            {
                if (playerOpponents.Value.Count == maxGameCount)
                    winners.Add(playerOpponents.Key);
                
                Array.Clear(hasGamesCountBuffer, 0, maxGameCount);
                
                foreach (var opponent in playerOpponents.Value)
                {
                    var countOfGames = playedWith[opponent].Count - 1;
                    if (countOfGames >= hasGamesCountBuffer.Length || hasGamesCountBuffer[countOfGames])
                    {
                        Console.WriteLine(NoSolution);
                        return;
                    }

                    hasGamesCountBuffer[countOfGames] = true;
                }
            }
            
            Console.WriteLine(string.Join(" ", winners));
        }
    }
}