using System;
using System.Collections.Generic;
using System.Linq;

namespace E.Bug
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var all = ReadInput();
            var result = Solve(all);
            PrintSolution(result);
        }

        private static SortedSet<StartLength> ReadInput()
        {
            var n = int.Parse(Console.ReadLine());
            var all = new SortedSet<StartLength>();
            
            for (var i = 0; i < n; i++)
            {
                var ax = Console.ReadLine().Split();
                var start = uint.Parse(ax[0]);
                var length = uint.Parse(ax[1]);
                
                var startLength = new StartLength()
                {
                    Start = start, 
                    Length = length,
                    End = start + length, 
                    Index = i
                };
                
                all.Add(startLength);
            }

            return all;
        }

        private static List<ResultContainer> Solve(SortedSet<StartLength> all)
        {
            var result = new List<ResultContainer>(all.Count);

            var bestTotalTime = 0UL;
            var lastEndTime = 0UL;
            
            foreach (var startLength in all)
            {
                var previousIndex = BinarySearch(result, startLength.Start);
                var previousTotalTime = previousIndex < 0? 0 : result[previousIndex].TotalTime;
                var totalTime = previousTotalTime + startLength.Length;
                
                if (bestTotalTime >= totalTime)
                    continue;
                
                var resultContainer = new ResultContainer()
                {
                    EndTime = startLength.End,
                    TotalTime = totalTime,
                    IndexOfPrevious = previousIndex,
                    ResultIndex = startLength.Index,
                };
                
                if (lastEndTime == startLength.End)
                    result[result.Count - 1] = resultContainer;
                else
                    result.Add(resultContainer);
                
                bestTotalTime = totalTime;
                lastEndTime = startLength.End;
            }

            return result;
        }

        private static int BinarySearch(List<ResultContainer> list, ulong startTime)
        {
            var start = 0;
            var end = list.Count;

            while (start != end)
            {
                var mid = (start + end) / 2;
                
                var endTime = list[mid].EndTime;
                if (endTime == startTime)
                    return mid;
                
                if (endTime > startTime)
                    end = mid;
                else
                    start = mid + 1;
            }

            return start - 1;
        }

        private static void PrintSolution(List<ResultContainer> result)
        {
            var winner = result.Last();
            Console.WriteLine(winner.TotalTime);
            
            while (true)
            {
                Console.Write(winner.ResultIndex);

                if (winner.IndexOfPrevious < 0)
                    break;

                Console.Write(' ');
                winner = result[winner.IndexOfPrevious];
            }
        }

        private struct StartLength : IComparable<StartLength>
        {
            public uint Start { get; set; }
            
            public uint Length { get; set; }
            
            public uint End { get; set; }
            
            public int Index { get; set; }

            public override string ToString() => $"{Start} {End} {Index}";

            public int CompareTo(StartLength other)
            {
                var comp = End.CompareTo(other.End);
                return comp != 0? comp : Start.CompareTo(other.Start);
            }
        }

        private struct ResultContainer
        {
            public ulong EndTime { get; set; }
            
            public ulong TotalTime { get; set; }
            
            public int ResultIndex { get; set; }
            
            public int IndexOfPrevious { get; set; }
        }
    }
}