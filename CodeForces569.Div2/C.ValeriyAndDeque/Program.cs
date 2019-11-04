using System;
using System.Collections.Generic;
using System.Linq;

namespace C.ValeriyAndDeque
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var q = int.Parse(Console.ReadLine().Split()[1]);
            var nums = Console.ReadLine().Split().Select(int.Parse).ToArray();

            var solver = new Solver();
            solver.Init(nums);

            for (var i = 0; i < q; i++)
            {
                var m = long.Parse(Console.ReadLine());
                var result = solver.GetAnswer(m);
                Console.WriteLine(result);
            }
        }
    }

    internal sealed class Solver
    {
        private string[] _answers;
        private int _stepsToFindMax, _period;

        public void Init(int[] nums)
        {
            _period = nums.Length - 1;
            _stepsToFindMax = FindIndexOfMax(nums);
            _answers = new string[_stepsToFindMax + _period];
            
            var deque = new Deque<int>(nums);

            var answerIndex = 0;
            while (answerIndex < _stepsToFindMax)
            {
                deque.MakeMove(out var first, out var second);
                _answers[answerIndex++] = $"{first} {second}";
            }

            var buffer = new int[nums.Length];
            deque.CopyTo(buffer);

            var head = buffer[0] + " ";
            for (var i = 1; i < buffer.Length; i++)
                _answers[answerIndex++] = head + buffer[i];
        }

        public string GetAnswer(long m)
        {
            --m;
            if (m >= _answers.Length)
                m = _stepsToFindMax + ((m - _stepsToFindMax) % _period);

            return _answers[m];
        }

        private static int FindIndexOfMax(int[] nums)
        {
            int max = 0, index = 0;
            for (var i = 0; i < nums.Length; i++)
            {
                var val = nums[i];
                if (val > max)
                {
                    max = val;
                    index = i;
                }
            }

            return index;
        }
    }

    internal sealed class Deque<T> where T : IComparable<T>
    {
        private Node _head, _tail;

        public Deque(IEnumerable<T> elements)
        {
            var preHead = new Node();
            var tail = preHead;

            foreach (var element in elements)
                tail = tail.Next = new Node() {Value = element};

            _head = preHead.Next;
            _tail = tail;
        }
        
        public void MakeMove(out T first, out T second)
        {
            var next = _head.Next;
            
            first = _head.Value;
            second = next.Value;

            Node min;
            if (first.CompareTo(second) < 0)
            {
                min = _head;
                _head = next;
            }
            else
            {
                min = next;
                _head.Next = next.Next;
            }

            _tail = _tail.Next = min;
        }

        public void CopyTo(T[] arr)
        {
            var head = _head;
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = head.Value;
                head = head.Next;
            }
        }

        private sealed class Node
        {
            public T Value { get; set; }
            public Node Next { get; set; }
        }
    }
}