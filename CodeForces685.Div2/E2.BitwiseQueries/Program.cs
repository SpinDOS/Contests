using System;
using System.Collections.Generic;
using System.Linq;

namespace E2.BitwiseQueries
{
    internal sealed class Program
    {
        public static void Main(string[] args)
        {
            var n = int.Parse(Console.ReadLine());
            
            string answer;
            try 
            { 
                answer = string.Join(" ", new Solver(n).Solve());
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
        private readonly int[] _xors;
        
        public Solver(int n)
        {
            _xors = new int[n];
        }

        public IEnumerable<int> Solve()
        {
            FillXors();
            var firstElement = CalculateFirstElement();
            return _xors.Select(it => firstElement ^ it);
        }

        private void FillXors()
        {
            for (var i = 1; i < _xors.Length; i++)
                _xors[i] = Ask("XOR", 0, i);
        }

        private int CalculateFirstElement()
        {
            const int smallestBitDiffersXor = 1 << 0;
            const int smallestBitSameXor = 1 << 1;
            
            var xorToIndex = new Dictionary<int, int>(_xors.Length);
            xorToIndex.Add(0, 0);
            for (var j = 1; j < _xors.Length; j++)
            {
                int i;
                if (xorToIndex.TryGetValue(_xors[j], out i))
                    return _xors[j] ^ Ask("AND", i, j);
                xorToIndex.Add(_xors[j], j);
            }

            var allButSmallestBit = Ask("AND", 0, xorToIndex[smallestBitDiffersXor]);
            var withSmallestBit = Ask("AND", 0, xorToIndex[smallestBitSameXor]);
            return allButSmallestBit | (withSmallestBit & 1);
        }

        private static int Ask(string operation, int i, int j)
        {
                Console.WriteLine("{0} {1} {2}", operation, i + 1, j + 1);
                var answer = int.Parse(Console.ReadLine());
                if (answer < 0)
                    throw new TooManyRequestsException();
                return answer;
        }
    }
}