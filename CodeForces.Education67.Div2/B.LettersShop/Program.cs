using System;
using System.Collections.Generic;
using System.Linq;

namespace B.LettersShop
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.ReadLine(); // n
            
            var solver = new Solver();
            solver.Init(Console.ReadLine());
            
            var m = int.Parse(Console.ReadLine());
            
            for (var i = 0; i < m; i++)
                Console.WriteLine(solver.Solve(Console.ReadLine()));
        }
    }

    internal class Solver
    {
        private readonly int[] _charBuffer = new int[30];
        private Dictionary<LetterInfo, int> _minCharsCounts;
        
        public void Init(string s)
        {
            var minCharsCounts = _minCharsCounts = new Dictionary<LetterInfo, int>(s.Length);
            
            var needLength = 0;
            foreach (var ch in s)
                minCharsCounts.Add(new LetterInfo(ch, ++_charBuffer[ch - 'a']), ++needLength);
        }

        public int Solve(string t)
        {
            Array.Clear(_charBuffer, 0, _charBuffer.Length);
            return t.Max(ch => _minCharsCounts[new LetterInfo(ch, ++_charBuffer[ch - 'a'])]);
        }
    }

    internal struct LetterInfo : IEquatable<LetterInfo>
    {
        public LetterInfo(char letter, int count)
        {
            Letter = letter;
            Count = count;
        }
        
        public char Letter { get; }
        public int Count { get; }

        public bool Equals(LetterInfo other)
        {
            return Letter == other.Letter && Count == other.Count;
        }

        public override bool Equals(object obj)
        {
            return obj is LetterInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Letter.GetHashCode() * 397) ^ Count;
            }
        }
    }
}