using System;

namespace H.Shards
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var nq = Console.ReadLine().Split();
            var storage = new ShardsStorage(int.Parse(nq[0]));
            var q = int.Parse(nq[1]);
            for (var i = 0; i < q; i++)
            {
                var parts = Console.ReadLine().Split();
                if (parts[0] == "+")
                    storage.IncWeight(int.Parse(parts[1]), uint.Parse(parts[2]));
                else
                    Console.WriteLine(storage.GetShard(ulong.Parse(parts[1])));
            }
        }

        class ShardsStorage
        {
            private const int GroupSize = 100;
            
            private readonly ulong[] _weights;
            private readonly ulong[] _sums;

            public ShardsStorage(int n)
            {
                _weights = new ulong[n];
                _sums = new ulong[n / GroupSize];
            }

            public void IncWeight(int id, uint weightChange)
            {
                _weights[id] += weightChange;

                for (var i = id / GroupSize; i < _sums.Length; i++)
                    _sums[i] += weightChange;
            }

            public int GetShard(ulong weight)
            {
                var groupIndex = FindGroupIndex(weight);
                var sum = groupIndex == 0 ? 0 : _sums[groupIndex - 1];
                
                for (var i = groupIndex * GroupSize; i < _weights.Length; i++)
                {
                    sum += _weights[i];
                    if (sum > weight)
                        return i - 1;
                }

                return _weights.Length - 1;
            }

            private int FindGroupIndex(ulong weight)
            {
                var start = 0;
                var end = _sums.Length;
                while (start != end)
                {
                    var mid = (start + end) / 2;
                    if (_sums[mid] <= weight)
                        start = mid + 1;
                    else
                        end = mid;
                }

                return start;
            }
        }
    }
}