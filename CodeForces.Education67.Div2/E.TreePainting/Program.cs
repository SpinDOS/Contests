using System;
using System.Collections.Generic;
using System.Linq;

namespace E.TreePainting
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var nodes = ReadInput();
            CalculateIfBlackComesTo(nodes);
            Console.WriteLine(FindSolution(nodes));
        }

        private static TreeNode[] ReadInput()
        {
            var nodes = new TreeNode[int.Parse(Console.ReadLine())];
            for (var i = 0; i < nodes.Length; i++)
                nodes[i] = new TreeNode(i + 1);

            for (var i = 0; i < nodes.Length - 1; i++)
            {
                var uv = Console.ReadLine().Split();
                var u = nodes[int.Parse(uv[0]) - 1];
                var v = nodes[int.Parse(uv[1]) - 1];
                
                u.IfBlackComesTo.Add(v, default(NodesCountInfo));
                v.IfBlackComesTo.Add(u, default(NodesCountInfo));
            }

            return nodes;
        }

        private static void CalculateIfBlackComesTo(TreeNode[] nodes)
        {
            var queue = new Queue<TreeNode>(nodes.Length);
            InitLeaves(nodes, queue);

            var fullyVisitedQueue = new Queue<TreeNode>(nodes.Length);
            MoveForwardToRoot(queue, fullyVisitedQueue);
            MoveBackToLeaves(fullyVisitedQueue, nodes.Length);
        }

        private static long FindSolution(TreeNode[] nodes)
        {
            return nodes
                       .Where(it => it.IfBlackComesTo.Count == 1)
                       .Max(it => it.IfBlackComesTo.Values.Single().TotalCount)
                   + nodes.Length;
        }

        private static void InitLeaves(TreeNode[] nodes, Queue<TreeNode> queue)
        {
            foreach (var node in nodes.Where(it => it.IfBlackComesTo.Count == 1))
                queue.Enqueue(node);
        }

        private static void MoveForwardToRoot(Queue<TreeNode> queue, Queue<TreeNode> fullyVisitedQueue)
        {
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node.VisitedChildCount == node.IfBlackComesTo.Count)
                {
                    fullyVisitedQueue.Enqueue(node);
                    continue;
                }

                var parent = ExploreNeighbours(node, out var nodesCountSum, out var totalCountSum);
                parent.IfBlackComesTo[node] = new NodesCountInfo(nodesCountSum, totalCountSum + nodesCountSum);
                
                if (++parent.VisitedChildCount == parent.IfBlackComesTo.Count - 1)
                    queue.Enqueue(parent);
            }
        }

        private static void MoveBackToLeaves(Queue<TreeNode> queue, int totalNodesSum)
        {
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                var totalCountSum = node.IfBlackComesTo.Values.Sum(it => it.TotalCount);

                foreach (var neighbourKeyValue in node.IfBlackComesTo)
                {
                    var neighbour = neighbourKeyValue.Key;
                    if (neighbour.IfBlackComesTo[node].NodesCount != 0)
                        continue;

                    var nodesCount = totalNodesSum - neighbourKeyValue.Value.NodesCount;
                    var totalCount = totalCountSum - neighbourKeyValue.Value.TotalCount;
                    neighbour.IfBlackComesTo[node] = new NodesCountInfo(nodesCount, totalCount + nodesCount);
                    
                    queue.Enqueue(neighbour);
                }
            }
        }

        private static TreeNode ExploreNeighbours(TreeNode node, out int nodesCountSum, out long totalCountSum)
        {
            TreeNode parent = null;
            nodesCountSum = 1;
            totalCountSum = 0L;
                
            foreach (var neighbour in node.IfBlackComesTo)
            {
                var nodesCountInfo = neighbour.Value;
                if (nodesCountInfo.NodesCount == 0)
                    parent = neighbour.Key;
                else
                {
                    nodesCountSum += nodesCountInfo.NodesCount;
                    totalCountSum += nodesCountInfo.TotalCount;
                }
            }
            
            return parent;
        }
    }

    internal sealed class TreeNode : IEquatable<TreeNode>
    {
        public TreeNode(int id) { Id = id; }
        
        public int Id { get; }
        
        public Dictionary<TreeNode, NodesCountInfo> IfBlackComesTo { get; } = new Dictionary<TreeNode, NodesCountInfo>();
        
        public int VisitedChildCount { get; set; }

        public bool Equals(TreeNode other) => other != null && Id == other.Id;

        public override bool Equals(object obj) => obj is TreeNode treeNode && treeNode.Id == Id;

        public override int GetHashCode() => Id;

        public override string ToString() => Id.ToString();
    }

    internal struct NodesCountInfo
    {
        public NodesCountInfo(int nodesCount, long totalCount)
        {
            NodesCount = nodesCount;
            TotalCount = totalCount;
        }
        
        public int NodesCount { get; }
        public long TotalCount { get; }

        public override string ToString() => $"{NodesCount}, {TotalCount}";
    }
}