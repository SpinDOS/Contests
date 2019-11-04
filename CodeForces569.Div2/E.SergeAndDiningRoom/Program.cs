using System;
using System.Linq;

namespace E.SergeAndDiningRoom
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var (dishPrices, pupilMonies, q) = ReadInput();
            var solver = new Solver(dishPrices, pupilMonies);
            
            for (var i = 0; i < q; i++)
            {
                var query = ReadQuery();
                
                if (query.queryType == QueryType.ChangeDishPrice)
                    solver.ChangeDishPrice(query.index, query.newValue);
                else if (query.queryType == QueryType.ChangePupilMoney)
                    solver.ChangePupilMoney(query.index, query.newValue);

                Console.WriteLine(solver.GetMostExpensiveDish() ?? -1L);
            }
        }

        private static (uint[] dishPrices, uint[] pupilMonies, uint q) ReadInput()
        {
            Console.ReadLine(); // n m

            var dishPrices = Console.ReadLine().Split().Select(uint.Parse).ToArray();
            var pupilMonies = Console.ReadLine().Split().Select(uint.Parse).ToArray();
            var q = uint.Parse(Console.ReadLine());
            
            return (dishPrices, pupilMonies, q);
        }

        private static (QueryType queryType, uint index, uint newValue) ReadQuery()
        {
            var parts = Console.ReadLine().Split();
            return (
                (QueryType) byte.Parse(parts[0]),
                uint.Parse(parts[1]) - 1,
                uint.Parse(parts[2])
            );
        }
        
        private enum QueryType : byte
        {
            ChangeDishPrice = 1,
            ChangePupilMoney = 2,
        }
    }

    internal sealed class Solver
    {
        private readonly uint[] _dishPrices, _pupilMonies;
        private readonly SegmentTree _segmentTree;

        public Solver(uint[] dishPrices, uint[] pupilMonies)
        {
            _dishPrices = dishPrices;
            _pupilMonies = pupilMonies;
            _segmentTree = new SegmentTree();

            foreach (var dishPrice in dishPrices)
                _segmentTree.Update(dishPrice, +1);

            foreach (var pupilMoney in pupilMonies)
                _segmentTree.Update(pupilMoney, -1);
        }

        public void ChangeDishPrice(uint index, uint newValue)
        {
            _segmentTree.Update(_dishPrices[index], -1);
            _dishPrices[index] = newValue;
            _segmentTree.Update(_dishPrices[index], +1);
        }
        
        public void ChangePupilMoney(uint index, uint newValue) 
        {
            _segmentTree.Update(_pupilMonies[index], +1);
            _pupilMonies[index] = newValue;
            _segmentTree.Update(_pupilMonies[index], -1);
        }

        public uint? GetMostExpensiveDish() => _segmentTree.Get();
    }

    internal sealed class SegmentTree
    {
        private const uint MaxValue = 1 << 20;

        private readonly int[]
            _mainTree = new int[MaxValue << 1],
            _lazyChangeTree = new int[MaxValue << 1];

        public void Update(uint price, int change) => UpdateSegment(1, (0, MaxValue), price, change);

        public uint? Get() => Get(1, (0, MaxValue));

        private void UpdateSegment(uint treeVertexIndex, Segment segment, uint price, int change)
        {
            if (price < segment.Start)
                return;
            
            if (price >= segment.End || segment.IsSinglePoint)
            {
                ApplyChangeWithLazy(treeVertexIndex, change);
                return;
            }
            
            PropogateLazy(treeVertexIndex);
            
            var nextLevel = treeVertexIndex + treeVertexIndex;
            UpdateSegment(nextLevel, (segment.Start, segment.Mid), price, change);
            UpdateSegment(nextLevel + 1, (segment.Mid, segment.End), price, change);
            
            _mainTree[treeVertexIndex] = Math.Max(_mainTree[nextLevel], _mainTree[nextLevel + 1]);
        }

        private uint? Get(uint treeVertexIndex, Segment segment)
        {
            if (_mainTree[treeVertexIndex] <= 0)
                return null;

            if (segment.IsSinglePoint)
                return segment.Start;

            PropogateLazy(treeVertexIndex);
            var nextLevelVertexIndex = treeVertexIndex + treeVertexIndex;
            
            return Get(nextLevelVertexIndex + 1, (segment.Mid, segment.End)) ?? 
                   Get(nextLevelVertexIndex, (segment.Start, segment.Mid));
        }

        private void ApplyChangeWithLazy(uint treeVertexIndex, int change)
        {
            _mainTree[treeVertexIndex] += change;
            _lazyChangeTree[treeVertexIndex] += change;
        }

        private void PropogateLazy(uint treeVertexIndex)
        {
            var lazyChange = _lazyChangeTree[treeVertexIndex];
            if (lazyChange == 0)
                return;
            
            _lazyChangeTree[treeVertexIndex] = 0;

            var nextLevel = treeVertexIndex + treeVertexIndex;
            ApplyChangeWithLazy(nextLevel, lazyChange);
            ApplyChangeWithLazy(nextLevel + 1, lazyChange);
        }
    }

    /// <summary>
    /// Segment [Start, End)
    /// </summary>
    internal struct Segment
    {
        public Segment(uint start, uint end)
        {
            Start = start;
            End = end;
        }
        
        public uint Start { get; }
        public uint End { get; }
        
        public uint Mid => (Start + End) / 2;
        
        public bool IsSinglePoint => Start + 1 == End;

        public static implicit operator Segment((uint start, uint end) startEnd) => 
            new Segment(startEnd.start, startEnd.end);
    }
}