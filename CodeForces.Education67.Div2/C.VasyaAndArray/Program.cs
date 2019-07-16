using System;
using System.Collections.Generic;

namespace C.VasyaAndArray
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var nm = Console.ReadLine().Split();
            var n = int.Parse(nm[0]);
            var m = int.Parse(nm[1]);
            
            var solver = new Solver();
            for (var i = 0; i < m; i++)
            {
                var tlr = Console.ReadLine().Split();
                var interval = new Interval() { Start = int.Parse(tlr[1]), End = int.Parse(tlr[2]), };
                
                if (tlr[0] == "1")
                    solver.AddNonDecreasing(interval);
                else
                    solver.AddDecreasing(interval);
            }

            var pointsOfDecrease = solver.GetPointOfDecrease();
            if (pointsOfDecrease == null)
            {
                Console.WriteLine("NO");
            }
            else
            {
                Console.WriteLine("YES");
                PrintSolution(n, pointsOfDecrease);
            }
        }

        private static void PrintSolution(int n, SortedSet<int> pointsOfDecrease)
        {
            var value = pointsOfDecrease.Count + 1;
            var printedElements = 1;
            
            foreach (var pointOfDecrease in pointsOfDecrease)
            {
                while (printedElements < pointOfDecrease)
                {
                    Console.Write(value);
                    Console.Write(' ');
                    ++printedElements;
                }
                
                --value;
            }

            while (printedElements++ < n)
            {
                Console.Write(value);
                Console.Write(' ');
            }

            Console.WriteLine(value);
        }
    }

    internal sealed class Solver
    {
        private readonly SortedSet<Interval> _nonDecreasing = new SortedSet<Interval>();
        private readonly List<Interval> _decreasing = new List<Interval>(1000);
        
        public void AddNonDecreasing(Interval interval)
        {
            _nonDecreasing.Add(interval);
        }
        
        public void AddDecreasing(Interval interval)
        {
            _decreasing.Add(interval);
        }

        public SortedSet<int> GetPointOfDecrease()
        {
            var pointsOfDecrease = new SortedSet<int>();
            
            foreach (var decreasing in _decreasing)
            {
                var pointOfDecrease = GetPointOfDecrease(decreasing);
                if (pointOfDecrease > decreasing.End)
                    return null;

                pointsOfDecrease.Add(pointOfDecrease);
            }

            return pointsOfDecrease;
        }

        private int GetPointOfDecrease(Interval decreasing)
        {
            var decreasePoint = decreasing.Start + 1;
            
            foreach (var interval in _nonDecreasing)
            {
                if (interval.End < decreasePoint)
                    continue;
                
                if (interval.Start >= decreasePoint)
                    return decreasePoint;

                decreasePoint = interval.End + 1;
            }

            return decreasePoint;
        }
    }

    internal struct Interval : IComparable<Interval>
    {
        public int Start { get; set; }
        public int End { get; set; }

        public int CompareTo(Interval other)
        {
            var startComparison = Start.CompareTo(other.Start);
            if (startComparison != 0) return startComparison;
            return End.CompareTo(other.End);
        }
    }
}