using System;
using System.Collections.Generic;
using System.Linq;

namespace D.SubarraySorting
{
    internal static class Program
    {
        public static void Main()
        {
            var t = int.Parse(Console.ReadLine());
            for (var i = 0; i < t; i++)
                Console.WriteLine(Solve() ? "YES" : "NO");
        }
        
        private static bool Solve()
        {
            var n = int.Parse(Console.ReadLine());
            var elementsA = Console.ReadLine().Split().Select(int.Parse);
            
            var nodeMap = new Queue<LinkedListNode<BNode>>[n + 1];
            var bList = ReadBList(nodeMap);
            
            foreach (var a in elementsA)
            {
                var nodesOfValueA = nodeMap[a];
                if (nodesOfValueA == null || nodesOfValueA.Count == 0)
                    return false;

                var node = nodesOfValueA.Dequeue();
                if (node.Value.MaxValueBefore >= node.Value.BValue)
                    return false;
                
                UpdateMaxValueInNextNodes(node);
                bList.Remove(node);
            }

            return true;
        }

        private static void UpdateMaxValueInNextNodes(LinkedListNode<BNode> node)
        {
            var maxValueBefore = node.Value.MaxValueBefore;
            var removedVal = node.Value.BValue;
            
            for (var nextNode = node.Next; nextNode != null; nextNode = nextNode.Next)
            {
                var val = nextNode.Value.BValue;
                nextNode.Value = new BNode()
                {
                    MaxValueBefore = maxValueBefore,
                    BValue = val
                };

                if (val >= removedVal)
                    break;
                    
                if (val > maxValueBefore)
                    maxValueBefore = val;
            }
        }

        private static LinkedList<BNode> ReadBList(Queue<LinkedListNode<BNode>>[] nodeMap)
        {
            var result = new LinkedList<BNode>();
            var maxValueBefore = 0;
            
            foreach (var b in Console.ReadLine().Split().Select(int.Parse))
            {
                var node = result.AddLast(new BNode()
                {
                    BValue = b,
                    MaxValueBefore = maxValueBefore,
                });
                
                if (b > maxValueBefore)
                    maxValueBefore = b;
                
                var queue = nodeMap[b] ?? 
                            (nodeMap[b] = new Queue<LinkedListNode<BNode>>());
                queue.Enqueue(node);
            }

            return result;
        }

        private struct BNode
        {
            public int BValue;
            public int MaxValueBefore;
        }
    }
}