using System;
using System.Collections.Generic;
using System.Linq;

namespace D.ShortestCycle
{
    internal static class Program
    {
        public static void Main()
        {
            Console.ReadLine(); // n

            var a = Console.ReadLine().Split()
                .Select(ulong.Parse)
                .Where(it => it != 0)
                .ToArray();
            
            var cycleLength = Check3LengthCycle(a)? 3 : new Solver(a).GetShortestCycleLength();
            Console.WriteLine(cycleLength);
        }
        
        private static bool Check3LengthCycle(IEnumerable<ulong> items)
        {
            var powersOf2 = new byte[sizeof(ulong) * 8];
            
            foreach (var item in items)
            {
                var a = item;
                for (var powerOf2 = 0; a > 0; powerOf2++, a >>= 1)
                    if ((a & 1) != 0 && ++powersOf2[powerOf2] == 3)
                        return true;
            }

            return false;
        }
    }

    internal sealed class Solver
    {
        private readonly List<NodeId>[] _connectedTo;
        private readonly int[] _pathLength;
        private readonly Queue<NodeId> _cycleSearchQueue;
        
        public Solver(ulong[] graph)
        {
            _pathLength = new int[graph.Length];
            _cycleSearchQueue = new Queue<NodeId>(graph.Length);
            
            _connectedTo = new List<NodeId>[graph.Length];
            for (var i = 0; i < graph.Length; i++)
                _connectedTo[i] = new List<NodeId>();
            
            for (var i = 0; i < graph.Length - 1; i++)
                for (var j = i + 1; j < graph.Length; j++)
                    if ((graph[i] & graph[j]) != 0)
                    {
                        _connectedTo[i].Add(new NodeId(j));
                        _connectedTo[j].Add(new NodeId(i));
                    }
        }
        
        public int GetShortestCycleLength()
        {
            var shortestCycleLength = default(int?);
            
            for (var i = 0; i < _connectedTo.Length; i++)
            {
                var cycleLength = GetCycleLength(new NodeId(i));
                if (shortestCycleLength == null || cycleLength < shortestCycleLength)
                    shortestCycleLength = cycleLength;
            }

            return shortestCycleLength ?? -1;
        }

        private int? GetCycleLength(NodeId start)
        {
            Array.Clear(_pathLength, 0, _pathLength.Length);
            _pathLength[start.Value] = 1;

            _cycleSearchQueue.Clear();
            _cycleSearchQueue.Enqueue(start);
            
            for (var depthLevel = 1; _cycleSearchQueue.Count > 0; depthLevel++)
                if (!TryMakeDepthStep(depthLevel, out var cycleLength))
                    return cycleLength;

            return null;
        }

        private bool TryMakeDepthStep(int depthLevel, out int cycleLength)
        {
            var onCurrentLevel = _cycleSearchQueue.Count;
            var nextDepthLevel = depthLevel + 1;

            for (var i = 0; i < onCurrentLevel; i++)
            {
                var node = _cycleSearchQueue.Dequeue();
                foreach (var connectedNode in _connectedTo[node.Value])
                {
                    var depthOfConnected = _pathLength[connectedNode.Value];
                    if (depthOfConnected >= depthLevel)
                    {
                        cycleLength = depthOfConnected + depthLevel - 1;
                        return false;
                    }
                        
                    if (depthOfConnected > 0)
                        continue;
                        
                    _pathLength[connectedNode.Value] = nextDepthLevel;
                    _cycleSearchQueue.Enqueue(connectedNode);
                }
            }

            cycleLength = -1;
            return true;
        }
        
        private struct NodeId
        {
            public NodeId(int value) => Value = value;
            public int Value { get; }
        }
    }
}