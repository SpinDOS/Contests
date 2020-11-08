using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace C.IndexFile
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var content = File.ReadAllBytes("index.bin");
            // var content = new byte[]
            // {
            //     0x03, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00,
            //     0x14, 0x00, 0x00, 0x00, 0x15, 0x00, 0x00, 0x00,
            //     0x00, 0x00, 0x00, 0x00, 0x0A, 0x09, 0x30, 0x30,
            //     0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30,
            // };
            var solution = new Solver(content).Solve();
            Console.WriteLine(FormatSolution(solution));
        }

        private static string FormatSolution(List<StringListInfo> solution)
        {
            var allResultStrings = new[] {solution.Count.ToString()}
                .Concat(solution.Select(it => $"{it.Index} {it.StringLengthSum}"));
            return string.Join(Environment.NewLine, allResultStrings);
        }
    }

    internal sealed class Solver
    {
        private readonly byte[] _content;
        private readonly bool[] _canBeString;
        private readonly bool[] _canBeStringList;

        public Solver(byte[] content)
        {
            _content = content;
            _canBeString = new bool[content.Length];
            _canBeStringList = new bool[content.Length];
        }

        public List<StringListInfo> Solve()
        {
            FillCanBeString();
            FillCanBeStringList();
            return CalculateStringLengthSum();
        }

        private void FillCanBeString()
        {
            var lengthOfGoodChars = 0;
            for (var i = _content.Length - 1; i >= 0; i--)
            {
                _canBeString[i] = _content[i] <= lengthOfGoodChars;
                if (_content[i] >= 9 && _content[i] <= 127)
                    lengthOfGoodChars++;
                else
                    lengthOfGoodChars = 0;
            }
        }

        private void FillCanBeStringList()
        {
            var goodStringsTrackers = Enumerable.Range(0, 4)
                .Select(_ => new GoodStringsTracker())
                .ToArray();
            for (var index = _content.Length - 4; index >= 0; index--)
                _canBeStringList[index] = CheckCanBeStringList(ToInt32LittleEndian(index), goodStringsTrackers[index % 4]);
        }

        private bool CheckCanBeStringList(int integer, GoodStringsTracker goodStringsTracker)
        {
            var canBeStringList = integer > 0 && integer <= goodStringsTracker.FollowingStringAreGood;

            var stringOffset = integer;
            if (stringOffset >= 0 && stringOffset < _canBeString.Length && _canBeString[stringOffset])
                goodStringsTracker.FollowingStringAreGood++;
            else
                goodStringsTracker.FollowingStringAreGood = 0;
            return canBeStringList;
        }

        private List<StringListInfo> CalculateStringLengthSum()
        {
            var stringLengthSums = Enumerable.Range(0, 4)
                .Select(_ => new StringLengthsSum())
                .ToArray();

            for (var i = 0; i <= _content.Length - 4; i++)
            {
                var stringLengthSum = stringLengthSums[i % 4].StringLengthSum;
                var lastSum = stringLengthSum.LastOrDefault();
                var stringIndex = ToInt32LittleEndian(i);
                if (stringIndex >= 0 && stringIndex < _content.Length)
                    lastSum += _content[stringIndex];
                stringLengthSum.Add(lastSum);
            }
            
            var result = new List<StringListInfo>();
            for (var i = 0; i <= _content.Length - 4; i++)
            {
                if (!_canBeStringList[i])
                    continue;
                
                var stringLengthSum = stringLengthSums[i % 4].StringLengthSum;
                var indexOfCurList = i / 4;
                var stringListSize = ToInt32LittleEndian(i);
                var stringListSum = stringLengthSum[indexOfCurList + stringListSize] - stringLengthSum[indexOfCurList];
                result.Add(new StringListInfo() { Index = i, StringLengthSum = stringListSum});
            }

            return result;
        }

        private int ToInt32LittleEndian(int index)
        {
            return  (_content[index + 3] << 24) | 
                    (_content[index + 2] << 16) | 
                    (_content[index + 1] << 8) | 
                    (_content[index]);
        }

        private sealed class GoodStringsTracker
        {
            public int FollowingStringAreGood { get; set; }
        }

        private sealed class StringLengthsSum
        {
            public List<int> StringLengthSum { get; } = new List<int>();
        }
    }

    internal struct StringListInfo
    {
        public int Index { get; set; }
        public int StringLengthSum { get; set; }
    }
}