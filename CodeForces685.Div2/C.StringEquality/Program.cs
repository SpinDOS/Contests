using System;

namespace C.StringEquality
{
    internal sealed class Program
    {
        const int AlphabetSize = (int)'z' - (int)'a' + 1;
        
        public static void Main(string[] args)
        {
            var t = int.Parse(Console.ReadLine());
            for (var i = 0; i < t; i++)
                TestCase();
        }

        private static void TestCase()
        {
            var k = int.Parse(Console.ReadLine().Split()[1]);
            var a = Console.ReadLine();
            var b = Console.ReadLine();
            Console.WriteLine(Solve(k, a, b) ? "yes" : "no");
        }

        private static bool Solve(int k, string a, string b)
        {
            var aChars = BuildCharsCount(a);
            var bChars = BuildCharsCount(b);

            for (var i = 0; i < AlphabetSize; i++)
            {
                var transformedCharsCount = aChars[i] - bChars[i];
                if (transformedCharsCount < 0 || transformedCharsCount % k != 0)
                    return false;
                if (i < AlphabetSize - 1)
                    aChars[i + 1] += transformedCharsCount;
            }

            return true;
        }

        private static int[] BuildCharsCount(string str)
        {
            var charsCounts = new int[AlphabetSize];
            foreach (var ch in str)
                charsCounts[ch - 'a']++;
            return charsCounts;
        }
    }
}