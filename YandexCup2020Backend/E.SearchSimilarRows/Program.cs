using System;
using System.Collections.Generic;
using System.Linq;

namespace E.SearchSimilarRows
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var nk = Console.ReadLine().Split();
            var n = int.Parse(nk[0]);
            var k = int.Parse(nk[1]);
            var s = Console.ReadLine().Split().Skip(1).Select(int.Parse).ToArray();
            
            var solver = new Solver(n, k, s);
            for (var i = 0; i < n; i++)
                solver.AddRow(Console.ReadLine().Split('\t'));
            Console.WriteLine(solver.Solve());
        }
    }

    internal sealed class Solver
    {
        private readonly int _n;
        private readonly int _k;
        private readonly int[] _s;
        private readonly int _bitLimit;
        private readonly int _numLimit;
        private readonly int _importantMask;
        private readonly List<Row> _rows;

        public Solver(int n, int k, int[] s)
        {
            _n = n;
            _k = k;
            _s = s;

            _bitLimit = _k + 1;
            _numLimit = 1 << _bitLimit;
            _importantMask = s.Select(it => 1 << (it - 1)).Aggregate((x, y) => x | y);
            _rows = new List<Row>(_n);
        }

        public void AddRow(string[] rowValues)
        {
            _rows.Add(new Row() { Values = rowValues, Mask = CalculateMask(rowValues) });
        }

        public long Solve()
        {
            var result = 0L;
            for (var submask = 1; submask < _numLimit; submask++)
            {
                var keyToOriginalMask = FillKeyToOriginalMaskDict(submask);
                foreach (var originalMaskToRowCount in keyToOriginalMask.Values)
                    result += CalculateMatchingPairs(submask, originalMaskToRowCount);
            }
            
            return result;
        }

        private Dictionary<Row, Dictionary<int, int>> FillKeyToOriginalMaskDict(int submask)
        {
            var comparer = new RowSubMaskEqualityComparer(GetActiveIndices(submask));
            var result = new Dictionary<Row, Dictionary<int, int>>(_n, comparer);
            foreach (var row in _rows)
            {
                if (!IsSubmask(row.Mask, submask))
                    continue;
                if (!result.TryGetValue(row, out var originalMaskToRowCount))
                    result[row] = originalMaskToRowCount = new Dictionary<int, int>();
                originalMaskToRowCount.TryGetValue(row.Mask, out var rowCount);
                originalMaskToRowCount[row.Mask] = rowCount + 1;
            }

            return result;
        }

        private long CalculateMatchingPairs(int submask, Dictionary<int, int> originalMaskToRowCount)
        {
            var result = 0L;
            if (HasImportantColumn(submask) && originalMaskToRowCount.TryGetValue(submask, out var selfMatchingRowCount))
                result += GetMatchingPairs(selfMatchingRowCount);
            foreach (var originalMaskCount1 in originalMaskToRowCount)
            foreach (var originalMaskCount2 in originalMaskToRowCount)
            {
                if (originalMaskCount1.Key >= originalMaskCount2.Key)
                    continue;
                var hasImportantColumn = HasImportantColumn(originalMaskCount1.Key) ||
                                         HasImportantColumn(originalMaskCount2.Key);
                if (hasImportantColumn && (originalMaskCount1.Key & originalMaskCount2.Key) == submask)
                    result += originalMaskCount1.Value * originalMaskCount2.Value;
            }

            return result;
        }

        private int CalculateMask(string[] values)
        {
            var result = 0;
            var curBit = 1;
                
            for (var i = 0; i < _k; i++, curBit <<= 1)
                if (!string.IsNullOrEmpty(values[i]))
                    result |= curBit;
            return result;
        }

        private bool HasImportantColumn(int mask) => (mask & _importantMask) != 0;

        private int[] GetActiveIndices(int mask)
        {
            return Enumerable.Range(0, _bitLimit)
                .Where(it => (mask & (1 << it)) != 0)
                .ToArray();
        }

        private static long GetMatchingPairs(long matchingRows) => matchingRows * (matchingRows - 1) / 2;
        
        private static bool IsSubmask(int mask, int submask) => (mask & submask) == submask;

        private struct Row
        {
            public string[] Values { get; set; }
            public int Mask { get; set; }
        }

        private sealed class RowSubMaskEqualityComparer : IEqualityComparer<Row>
        {
            private readonly int[] _activeIndices;

            public RowSubMaskEqualityComparer(int[] activeIndices) => _activeIndices = activeIndices;

            public bool Equals(Row x, Row y)
            {
                foreach (var index in _activeIndices)
                    if (!string.Equals(x.Values[index], y.Values[index], StringComparison.InvariantCulture))
                        return false;
                return true;
            }

            public int GetHashCode(Row obj)
            {
                var result = 0;
                foreach (var index in _activeIndices)
                    result = HashCode.Combine(result, obj.Values[index]);
                return result;
            }
        }
    }
}