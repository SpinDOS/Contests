using System;
using System.IO;

namespace E.PalindromicPaths
{
    internal static class Program
    {
        public static void Main()
        {
            var n = int.Parse(Console.ReadLine());
            var solver = new Solver(n, Console.In, Console.Out);

            try
            {
                solver.Solve();
            }
            catch (ProgramReportedMinusOneException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            Console.WriteLine("!");
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                    Console.Write(solver.Grid[i, j].Value ? 1 : 0);
                Console.WriteLine();
            }
        }
    }

    internal sealed class Solver
    {
        private readonly int _n;
        private readonly TextReader _input;
        private readonly TextWriter _output;
        private int _questionsCount;
        
        public bool?[,] Grid { get; }
        
        public Solver(int n, TextReader input, TextWriter output)
        {
            _n = n;
            _input = input;
            _output = output;
            
            Grid = new bool?[n, n];
            Grid[0, 0] = true;
            Grid[n - 1, n - 1] = false;
        }

        public void Solve()
        {
            FillOddPoints();
            var squareFrom1To0 = FindSquareFrom1To0();
            
            FillEvenInSquare3x3(squareFrom1To0);

            var filledEven = FindFilledEvenInSquare3x3(squareFrom1To0);
            FillEvenTillRow1Column2(filledEven);
            FillEvenPoints();
        }

        private void FillEvenInSquare3x3(Point topLeft)
        {
            // 1  x1 _
            // x2 c  x3
            // _  x4 0

            var x1 = topLeft.Add(0, 1);
            var x2 = topLeft.Add(1, 0);
            var x3 = topLeft.Add(1, 2);
            var x4 = topLeft.Add(2, 1);
            var bottomRight = topLeft.Add(2, 2);

            var cVal = GetGridValue(topLeft.Add(1, 1)).Value;
            
            if (AskPalindrom(topLeft, x4))
                SetGridValue(x4, true);
            else if (AskPalindrom(topLeft, x3))
                SetGridValue(x3, true);
            else if (AskPalindrom(x1, bottomRight))
                SetGridValue(x1, false);
            else if (AskPalindrom(x2, bottomRight))
            {
                SetGridValue(x2, false);
                SetGridValue(x4, false);
            }
            else if (AskPalindrom(x1, x3))
            {
                SetGridValue(x1, !cVal);
                SetGridValue(x3, !cVal);
                
                if (cVal)
                    SetGridValue(x4, false);
                else
                    SetGridValue(x2, true);
            }
            else
            {
                SetGridValue(x1, true);
                SetGridValue(x3, false);
                if (AskPalindrom(x2, x4))
                {
                    SetGridValue(x2, !cVal);
                    SetGridValue(x4, !cVal);
                }
                else
                {
                    SetGridValue(x2, true);
                    SetGridValue(x4, false);
                }
            }
        }

        private void FillOddPoints()
        {
            FillRow(0, 0);

            for (var row = 2; row < _n; row += 2)
            {
                FillFromPalindromOfLength3(new Point(row - 2, 0), new Point(row - 1, 1));
                FillRow(row - 1, 1);

                FillFromPalindromOfLength3(new Point(row - 2, 0), new Point(row, 0));
                FillRow(row, 0);
            }
        }

        private Point FindSquareFrom1To0()
        {
            for (var i = 2; i < _n; i += 2)
                if (!Grid[i, i].Value)
                    return new Point(i - 2, i - 2);
            throw new InvalidOperationException("Invalid square. Odd square always contains subsquare 3x3 1->0 in corners");
        }

        private Point FindFilledEvenInSquare3x3(Point topLeft)
        {
            Point point;
            
            point = topLeft.Add(0, 1);
            if (GetGridValue(point).HasValue)
                return point;
            
            point = topLeft.Add(1, 0);
            if (GetGridValue(point).HasValue)
                return point;
            
            point = topLeft.Add(1, 2);
            if (GetGridValue(point).HasValue)
                return point;

            return topLeft.Add(2, 1);
        }

        private void FillEvenTillRow1Column2(Point filledEven)
        {
            if (filledEven.X % 2 == 0)
            {
                if (filledEven.X + 1 == _n)
                    FillFromPalindromOfLength3AndReplace(ref filledEven, filledEven.Add(-2, 0));
                
                FillFromPalindromOfLength3AndReplace(ref filledEven, filledEven.Add(1, 1));
            }

            if (filledEven.Y == 0)
                FillFromPalindromOfLength3AndReplace(ref filledEven, filledEven.Add(0, 2));
            
            while (filledEven.Y > 2)
                FillFromPalindromOfLength3AndReplace(ref filledEven, filledEven.Add(0, -2));
            
            while (filledEven.X > 1)
                FillFromPalindromOfLength3AndReplace(ref filledEven, filledEven.Add(-2, 0));
        }

        private void FillEvenPoints()
        {
            for (var row = 1; row < _n; row += 2)
            {
                FillFromPalindromOfLength3(new Point(row - 2, 2), new Point(row, 2));
                FillFromPalindromOfLength3(new Point(row, 2), new Point(row, 0));
                FillRow(row, 2);
            }
            
            FillFromPalindromOfLength3(new Point(1, 2), new Point(0, 1));
            for (var row = 0; row < _n; row += 2)
            {
                FillFromPalindromOfLength3(new Point(row - 2, 1), new Point(row, 1));
                FillRow(row, 1);
            }
        }

        private void FillRow(int row, int startColumn)
        {
            for (var col = startColumn + 2; col < _n; col += 2)
                FillFromPalindromOfLength3(new Point(row, col - 2), new Point(row, col));
        }

        private void FillFromPalindromOfLength3AndReplace(ref Point from, Point to)
        {
            FillFromPalindromOfLength3(from, to);
            from = to;
        }

        private void FillFromPalindromOfLength3(Point from, Point to)
        {
            if (GetGridValue(to).HasValue)
                return;

            var val = GetGridValue(from).Value;
            if (!AskPalindrom(from, to))
                val = !val;
            
            SetGridValue(to, val);
        }

        private bool AskPalindrom(Point from, Point to)
        {
            if (from.X > to.X || from.Y > to.Y)
                Swap(ref from, ref to);
            
            _output.WriteLine($"? {from.X + 1} {from.Y + 1} {to.X + 1} {to.Y + 1}");
            _output.Flush();

            _questionsCount++;
            
            char answer;
            do
                answer = (char)_input.Read();
            while (answer != '0' && answer != '1' && answer != '-');
            
            if (answer == '0' || answer == '1')
                return answer == '1';

            throw new ProgramReportedMinusOneException(
                $"Got {answer} response. " +
                $"Asked {_questionsCount} questions (limit {_n * _n}). " +
                $"Last question: ({from.X + 1}, {from.Y + 1}) -> ({to.X + 1}, {to.Y + 1})");
        }

        private bool? GetGridValue(Point point) => Grid[point.X, point.Y];
        private void SetGridValue(Point point, bool value) => Grid[point.X, point.Y] = value;

        private static void Swap(ref Point x, ref Point y)
        {
            var z = x;
            x = y;
            y = z;
        }

        private struct Point
        {
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
            
            public int X { get; }
            public int Y { get; }
            
            public Point Add(int deltaX, int deltaY) => new Point(X + deltaX, Y + deltaY);
        }
    }

    internal sealed class ProgramReportedMinusOneException : Exception
    {
        public ProgramReportedMinusOneException(string message) : base(message) { }
    }
}