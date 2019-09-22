using System;
using System.Collections.Generic;
using System.Linq;

namespace F.AlmostAll
{
    internal static class Program
    {
        public static void Main()
        {
            var adjacencyList = ReadInput();
            
            var solver = new Solver(adjacencyList);
            solver.Solve();

            foreach (var edge in solver.Edges)
                Console.WriteLine($"{edge.From} {edge.To} {edge.Num}");
        }

        private static List<Node>[] ReadInput()
        {
            var n = int.Parse(Console.ReadLine());
            var adjacencyList = new List<Node>[n];
            for (var i = 0; i < n; i++)
                adjacencyList[i] = new List<Node>();
            
            for (var i = 1; i < n; i++)
            {
                var uv = Console.ReadLine().Split();
                var u = int.Parse(uv[0]) - 1;
                var v = int.Parse(uv[1]) - 1;
                
                adjacencyList[u].Add(new Node(v));
                adjacencyList[v].Add(new Node(u));
            }

            return adjacencyList;
        }
    }

    internal sealed class Solver
    {
        private readonly List<Node>[] _adjacencyList;
        private readonly List<Edge> _resultEdges;
        private readonly int _centroidLimit;
        
        private Node? _centroid;

        public Solver(List<Node>[] adjacencyList)
        {
            _adjacencyList = adjacencyList;
            _resultEdges = new List<Edge>(N - 1);
            
            _centroidLimit = N / 2;
        }

        public IReadOnlyCollection<Edge> Edges => _resultEdges;

        private int N => _adjacencyList.Length;

        private Node Centroid => _centroid.Value;
        
        public void Solve()
        {
            FindCentroidDfs(new Node(0), new Node(-1));
            var (groupA, groupB) = SplitRootChildrenToGroups();

            var groupANums = Enumerable.Range(1, groupA.TotalSize);
            FillGroupEdges(groupA.DirectChildren, groupANums);
            
            var groupBNums = Enumerable.Range(1, groupB.TotalSize)
                .Select(it => it * (groupA.TotalSize + 1));
            FillGroupEdges(groupB.DirectChildren, groupBNums);
        }

        private int FindCentroidDfs(Node node, Node prev)
        {
            var isCentroid = true;
            
            var subTreeSize = 1;
            foreach (var next in GetAdjacent(node).Where(it => it != prev))
            {
                var nextSubTree = FindCentroidDfs(next, node);
                if (_centroid.HasValue)
                    return 0;
                
                isCentroid = isCentroid && nextSubTree <= _centroidLimit;
                subTreeSize += nextSubTree;
            }

            if (isCentroid && N - subTreeSize <= _centroidLimit)
                _centroid = node;
            
            return subTreeSize;
        }

        private (RootChildGroup, RootChildGroup) SplitRootChildrenToGroups()
        {
            RootChildGroup
                groupA = new RootChildGroup() {DirectChildren = new List<Node>()},
                groupB = new RootChildGroup() {DirectChildren = new List<Node>()};
            
            var groupLimit = (N + 1) / 3; // ceiling ((N - 1) / 3)

            var rootChildren = GetAdjacent(Centroid)
                .Select(it => new {Node = it, Size = GetSubtreeSize(it, Centroid)})
                .OrderBy(it => it.Size);
            
            foreach (var child in rootChildren)
            {
                if (groupA.TotalSize < groupLimit)
                    groupA.Add(child.Node, child.Size);
                else
                    groupB.Add(child.Node, child.Size);
            }

            return (groupA, groupB);
        }

        private int GetSubtreeSize(Node node, Node parent)
        {
            return 1 + GetAdjacent(node)
                       .Where(it => it != parent)
                       .Select(it => GetSubtreeSize(it, node))
                       .Sum();
        }

        private void FillGroupEdges(List<Node> group, IEnumerable<int> nums)
        {
            using (var num = nums.GetEnumerator())
                foreach (var node in group)
                    FillGroupEdges(node, Centroid, 0, num);
        }

        private void FillGroupEdges(Node node, Node parent, int parentSum, IEnumerator<int> numEnumerator)
        {
            numEnumerator.MoveNext();
            var curSum = numEnumerator.Current;
            _resultEdges.Add(new Edge(node, parent, curSum - parentSum));
            
            foreach (var child in GetAdjacent(node).Where(it => it != parent))
                FillGroupEdges(child, node, curSum, numEnumerator);
        }

        private List<Node> GetAdjacent(Node node) => _adjacencyList[node.Id];

        private struct RootChildGroup
        {
            public List<Node> DirectChildren { get; set; }
            public int TotalSize { get; private set; }

            public void Add(Node child, int size)
            {
                DirectChildren.Add(child);
                TotalSize += size;
            }
        }
    }

    internal struct Node
    {
        public Node(int id) => Id = id;
        public int Id { get; }

        public override bool Equals(object obj) => obj is Node node && node.Id == Id;
        public override int GetHashCode() => Id;

        public static bool operator==(Node a, Node b) => a.Id == b.Id;
        public static bool operator!=(Node a, Node b) => a.Id != b.Id;
        
        public override string ToString() => (Id + 1).ToString();
    }

    internal struct Edge
    {
        public Edge(Node from, Node to, int num)
        {
            From = from;
            To = to;
            Num = num;
        }
        
        public Node From { get; }
        public Node To { get; }
        public int Num { get; }
    }
}