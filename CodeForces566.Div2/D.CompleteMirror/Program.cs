using System;
using System.Collections.Generic;
using System.Linq;

namespace D.CompleteMirror
{
    internal static class Program
    {
        public static void Main()
        {
            var nodes = ReadInput();
            var root = new Solver().FindRoot(nodes);
            Console.WriteLine(root?.Id ?? -1);
        }
        
        private static TreeNode[] ReadInput()
        {
            var n = int.Parse(Console.ReadLine());
            
            var nodes = new TreeNode[n + 1];
            for (var i = 0; i <= n; i++)
                nodes[i] = new TreeNode() { Id = i, };

            for (var i = 0; i < n - 1; i++)
            {
                var ab = Console.ReadLine().Split();
                var a = nodes[int.Parse(ab[0])];
                var b = nodes[int.Parse(ab[1])];

                a.ConnectedTo.AddLast(b);
                b.ConnectedTo.AddLast(a);
            }

            return nodes;
        }
    }

    internal sealed class Solver
    {
        private Queue<TreeNodeWithLevel> _isRootQueue;
        private Queue<TreeNode> _nodesToExplore;
        private List<int> _degreesBuffer;
        private int _visitVersion;

        public TreeNode FindRoot(TreeNode[] treeNodes)
        {
            if (treeNodes.Length == 2)
                return treeNodes[1];

            Init(treeNodes);
            
            var ok = true;
            while (ok && _nodesToExplore.Count > 1)
                ok = CheckCurrentGeneration();

            return ok ? _nodesToExplore.Single() : _nodesToExplore.FirstOrDefault(IsRoot);

        }

        private void Init(TreeNode[] treeNodes)
        {
            _nodesToExplore = new Queue<TreeNode>(treeNodes.Length / 2);
            _isRootQueue = new Queue<TreeNodeWithLevel>(treeNodes.Length / 2);
            _degreesBuffer = new List<int>(treeNodes.Length);

            var version = ++_visitVersion;

            foreach (var treeNode in treeNodes.Where(it => it.Degree == 1))
            {
                _nodesToExplore.Enqueue(treeNode);
                treeNode.LeafsCount = 1;
                treeNode.VisitVersion = version;
                treeNode.Leaf = treeNode;
            }
        }

        private bool CheckCurrentGeneration()
        {
            var currentLevelVersion = _visitVersion++;
            var count = _nodesToExplore.Count;

            var firstNode = _nodesToExplore.Peek();
            var degree = firstNode.Degree;
            var leafs = firstNode.LeafsCount;

            for (var i = 0; i < count; i++)
            {
                var current = _nodesToExplore.Dequeue();
                if (current.Degree != degree || current.LeafsCount != leafs)
                    return InitErrors(firstNode, current);

                var parent = TryFindSingleParent(current, currentLevelVersion);
                if (parent == null || parent.VisitVersion == currentLevelVersion)
                    return InitErrors(current, parent);

                parent.LeafsCount += current.LeafsCount;
                if (parent.VisitVersion != _visitVersion)
                {
                    parent.VisitVersion = _visitVersion;
                    parent.Leaf = current.Leaf;
                    _nodesToExplore.Enqueue(parent);
                }
            }

            return true;
        }

        private TreeNode TryFindSingleParent(TreeNode current, int currentLevelVersion)
        {
            TreeNode parent = null;
            foreach (var node in current.ConnectedTo)
            {
                if (node.VisitVersion == 0 || node.VisitVersion >= currentLevelVersion)
                {
                    if (parent != null)
                        return null;
                    parent = node;
                }
            }

            return parent;
        }

        private bool InitErrors(TreeNode firstError, TreeNode secondError)
        {
            _nodesToExplore.Clear();

            if (firstError?.LeafsCount == 1)
                _nodesToExplore.Enqueue(firstError.Leaf);
            if (secondError?.LeafsCount == 1)
                _nodesToExplore.Enqueue(secondError.Leaf);

            return false;
        }

        private bool IsRoot(TreeNode root)
        {
            root.VisitVersion = ++_visitVersion;

            _degreesBuffer.Clear();
            _isRootQueue.Clear();
            
            _isRootQueue.Enqueue(new TreeNodeWithLevel() {TreeNode = root, Level = 0});
            while (_isRootQueue.Count > 0)
            {
                var current = _isRootQueue.Dequeue();
                var level = current.Level;
                var currentNode = current.TreeNode;

                if (_degreesBuffer.Count == level)
                    _degreesBuffer.Add(currentNode.Degree);
                else if (currentNode.Degree != _degreesBuffer[level])
                    return false;

                ++level;
                foreach (var child in currentNode.ConnectedTo.Where(it => it.VisitVersion != _visitVersion))
                {
                    child.VisitVersion = _visitVersion;
                    _isRootQueue.Enqueue(new TreeNodeWithLevel() {TreeNode = child, Level = level,});
                }
            }

            return true;
        }

        private struct TreeNodeWithLevel
        {
            public TreeNode TreeNode { get; set; }
            public int Level { get; set; }
        }
    }

    internal sealed class TreeNode
    {
        public int Id { get; set; }
        
        public LinkedList<TreeNode> ConnectedTo { get; } = new LinkedList<TreeNode>();

        public int Degree => ConnectedTo.Count;
        
        public int VisitVersion { get; set; }
        
        public int LeafsCount { get; set; }
        
        public TreeNode Leaf { get; set; }
        
        public override string ToString() => Id.ToString();
    }
}