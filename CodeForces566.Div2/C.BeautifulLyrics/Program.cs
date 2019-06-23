using System;
using System.Collections.Generic;

namespace C.BeautifulLyrics
{
    internal class Program
    {
        private const int VowelCount = 5;
        
        public static void Main(string[] args)
        {
            int countOfSecondWords;
            var wordPairs = ReadAndSolve(out countOfSecondWords);

            var lyricsCount = Math.Min(countOfSecondWords, wordPairs.Count / 2);
            Console.WriteLine(lyricsCount);
            
            var secondWordsIndex = 0;
            var firstWordsIndex = wordPairs.Count - 1;
            
            while(lyricsCount-- > 0)
            {
                var firstWords = wordPairs[firstWordsIndex--];
                var lastWords = wordPairs[secondWordsIndex++];
                
                Console.Write(firstWords.FirstRow);
                Console.Write(' ');
                Console.WriteLine(lastWords.FirstRow);
                Console.Write(firstWords.SecondRow);
                Console.Write(' ');
                Console.WriteLine(lastWords.SecondRow);
            }
        }

        private static List<WordPair> ReadAndSolve(out int countOfSecondWords)
        {
            int vowelCount, lastVowelId;
            string[] unmatchedWords;
            
            var n = int.Parse(Console.ReadLine());
            var wordPairs = new List<WordPair>(n / 2);

            var wordsByVowelCount = new Dictionary<int, string[]>();

            for (var i = 0; i < n; i++)
            {
                var word = ReadWord(out vowelCount, out lastVowelId);
                if (!wordsByVowelCount.TryGetValue(vowelCount, out unmatchedWords))
                    wordsByVowelCount.Add(vowelCount, unmatchedWords = new string[VowelCount]);

                var unmatchedWord = unmatchedWords[lastVowelId];
                if (unmatchedWord == null)
                    unmatchedWords[lastVowelId] = word;
                else
                {
                    wordPairs.Add(new WordPair() { FirstRow = word, SecondRow = unmatchedWord });
                    unmatchedWords[lastVowelId] = null;
                }
            }

            countOfSecondWords = wordPairs.Count;

            foreach (var unmatchedWordsArr in wordsByVowelCount.Values)
            {
                string unmatchedWord = null;
                foreach (var word in unmatchedWordsArr)
                {
                    if (word == null)
                        continue;

                    if (unmatchedWord == null)
                        unmatchedWord = word;
                    else
                    {
                        wordPairs.Add(new WordPair() { FirstRow = unmatchedWord, SecondRow = word });
                        unmatchedWord = null;
                    }
                }
            }

            return wordPairs;
        }

        private static string ReadWord(out int vowelCount, out int lastVowelId)
        {
            vowelCount = 0;
            lastVowelId = -1;
            
            var str = Console.ReadLine();
            foreach (var ch in str)
            {
                var vowelId = GetVowelId(ch);
                if (vowelId < 0)
                    continue;

                vowelCount++;
                lastVowelId = vowelId;
            }

            return str;
        }

        private static int GetVowelId(char ch)
        {
            switch (ch)
            {
                case 'a':
                    return 0;
                case 'e':
                    return 1;
                case 'o':
                    return 2;
                case 'i':
                    return 3;
                case 'u':
                    return 4;
                default:
                    return -1;
            }
        }
    }

    internal struct WordPair
    {
        public string FirstRow { get; set; }
        public string SecondRow { get; set; }
    }
}