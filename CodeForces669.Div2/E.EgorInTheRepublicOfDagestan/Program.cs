using System;
using System.Collections.Generic;
using System.Linq;

namespace E.EgorInTheRepublicOfDagestan
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var nm = Console.ReadLine().Split();
            var n = int.Parse(nm[0]);
            var m = int.Parse(nm[1]);
            var solver = new Solver(n, m);
            for (var i = 0; i < m; i++)
            {
                var uvt = Console.ReadLine().Split();
                var u = int.Parse(uvt[0]) - 1;
                var v = int.Parse(uvt[1]) - 1;
                var t = uvt[2].Single() == '0' ? DayTime.Night : DayTime.Morning;
                solver.AddRoad(u, v, t);
            }

            Console.WriteLine(solver.Solve());
            var cityTypesStrs = solver.CityTypes.Select(it => it == DayTime.Night ? "0" : "1");
            Console.WriteLine(string.Join("", cityTypesStrs));
        }
    }

    [Flags]
    internal enum DayTime
    {
        Morning = 1,
        Night = 2,
        Any = Morning | Night,
    };

    internal sealed class Solver
    {
        private readonly int _n;
        private readonly int _m;

        private readonly bool[] _enqueued;
        private readonly DayTime[] _cityTypes;
        private readonly Dictionary<int, DayTime>[] _incomingRoads;
        
        public Solver(int n, int m)
        {
            _n = n;
            _m = m;
            
            _enqueued = new bool[n];

            _cityTypes = new DayTime[n];
            for (var i = 0; i < n; i++)
                _cityTypes[i] = DayTime.Any;

            _incomingRoads = new Dictionary<int, DayTime>[n];
            for (var i = 0; i < n; i++)
                _incomingRoads[i] = new Dictionary<int, DayTime>();
        }

        public void AddRoad(int from, int to, DayTime dayTime)
        {
            var incomingRoads = _incomingRoads[to];
            
            DayTime existingRoadType;
            if (!incomingRoads.TryGetValue(from, out existingRoadType))
                existingRoadType = dayTime;
            
            incomingRoads[from] = existingRoadType | dayTime;
        }

        public int Solve()
        {
            var hops = 0;
            var queue = new Queue<CityTypeOutgoingRoad>(_m);
            queue.Enqueue(new CityTypeOutgoingRoad() { City = _n - 1, DayTime = DayTime.Any,});
            while (queue.Count > 0)
            {
                var curHopSize = queue.Count;
                for (var i = 0; i < curHopSize; i++)
                    if (VisitOutgoingRoad(queue))
                        return hops;
                ++hops;
            }

            return -1;
        }

        public DayTime[] CityTypes => _cityTypes;

        private bool VisitOutgoingRoad(Queue<CityTypeOutgoingRoad> queue)
        {
            var road = queue.Dequeue();
            
            var cityStatus = _cityTypes[road.City];
            if (cityStatus == DayTime.Any)
                _cityTypes[road.City] = cityStatus = (cityStatus & Invert(road.DayTime));
            
            if ((cityStatus & road.DayTime) == 0)
                return false;
            
            if (road.City == 0)
                return true;

            if (!_enqueued[road.City])
            {
                foreach (var kv in _incomingRoads[road.City])
                    queue.Enqueue(new CityTypeOutgoingRoad() { City = kv.Key, DayTime = kv.Value });
                _enqueued[road.City] = true;
            }
            
            return false;
        }

        private static DayTime Invert(DayTime dayTime)
        {
            if (dayTime == DayTime.Any)
                return DayTime.Any;
            return dayTime == DayTime.Morning ? DayTime.Night : DayTime.Morning;
        }

        private struct CityTypeOutgoingRoad
        {
            public int City { get; set; }
            public DayTime DayTime { get; set; }
        }
    }
}