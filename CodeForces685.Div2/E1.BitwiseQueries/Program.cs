using System;
using System.Collections.Generic;

namespace E1.BitwiseQueries
{
    internal sealed class Program
    {
        public static void Main(string[] args)
        {
            var n = int.Parse(Console.ReadLine());
            
            string answer;
            try 
            { 
                answer = string.Join(" ", new Solver().Solve(n));
            }
            catch (TooManyRequestsException)
            {
                return;
            }
            
            Console.Write("! ");
            Console.WriteLine(answer);
        }
    }
    
    internal sealed class TooManyRequestsException : Exception { }

    internal sealed class Solver
    {
        public IEnumerable<uint> Solve(int n)
        {
            var and01 = Ask("AND", 0, 1);
            var and02 = Ask("AND", 0, 2);
            var and12 = Ask("AND", 1, 2);
            var xor01 = Ask("XOR", 0, 1);
            var xor02 = Ask("XOR", 0, 2);

            var firstElement = and01 | and02 | (xor01 & xor02 & ~and12);
            yield return firstElement;
            yield return firstElement ^ xor01;
            yield return firstElement ^ xor02;

            for (var i = 3; i < n; i++)
                yield return firstElement ^ Ask("XOR", 0, i);
        }

        private static uint Ask(string operation, int i, int j)
        {
                Console.WriteLine("{0} {1} {2}", operation, i + 1, j + 1);
                var answer = int.Parse(Console.ReadLine());
                if (answer < 0)
                    throw new TooManyRequestsException();
                return (uint)answer;
        }
    }
}