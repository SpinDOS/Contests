using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace D.DiscreteCentrifugalJumps
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            int.Parse(Console.ReadLine()); // n
            var h = Console.ReadLine().Split().Select(int.Parse).ToArray();
            Console.WriteLine(new Solver(h).Solve());
        }
    }

    internal sealed class Solver
    {
        private readonly int _n;

        private readonly int[] _h;
        
        private readonly int[] _bestOutcomingJumpUp;
        private readonly int[] _bestOutcomingJumpDown;
        private readonly int[] _bestIncomingJumpUp;
        private readonly int[] _bestIncomingJumpDown;
        
        private readonly int[] _bestPath;

        public Solver(int[] h)
        {
            _h = h;
            _n = _h.Length;

            _bestOutcomingJumpUp = new int[_n];
            _bestOutcomingJumpDown = new int[_n];
            _bestIncomingJumpUp = new int[_n];
            _bestIncomingJumpDown = new int[_n];

            _bestPath = new int[_n];
        }

        public int Solve()
        {
            FillBestJumps();
            FillBestPath();
            return _bestPath.Last();
        }

        private void FillBestJumps()
        {
            var stack = new Stack<int>(_n);


            ArrayFill(_bestOutcomingJumpDown, -1);
            stack.Push(0);
            for (var i = 1; i < _n; i++)
            {
                while (stack.Count > 0 && _h[i] <= _h[stack.Peek()])
                    _bestOutcomingJumpDown[stack.Pop()] = i;
                stack.Push(i);
            }
            stack.Clear();

            
            ArrayFill(_bestOutcomingJumpUp, -1);
            stack.Push(0);
            for (var i = 1; i < _n; i++)
            {
                while (stack.Count > 0 && _h[i] >= _h[stack.Peek()])
                    _bestOutcomingJumpUp[stack.Pop()] = i;
                stack.Push(i);
            }
            stack.Clear();


            ArrayFill(_bestIncomingJumpDown, -1);
            stack.Push(_n - 1);
            for (var i = _n - 2; i >= 0; i--)
            {
                while (stack.Count > 0 && _h[i] >= _h[stack.Peek()])
                    _bestIncomingJumpDown[stack.Pop()] = i;
                stack.Push(i);
            }
            stack.Clear();


            ArrayFill(_bestIncomingJumpUp, -1);
            stack.Push(_n - 1);
            for (var i = _n - 2; i >= 0; i--)
            {
                while (stack.Count > 0 && _h[i] <= _h[stack.Peek()])
                    _bestIncomingJumpUp[stack.Pop()] = i;
                stack.Push(i);
            }
            stack.Clear();
        }

        private void FillBestPath()
        {
            for (var i = 0; i < _n; i++)
                _bestPath[i] = i;
            
            for (var i = 0; i < _n; i++)
            {
                TryUpdateBestPath(_bestIncomingJumpDown[i], i);
                TryUpdateBestPath(_bestIncomingJumpUp[i], i);
                TryUpdateBestPath(i, _bestOutcomingJumpDown[i]);
                TryUpdateBestPath(i, _bestOutcomingJumpUp[i]);
            }
        }

        private void TryUpdateBestPath(int jumpFrom, int jumpTo)
        {
            if (jumpFrom < 0 || jumpTo < 0)
                return;
            var candidatePath = _bestPath[jumpFrom] + 1;
            if (candidatePath < _bestPath[jumpTo])
                _bestPath[jumpTo] = candidatePath;
        }

        private static void ArrayFill(int[] arr, int value)
        {
            for (var i = 0; i < arr.Length; i++)
                arr[i] = value;
        }
    }
}