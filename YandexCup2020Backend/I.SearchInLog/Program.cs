using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace I.SearchInLog
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var searchString = Console.ReadLine();
            var solver = new Solver(searchString);
            var n = int.Parse(Console.ReadLine());
            for (var i = 0; i < n; i++)
                AddLogBlock(solver, Console.ReadLine());
            var solution = solver.GetSolution();
            Console.WriteLine(solution.HasValue ? "YES" : "NO");
            if (solution.HasValue)
                Console.WriteLine(solution.Value);
        }

        private static void AddLogBlock(Solver solver, string blockInfo)
        {
            var parts = blockInfo.Split('\t');
            if (parts[0] == "0")
                solver.AddBlock0(parts[1]);
            else
                solver.AddBlock1(long.Parse(parts[1]), long.Parse(parts[2]));
        }
    }

    internal sealed class Solver
    {
        private const int MaxBlocks = 5000;
        
        private readonly string _searchString;
        private readonly int _n;
        
        private readonly List<ConnectingBlock> _connectingBlocks = new List<ConnectingBlock>(MaxBlocks);
        private readonly List<long> _connectingBlockFroms = new List<long>(MaxBlocks);
        private readonly List<OrdinalBlock> _ordinalBlocks = new List<OrdinalBlock>(MaxBlocks);
        private readonly List<long> _ordinalBlockFroms = new List<long>(MaxBlocks);

        private readonly int[] _bmSearchStep = new int[256];
        private long _nextIndex = 0;

        public Solver(string searchString)
        {
            _searchString = searchString;
            _n = _searchString.Length;
            
            Array.Fill(_bmSearchStep, -1);
            for (var i = 0; i < _n; i++)
                _bmSearchStep[_searchString[i]] = i;
        }

        public long? GetSolution()
        {
            foreach (var connectingBlock in _connectingBlocks)
            {
                var foundIndex = BMSearch(connectingBlock.Data.ToString());
                if (foundIndex.HasValue)
                    return connectingBlock.From + foundIndex.Value;
            }

            return null;
        }

        public void AddBlock0(string singleChar)
        {
            GetConnectingBlockToAppend().Data.Append(singleChar);
            ++_nextIndex;
        }

        public void AddBlock1(long from, long length)
        {
            var blockStartString = GetStringFromLog(from, Math.Min(_n, length));
            GetConnectingBlockToAppend().Data.Append(blockStartString);

            _ordinalBlocks.Add(new OrdinalBlock { From = _nextIndex, CopyFrom = from, });
            _ordinalBlockFroms.Add(_nextIndex);
            _nextIndex += length;
        }

        private ConnectingBlock GetConnectingBlockToAppend()
        {
            var connectingBlock = _connectingBlocks.LastOrDefault();
            var currentSuffixFrom = Math.Max(_nextIndex - _n, 0);
            if (connectingBlock != null && connectingBlock.To >= currentSuffixFrom)
            {
                connectingBlock.Data.Append(GetStringFromLog(connectingBlock.To, _nextIndex - connectingBlock.To));
                return connectingBlock;
            }

            connectingBlock = new ConnectingBlock() {From = currentSuffixFrom,};
            connectingBlock.Data.Append(GetStringFromLog(currentSuffixFrom, _nextIndex - currentSuffixFrom));
            _connectingBlocks.Add(connectingBlock);
            _connectingBlockFroms.Add(currentSuffixFrom);
            return connectingBlock;
        }

        private char[] GetStringFromLog(long from, long length)
        {
            if (length == 0)
                return Array.Empty<char>();
            var maxConnBlockIndex = _connectingBlocks.Count;
            var maxOrdinalBlockIndex = _ordinalBlocks.Count;
            while (true)
            {
                var resultFromConnectingBlocks = GetLogStringFromConnectingBlocks(from, length, ref maxConnBlockIndex);
                if (resultFromConnectingBlocks != null)
                    return resultFromConnectingBlocks;
                from = GetTargetBlockFrom(from, ref maxOrdinalBlockIndex);
            }
        }

        private char[] GetLogStringFromConnectingBlocks(long from, long length, ref int maxConnBlockIndex)
        {
            var index = _connectingBlockFroms.BinarySearch(0, maxConnBlockIndex, from, null);
            if (index < 0)
                index = ~index - 1;
            maxConnBlockIndex = index + 1;
            var connectingBlock = _connectingBlocks[index];
            if (connectingBlock.From <= from && connectingBlock.To >= from + length)
            {
                var result = new char[length];
                connectingBlock.Data.CopyTo((int)(from - connectingBlock.From), result, (int)length);
                return result;
            }
            return null;
        }

        private long GetTargetBlockFrom(long from, ref int maxOrdinalBlockIndex)
        {
            var index = _ordinalBlockFroms.BinarySearch(0, maxOrdinalBlockIndex, from, null);
            if (index < 0)
                index = ~index - 1;
            maxOrdinalBlockIndex = index + 1;
            var ordinalBlock = _ordinalBlocks[index];
            return ordinalBlock.CopyFrom + (from - ordinalBlock.From);
        }
        
        private int? BMSearch(string data)
        {
            var lastIndex = _n - 1;
            for (var i = 0; i < data.Length - lastIndex; )
            {
                int j;
                for (j = lastIndex; data[i + j] == _searchString[j]; j--)
                    if (j == 0)
                        return i;
                i += Math.Max(1, j - _bmSearchStep[data[i + j]]);
            }

            return null;
        }
        
        private abstract class LogBlockBase
        {
            public long From { get; set; }
        }

        private sealed class OrdinalBlock : LogBlockBase
        {
            public long CopyFrom { get; set; }
        }

        private sealed class ConnectingBlock : LogBlockBase
        {
            public StringBuilder Data { get; } = new StringBuilder();
            public long To => From + Data.Length;
        }
    }
}