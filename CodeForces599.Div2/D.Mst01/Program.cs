using System;
using System.Collections.Generic;
using System.Linq;
 
namespace D.Mst01
{
    internal static class Program
    {
        public static void Main()
        {
            var (n, edges) = ReadInput();
            Console.WriteLine(new Solver(n, edges).Solve());
        }
 
        private static (int n, Edge[] edges) ReadInput()
        {
            var nm = Console.ReadLine().Split();
            var n = int.Parse(nm[0]);
            var m = int.Parse(nm[1]);
 
            var edges = new Edge[m];
            
            for (var i = 0; i < m; i++)
            {
                var ab = Console.ReadLine().Split();
                var a = int.Parse(ab[0]) - 1;
                var b = int.Parse(ab[1]) - 1;
                edges[i] = new Edge(a, b);
            }
 
            return (n, edges);
        }
    }
 
    internal sealed class Solver
    {
        private readonly int _n;
        private readonly HashSet<int>[] _oneWeightEdges;
 
        public Solver(int n, Edge[] edges)
        {
            _n = n;
            
            _oneWeightEdges = new HashSet<int>[n];
            for (var i = 0; i < n; i++)
                _oneWeightEdges[i] = new HashSet<int>();
 
            foreach (var edge in edges)
            {
                _oneWeightEdges[edge.A].Add(edge.B);
                _oneWeightEdges[edge.B].Add(edge.A);
            }
        }
 
        public int Solve()
        {
            var totalCost = -1;
            
            var nonVisitedVertices = new LinkedList<int>(Enumerable.Range(0, _n));
            var visitQueue = new Queue<int>(_n);
            
            while (nonVisitedVertices.Any())
            {
                ++totalCost;
                visitQueue.Enqueue(nonVisitedVertices.First.Value);
                nonVisitedVertices.Remove(nonVisitedVertices.First);
 
                while (visitQueue.Any())
                {
                    var cantVisitForFree = _oneWeightEdges[visitQueue.Dequeue()];
                    for (var node = nonVisitedVertices.First; node != null; )
                    {
                        var curNode = node;
                        node = node.Next;
 
                        var vertex = curNode.Value;
                        if (cantVisitForFree.Contains(vertex))
                            continue;
                        
                        visitQueue.Enqueue(vertex);
                        nonVisitedVertices.Remove(curNode);
                    }
                }
            }
 
            return totalCost;
        }
    }
 
    internal struct Edge
    {
        public Edge(int a, int b)
        {
            A = a;
            B = b;
        }
        
        public int A { get; }
        public int B { get; }
    }
}