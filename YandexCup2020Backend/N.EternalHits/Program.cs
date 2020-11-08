using System;
using System.Linq;
using System.Text;

namespace N.EternalHits
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var mx = Console.ReadLine().Split();
            var m = int.Parse(mx[0]);
            var x = int.Parse(mx[1]);
            var solver = new Solver(x);

            var ans = new StringBuilder();
            for (var i = 0; i < m; i++)
            {
                solver.AddMonth(Console.ReadLine().Split().Skip(1).Select(int.Parse).ToArray());
                ans.AppendLine(solver.Hits.ToString());
            }
            Console.WriteLine(ans);
        }
    }

    class Solver
    {
        private readonly BalancedTree _avl = new BalancedTree();
        private readonly int _x;

        public Solver(int x)
        {
            _x = x;
        }
        
        public int Hits { get; private set; }

        public void AddMonth(int[] ratings)
        {
            var prevMonthHitsCount = (_avl.Count * _x + 99) / 100;
            var thisMonthHitsCount = ((_avl.Count + ratings.Length) * _x + 99) / 100;

            Array.Sort(ratings);
            foreach (var rating in ratings)
            {
                int thresholdRating;
                if (thisMonthHitsCount == 0)
                    thresholdRating = int.MinValue;
                else if (thisMonthHitsCount > _avl.Count)
                    thresholdRating = int.MaxValue;
                else
                    thresholdRating = _avl.GetByIndex(thisMonthHitsCount - 1);
                
                var isHit = rating < thresholdRating;
                if (isHit)
                    Hits++;
                _avl.Add(rating, isHit);
            }

            Hits += _avl.MarkAsHit(prevMonthHitsCount, thisMonthHitsCount);
        }
    }
    
    public sealed partial class BalancedTree
    {
        private static bool Balance(Node node)
        {
            var height = node.Height;
            if (Math.Abs(node.LeftHeight - node.RightHeight) <= 1)
            {
                node.Height = Math.Max(node.LeftHeight, node.RightHeight) + 1;
            }
            else if (node.RightHeight > node.LeftHeight)
            {
                if (node.Right.LeftHeight <= node.Right.RightHeight)
                    LeftSmallRotate(node);
                else
                    LeftBigRotate(node);
            }
            else
            {
                if (node.Left.RightHeight <= node.Left.LeftHeight)
                    RightSmallRotate(node);
                else
                    RightBigRotate(node);
            }
            return height != node.Height;
        }

        private static void LeftSmallRotate(Node node)
        {
            Node a = node,
                b = node.Right,
                L = node.Left,
                C = b.Left,
                R = b.Right;

            node.Left = b;
            node.Right = L;
            node.Left.Left = L;
            node.Left.Right = C;
            node.Right = R;
            SwapKeyValue(a, b);

            node.Left.Height = node.Left.RightHeight + 1;
            node.Height = node.Left.Height + 1;

            node.Left.NodesCount = node.Left.LeftNodesCount + node.Left.RightNodesCount + 1;
            node.NodesCount = node.LeftNodesCount + node.RightNodesCount + 1;
        }

        private static void LeftBigRotate(Node node)
        {
            Node a = node,
                b = node.Right,
                c = b.Left,
                L = node.Left,
                M = c.Left,
                N = c.Right,
                R = b.Right;

            node.Left = c;
            node.Left.Left = L;
            node.Left.Right = M;
            node.Right.Left = N;
            node.Right.Right = R;
            SwapKeyValue(a, c);

            node.Left.Height = node.Left.LeftHeight + 1;
            node.Right.Height = node.Right.RightHeight + 1;
            node.Height = node.Left.Height + 1;
            
            node.Left.NodesCount = node.Left.LeftNodesCount + node.Left.RightNodesCount + 1;
            node.Right.NodesCount = node.Right.LeftNodesCount + node.Right.RightNodesCount + 1;
            node.NodesCount = node.LeftNodesCount + node.RightNodesCount + 1;
        }

        private static void RightSmallRotate(Node node)
        {
            Node a = node,
                b = node.Left,
                L = b.Left,
                C = b.Right,
                R = node.Right;

            node.Left = R;
            node.Right = b;
            node.Left = L;
            node.Right.Left = C;
            node.Right.Right = R;
            SwapKeyValue(a, b);

            node.Right.Height = node.Right.LeftHeight + 1;
            node.Height = node.Right.Height + 1;
            
            node.Right.NodesCount = node.Right.LeftNodesCount + node.Right.RightNodesCount + 1;
            node.NodesCount = node.LeftNodesCount + node.RightNodesCount + 1;
        }

        private static void RightBigRotate(Node node)
        {
            Node a = node,
                b = node.Left,
                c = b.Right,
                L = b.Left,
                M = c.Left,
                N = c.Right,
                R = node.Right;

            node.Right = c;
            node.Left.Left = L;
            node.Left.Right = M;
            node.Right.Left = N;
            node.Right.Right = R;
            SwapKeyValue(a, c);

            node.Left.Height = node.Left.LeftHeight + 1;
            node.Right.Height = node.Right.RightHeight + 1;
            node.Height = node.Right.Height + 1;
            
            node.Left.NodesCount = node.Left.LeftNodesCount + node.Left.RightNodesCount + 1;
            node.Right.NodesCount = node.Right.LeftNodesCount + node.Right.RightNodesCount + 1;
            node.NodesCount = node.LeftNodesCount + node.RightNodesCount + 1;
        }

        private static void SwapKeyValue(Node node1, Node node2)
        {
            var key = node1.Key;
            node1.Key = node2.Key;
            node2.Key = key;

            var isHit = node1.IsHit;
            node1.IsHit = node2.IsHit;
            node2.IsHit = isHit;
        }
    }
    public sealed partial class BalancedTree
    {
        private Node Root { get; set; }

        public int Count => Root?.NodesCount ?? 0;

        public void Add(int key, bool isHit)
        {
            if (Root == null)
                Root = new Node(key, isHit);
            else
                Add(Root, new Node(key, isHit));
        }
        
        public int GetByIndex(int index) => GetByIndex(index, Root);

        public int MarkAsHit(int from, int to) => MarkAsHit(from, to, Root);

        private static bool Add(Node node, Node nodeToAdd)
        {
            // return true, if the height of the tree has been increased
            // then the parent node must be balanced
            int compare = nodeToAdd.Key.CompareTo(node.Key);
            if (compare == 0)
            {
                throw new Exception("Duplicate found");
            }

            node.NodesCount++;
            if (compare < 0)
            {
                if (node.Left == null)
                {
                    node.Left = nodeToAdd;
                    if (node.Right != null)
                        return false;
                    node.Height++;
                    return true;
                }
                if (!Add(node.Left, nodeToAdd))
                    return false;
            }
            else
            {
                if (node.Right == null)
                {
                    node.Right = nodeToAdd;
                    if (node.Left != null)
                        return false;
                    node.Height++;
                    return true;
                }
                if (!Add(node.Right, nodeToAdd))
                    return false;
            }
            return Balance(node);
        }

        private static int GetByIndex(int index, Node node)
        {
            var leftCount = node.LeftNodesCount;
            if (leftCount == index)
                return node.Key;
            if (leftCount > index)
                return GetByIndex(index, node.Left);
            return GetByIndex(index - leftCount - 1, node.Right);
        }

        private static int MarkAsHit(int from, int to, Node node)
        {
            var newHits = 0;
            var leftCount = node.LeftNodesCount;
            
            if (from < leftCount)
                newHits += MarkAsHit(from, Math.Min(leftCount, to), node.Left);
            
            if (from <= leftCount && to > leftCount && !node.IsHit)
            {
                node.IsHit = true;
                newHits++;
            }

            if (to > leftCount + 1 && node.Right != null)
                newHits += MarkAsHit(Math.Max(from - leftCount - 1, 0), to - leftCount - 1, node.Right);

            return newHits;
        }

        private class Node
        {
            public Node(int key, bool isHit)
            {
                Key = key;
                IsHit = isHit;
            }
            public int Key { get; set; }
            public bool IsHit { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            public int LeftHeight => Left?.Height ?? 0;
            public int RightHeight => Right?.Height ?? 0;
            public int Height { get; set; } = 1; // can be byte
            public int NodesCount { get; set; } = 1;
            public int LeftNodesCount => Left?.NodesCount ?? 0;
            public int RightNodesCount => Right?.NodesCount ?? 0;
        }

    }
}