using System;
using System.Collections.Generic;

namespace G.GangUp
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var nmkcd = Console.ReadLine().Split();
            var n = int.Parse(nmkcd[0]);
            var m = int.Parse(nmkcd[1]);
            var k = int.Parse(nmkcd[2]);
            var c = int.Parse(nmkcd[3]);
            var d = int.Parse(nmkcd[4]);
            
            var solver = new Solver(n, m, k, c, d);
            solver.Init();
            
            ReadInput(solver, n, m);
            
            for (var i = 0; i < k; i++)
                solver.IncreaseFlow();

            Console.WriteLine(solver.TotalCost);
        }

        private static void ReadInput(Solver solver, int n, int m)
        {
            var memberCounts = new int[n];
            foreach (var memberHome in Console.ReadLine().Split())
                memberCounts[int.Parse(memberHome) - 1]++;

            for (var crossroad = 0; crossroad < n; crossroad++)
                solver.AddMemberHome(crossroad, memberCounts[crossroad]);
            
            for (var i = 0; i < m; i++)
            {
                var xy = Console.ReadLine().Split();
                var x = int.Parse(xy[0]) - 1;
                var y = int.Parse(xy[1]) - 1;
                
                solver.AddEdge(x, y);
                solver.AddEdge(y, x);
            }
        }
    }

    internal sealed class Solver
    {
        private static readonly Edge FakeEdge = new Edge();
        
        private readonly int _n, _m, _k, _c, _d;
        private readonly int _maxTime;
        private readonly Node _start, _sink;
        private readonly Node[,] _nodes;
        private readonly List<Edge>[] _nodeEdges;
        private readonly BestPathSearcher _bestPathSearcher;

        public Solver(int n, int m, int k, int c, int d)
        {
            _n = n;
            _m = m;
            _k = k;
            _c = c;
            _d = d;

            _maxTime = k + m;
            _nodes = new Node[n, _maxTime];

            _start = new Node() {Id = _nodes.Length};
            _sink = new Node() {Id = _nodes.Length + 1};

            var allNodesCount = _nodes.Length + 2;
            
            _nodeEdges = new List<Edge>[allNodesCount];
            for (var i = 0; i < allNodesCount; i++)
                _nodeEdges[i] = new List<Edge>(_maxTime);
            
            _bestPathSearcher = new BestPathSearcher(_nodeEdges);
        }
        
        public int TotalCost { get; private set; }

        public void Init()
        {
            var nodeId = 0;
            for (var node = 0; node < _n; node++)
                for (var time = 0; time < _maxTime; time++)
                    _nodes[node, time] = new Node() { Id = nodeId++ };

            for (var time = 0; time < _maxTime; time++)
                AddEdgeWithoutPair(_nodes[0, time], _sink, time * _c, _k);

            for (var node = 0; node < _n; node++)
            {
                var from = _nodes[node, 0];
                for (var time = 1; time < _maxTime; time++)
                {
                    var to = _nodes[node, time];
                    AddEdgePair(from, to, 0, _k);
                    from = to;
                }
            }
        }

        public void AddMemberHome(int home, int count)
        {
            if (count > 0)
                AddEdgeWithoutPair(_start, _nodes[home, 0], 0, count);
        }

        public void AddEdge(int from, int to)
        {
            for (var time = 0; time < _maxTime - 1; time++)
            {
                var nodeFrom = _nodes[from, time];
                var nodeTo = _nodes[to, time + 1];
                
                for (int people = 0, cost = 1; people < _k; people++, cost += 2)
                    AddEdgePair(nodeFrom, nodeTo, cost * _d, 1);
            }
        }

        public void IncreaseFlow()
        {
            var bestPath = _bestPathSearcher.FindBestPath(_start, _sink);
            foreach (var edge in bestPath)
            {
                edge.ResidualCapacity--;
                edge.PairedEdge.ResidualCapacity++;
                TotalCost += edge.Cost;
            }
        }

        private void AddEdgePair(Node from, Node to, int cost, int capacity)
        {
            var forwardEdge = new Edge()
            {
                From = from,
                To = to,
                Cost = cost,
                ResidualCapacity = capacity,
            };

            var reverseEdge = new Edge()
            {
                From = to,
                To = from,
                Cost = -cost,
                ResidualCapacity = 0,
            };

            forwardEdge.PairedEdge = reverseEdge;
            reverseEdge.PairedEdge = forwardEdge;
            
            _nodeEdges[from.Id].Add(forwardEdge);
            _nodeEdges[to.Id].Add(reverseEdge);
        }

        private void AddEdgeWithoutPair(Node from, Node to, int cost, int capacity)
        {
            _nodeEdges[from.Id].Add(new Edge()
            {
                From = from,
                To = to,
                Cost = cost,
                ResidualCapacity = capacity, 
                PairedEdge = FakeEdge,
            });
        }
    }

    internal sealed class BestPathSearcher
    {
        private readonly List<Edge>[] _nodeEdges;

        private readonly int[] _bestCosts;
        private readonly Edge[] _bestMoves;
        private readonly bool[] _isInQueue;
        private readonly Queue<Node> _bestPathSearchQueue;

        public BestPathSearcher(List<Edge>[] nodeEdges)
        {
            _nodeEdges = nodeEdges;
            _bestCosts = new int[nodeEdges.Length];
            _bestMoves = new Edge[nodeEdges.Length];
            _isInQueue = new bool[nodeEdges.Length];
            _bestPathSearchQueue = new Queue<Node>(nodeEdges.Length);
        }

        public IEnumerable<Edge> FindBestPath(Node start, Node to)
        {
            for (var i = 0; i < _bestCosts.Length; i++)
                _bestCosts[i] = int.MaxValue;

            _bestCosts[start.Id] = 0;
            _bestPathSearchQueue.Enqueue(start);
            
            while (_bestPathSearchQueue.Count > 0)
            {
                var node = _bestPathSearchQueue.Dequeue();
                var costToNode = _bestCosts[node.Id];
                
                foreach (var edge in _nodeEdges[node.Id])
                {
                    if (edge.ResidualCapacity == 0)
                        continue;
 
                    var possibleCost = costToNode + edge.Cost;
                    if (possibleCost >= _bestCosts[edge.To.Id])
                        continue;
 
                    _bestCosts[edge.To.Id] = possibleCost;
                    _bestMoves[edge.To.Id] = edge;
                    if (_isInQueue[edge.To.Id])
                        continue;
                    
                    _isInQueue[edge.To.Id] = true;
                    _bestPathSearchQueue.Enqueue(edge.To);
                }
 
                _isInQueue[node.Id] = false;
            }

            var cur = _bestMoves[to.Id];
            while (cur != null)
            {
                yield return cur;
                cur = _bestMoves[cur.From.Id];
            }
        }
    }

    internal struct Node
    {
        public int Id { get; set; }
        public override string ToString() => Id.ToString();
    }

    internal sealed class Edge
    {
        public Node From { get; set; }
        public Node To { get; set; }
        public int Cost { get; set; }
        public int ResidualCapacity { get; set; }
        public Edge PairedEdge { get; set; }
    }
}