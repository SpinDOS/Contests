using System;

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
                var s = Console.ReadLine();
                var t = Console.ReadLine();
                Console.WriteLine(CanSwap(n, s, t) ? "YES" : "NO");
            }
        }

        private static bool CanSwap(int n, string s, string t)
        {
            var state = StringInspectState.NoDiff;
            var firstDiffIndex = 0;
            
            for (var i = 0; i < n; i++)
            {
                if (s[i] == t[i])
                    continue;

                switch (state)
                {
                    case StringInspectState.NoDiff:
                        firstDiffIndex = i;
                        state = StringInspectState.FoundFirstDiff;
                        break;
                    case StringInspectState.FoundFirstDiff:
                        if (s[firstDiffIndex] != s[i] || t[firstDiffIndex] != t[i])
                            return false;
                        state = StringInspectState.FoundBothDiffs;
                        break;
                    case StringInspectState.FoundBothDiffs:
                        return false;
                }
            }

            return state == StringInspectState.FoundBothDiffs;
        }
        
        private enum StringInspectState : byte
        {
            NoDiff,
            FoundFirstDiff,
            FoundBothDiffs,
        }
    }
}