using System;
using System.Collections.Generic;
using System.Linq;

namespace E.TheCakeIsALie
{
    internal static class Program
    {
        public static void Main()
        {
            var t = int.Parse(Console.ReadLine());
            
            for (var i = 0; i < t; i++)
            {
                var (vertices, triangles) = ReadInput();
                Console.WriteLine(string.Join(" ", OrderVertices(triangles).Select(it => it + 1)));
                Console.WriteLine(string.Join(" ", OrderTriangles(vertices).Select(it => it + 1)));
            }
        }

        private static (Vertex[] vertices, Triangle[] triangles) ReadInput()
        {
            var n = int.Parse(Console.ReadLine());

            var vertices = Enumerable.Range(0, n).Select(it => new Vertex(it)).ToArray();
            var triangles = Enumerable.Range(0, n - 2).Select(it => new Triangle(it)).ToArray();

            foreach (var triangle in triangles)
                foreach (var vertexId in Console.ReadLine().Split())
                {
                    var vertex = vertices[int.Parse(vertexId) - 1];
                    
                    triangle.Vertices.Add(vertex);
                    vertex.Triangles.Add(triangle);
                }

            return (vertices, triangles);
        }

        private static LinkedList<int> OrderVertices(Triangle[] triangles)
        {
            var result = new LinkedList<int>();
            var usedTriangles = new bool[triangles.Length];
            var queue = new Queue<LinkedListNode<int>>();

            var edgesDict = new TrianglesEdgesDictionary(triangles.Length);
            foreach (var triangle in triangles)
                edgesDict.AddTriangle(triangle);

            var firstVertex = triangles.SelectMany(it => it.Vertices).First(it => it.Triangles.Count == 1);
            var firstTriangle = firstVertex.Triangles.Single();
            
            usedTriangles[firstTriangle.Id] = true;
            result.AddLast(firstVertex.Id);
            result.AddLast(firstTriangle.Vertices.First(it => it.Id != firstVertex.Id).Id);
            result.AddLast(firstTriangle.Vertices.Last(it => it.Id != firstVertex.Id).Id);
            
            queue.Enqueue(result.Last);

            while (result.Count < triangles.Length + 2)
            {
                var edgeEndNode = queue.Dequeue();
                var edge = new TriangleEdge() { A = edgeEndNode.Value, B = edgeEndNode.Previous.Value };
                
                foreach (var ownerTriangle in edgesDict[edge])
                {
                    if (usedTriangles[ownerTriangle.TriangleId])
                        continue;
                    usedTriangles[ownerTriangle.TriangleId] = true;
                    
                    var newNode = result.AddBefore(edgeEndNode, ownerTriangle.ThirdVertex);
                    queue.Enqueue(newNode);
                    queue.Enqueue(edgeEndNode);
                }
            }

            return result;
        }

        private static List<int> OrderTriangles(Vertex[] vertices)
        {
            var result = new List<int>(vertices.Length - 2);
            
            var usedTriangles = new bool[result.Capacity];
            var trianglesCount = vertices.Select(it => it.Triangles.Count).ToArray();

            var queue = new Queue<Vertex>(vertices.Where(it => trianglesCount[it.Id] == 1));

            while (result.Count < result.Capacity)
            {
                var vertex = queue.Dequeue();
                var triangle = vertex.Triangles.SingleOrDefault(it => !usedTriangles[it.Id]);
                if (triangle.Vertices == null)
                    continue;

                usedTriangles[triangle.Id] = true;
                result.Add(triangle.Id);

                foreach (var nextVertex in triangle.Vertices)
                    if (--trianglesCount[nextVertex.Id] == 1)
                        queue.Enqueue(nextVertex);
            }

            return result;
        }

        private struct TriangleEdge
        {
            public int A { get; set; }
            public int B { get; set; }
        }

        private struct EdgeOwnerTriangle
        {
            public int TriangleId { get; set; }
            public int ThirdVertex { get; set; }
        }

        private sealed class TrianglesEdgesDictionary : Dictionary<TriangleEdge, List<EdgeOwnerTriangle>>
        {
            public TrianglesEdgesDictionary(int trianglesCount) : base(trianglesCount * 3, new EdgesEqualityComparer()) { }

            public void AddTriangle(Triangle triangle)
            {
                AddEdgeWitOwnerByVertexIndexes(triangle, 0, 1, 2);
                AddEdgeWitOwnerByVertexIndexes(triangle, 1, 2, 0);
                AddEdgeWitOwnerByVertexIndexes(triangle, 2, 0, 1);
            }

            private void AddEdgeWitOwnerByVertexIndexes(Triangle triangle, int a, int b, int c)
            {
                var key = new TriangleEdge() { A = triangle.Vertices[a].Id, B = triangle.Vertices[b].Id };
                var value = new EdgeOwnerTriangle() { TriangleId = triangle.Id, ThirdVertex = triangle.Vertices[c].Id };
                
                if (!base.TryGetValue(key, out var list))
                    base[key] = list = new List<EdgeOwnerTriangle>(2);
                list.Add(value);
            }

            private sealed class EdgesEqualityComparer : IEqualityComparer<TriangleEdge>
            {
                public bool Equals(TriangleEdge x, TriangleEdge y) =>
                    (x.A == y.A && x.B == y.B) || (x.A == y.B && x.B == y.A);

                public int GetHashCode(TriangleEdge obj)
                {
                    var min = Math.Min(obj.A, obj.B);
                    var max = Math.Max(obj.A, obj.B);
                    return (min * 397) ^ max;
                }
            }
        }
    }

    internal struct Vertex
    {
        public Vertex(int id)
        {
            Id = id;
            Triangles = new List<Triangle>();
        }
        
        public int Id { get; }
        public List<Triangle> Triangles { get; }
    }

    internal struct Triangle
    {
        public Triangle(int id)
        {
            Id = id;
            Vertices = new List<Vertex>(3);
        }
        
        public int Id { get; }
        public List<Vertex> Vertices { get; }
    }
}