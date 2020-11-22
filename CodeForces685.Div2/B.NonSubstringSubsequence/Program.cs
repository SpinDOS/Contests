using System;

namespace B.NonSubstringSubsequence
{
    internal sealed class Program
    {
        public static void Main(string[] args)
        {
            var t = int.Parse(Console.ReadLine());
            for (var i = 0; i < t; i++)
                TestCase();                
        }

        private static void TestCase()
        {
            var nq = Console.ReadLine().Split();
            var n = int.Parse(nq[0]);
            var q = int.Parse(nq[1]);
            var solver = new Solver(Console.ReadLine());
            for (var i = 0; i < q; i++)
            {
                var lr = Console.ReadLine().Split();
                var answer = solver.HasSubstring(int.Parse(lr[0]) - 1, int.Parse(lr[1]) - 1);
                Console.WriteLine(answer ? "yes" : "no");
            }
        }
    }

    internal sealed class Solver
    {
        private readonly bool[] _isGoodAsFirst;
        private readonly bool[] _isGoodAsLast;
        
        public Solver(string s)
        {
            bool hasZero, hasOne;
            
            _isGoodAsFirst = new bool[s.Length];
            hasZero = hasOne = false;
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] == '0')
                {
                    _isGoodAsFirst[i] = hasZero;
                    hasZero = true;
                }
                else
                {
                    _isGoodAsFirst[i] = hasOne;
                    hasOne = true;
                }
            }
            
            _isGoodAsLast = new bool[s.Length];
            hasZero = hasOne = false;
            for (var i = s.Length - 1; i >= 0; i--)
            {
                if (s[i] == '0')
                {
                    _isGoodAsLast[i] = hasZero;
                    hasZero = true;
                }
                else
                {
                    _isGoodAsLast[i] = hasOne;
                    hasOne = true;
                }
            }
        }

        public bool HasSubstring(int l, int r) => r > l && (_isGoodAsFirst[l] || _isGoodAsLast[r]);
    }
}