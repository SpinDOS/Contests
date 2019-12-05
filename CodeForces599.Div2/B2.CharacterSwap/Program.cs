using System;
using System.Collections.Generic;
 
namespace B1.CharacterSwap
{
    internal static class Program
    {
        public static void Main()
        {
            var k = int.Parse(Console.ReadLine());
            for (var i = 0; i < k; i++)
            {
                var n = int.Parse(Console.ReadLine());
                var s = Console.ReadLine().ToCharArray();
                var t = Console.ReadLine().ToCharArray();
 
                var swaps = Solve(n, s, t);
                Console.WriteLine(swaps != null ? "YES" : "NO");
                
                if (swaps == null)
                    continue;
                
                Console.WriteLine(swaps.Count);
                foreach (var swap in swaps)
                    Console.WriteLine($"{swap.I + 1} {swap.J + 1}");
            }
        }
 
        private static List<Swap> Solve(int n, char[] s, char[] t)
        {
            // abba -> abbc -> abdc -> abdd -> abdc
            // cdcd -> adcd -> abcd -> abcc -> abdc
            
            var result = new List<Swap>(2 * n);

            for (var i = 0; i < n; i++)
            {
                if (s[i] == t[i])
                    continue;

                var j = FindUnmatched(n, s, t, s[i], i + 1);
                if (j < 0)
                {
                    j = FindUnmatched(n, t, s, s[i], i + 1);
                    if (j < 0)
                        return null;
                    
                    result.Add(DoSwap(s, t, j, j));
                }

                result.Add(DoSwap(s, t, j, i));
            }
 
            return result;
        }

        private static Swap DoSwap(char[] s, char[] t, int i, int j)
        {
            var temp = s[i];
            s[i] = t[j];
            t[j] = temp;
            return new Swap(i, j);
        }

        private static int FindUnmatched(int n, char[] s1, char[] s2, char ch, int startIndex)
        {
            for (var i = startIndex; i < n; i++)
                if (s1[i] == ch && s2[i] != ch)
                    return i;
            return -1;
        }
 
        private struct Swap
        {
            public Swap(int i, int j)
            {
                I = i;
                J = j;
            }
        
            public int I { get; }
            public int J { get; }
        }
    }
}