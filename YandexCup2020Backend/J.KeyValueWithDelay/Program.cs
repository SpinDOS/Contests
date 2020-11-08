using System;
using System.Collections.Generic;
using KormenAlgorithms.BinaryHeap;

namespace J.KeyValueWithDelay
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var solver = new Solver();
            var parser = new Parser();
            while (true)
            {
                var str = Console.ReadLine();
                if (str == "-1")
                    break;
                Console.WriteLine(parser.ParseAndApply(solver, str));
            }
        }
    }
}

internal sealed class Parser
{
    public string ParseAndApply(Solver solver, string cmd)
    {
        var parts = cmd.Split('\t');
        var t1 = int.Parse(parts[0]);
        switch (parts[1])
        {
            case "set":
            {
                var t2 = int.Parse(parts[2]);
                var key = parts[3];
                var value = parts.Length == 5 ? parts[4] : string.Empty;
                var solveResult = solver.Set(t1, t2, key, value);
                return FormatGetAnswer(solveResult);
            }
            case "get":
            {
                var key = parts[2];
                var solveResult = solver.Get(t1, key);
                return FormatGetAnswer(solveResult);
            }
            case "cancel":
            {
                var t2 = int.Parse(parts[2]);
                var solveResult = solver.Cancel(t1, t2);
                return solveResult ? "true" : "false";
            }
            default:
                throw new Exception("Unknown command: " + parts[1]);
        }
    }

    private string FormatGetAnswer(string solveResult)
    {
        if (solveResult == null)
            return "false";
        return "true\t" + solveResult;
    }
}

internal sealed class Solver
{
    private const int MaxRequests = 100 * 1000;
    private sealed class SetCommand
    {
        public bool Cancelled { get; set; }
        public bool Applied { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    private struct SetCommandAndTimes
    {
        public int T1 { get; set; }
        public int T2 { get; set; }
        public SetCommand SetCommand { get; set; }
    }

    private class SetCommandAndTimesComparer : IComparer<SetCommandAndTimes>
    {
        public int Compare(SetCommandAndTimes x, SetCommandAndTimes y)
        {
            var t2Compare = x.T2.CompareTo(y.T2);
            if (t2Compare != 0)
                return -t2Compare;
            return -x.T1.CompareTo(y.T1);
        }
    }

    private readonly Dictionary<int, SetCommand> SetCommandsByT1 = new Dictionary<int, SetCommand>(MaxRequests);
    private readonly PriorityQueue<SetCommandAndTimes> SetCommandsByT2T1 = new PriorityQueue<SetCommandAndTimes>(MaxRequests, new SetCommandAndTimesComparer());
    private readonly Dictionary<string, string> CurrentStore = new Dictionary<string, string>(MaxRequests);

    public string Set(int t1, int t2, string key, string value)
    {
        var setCmd = new SetCommand() { Key = key, Value = value };
        SetCommandsByT1.Add(t1, setCmd);
        SetCommandsByT2T1.Insert(new SetCommandAndTimes() { T1 = t1, T2 = t2, SetCommand = setCmd });
        return Get(t1, key);
    }

    public string Get(int t1, string key)
    {
        ApplyUntil(t1);
        string value;
        CurrentStore.TryGetValue(key, out value);
        return value;
    }

    public bool Cancel(int t1, int t2)
    {
        ApplyUntil(t1);
        SetCommand setCmd;
        if (!SetCommandsByT1.TryGetValue(t2, out setCmd))
            return false;
        if (setCmd.Applied || setCmd.Cancelled)
            return false;
        setCmd.Cancelled = true;
        return true;
    }

    private void ApplyUntil(int t1)
    {
        while (SetCommandsByT2T1.Count > 0 && SetCommandsByT2T1.GetMax().T2 <= t1)
        {
            var setCmd = SetCommandsByT2T1.ExtractMax().SetCommand;
            if (!setCmd.Cancelled)
            {
                CurrentStore[setCmd.Key] = setCmd.Value;
                setCmd.Applied = true;
            }
        }

    }
}

namespace KormenAlgorithms.BinaryHeap
{
    internal static class CommonHelper
    {
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            var temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }

    internal class BinaryHeap<T>
    {
        public T[] Items { get; }
        public IComparer<T> Comparer { get; }
        public int HeapSize { get; set; }

        public BinaryHeap(T[] arr, IComparer<T> comparer = null)
        {
            Items = arr;
            HeapSize = arr.Length;
            Comparer = comparer ?? Comparer<T>.Default;
        }

        public static BinaryHeap<T> BuildHeap(T[] arr, IComparer<T> comparer = null)
        {
            var heap = new BinaryHeap<T>(arr, comparer);
            for (var i = (arr.Length - 1) / 2; i >= 0; i--)
                heap.MaxHeapify(i);
            return heap;
        }

        public void MaxHeapify(int index)
        {
            var largestIndex = index;

            for (var i = 1; i <= 2; i++)
            {
                var childIndex = 2 * index + i;
                if (childIndex >= HeapSize)
                    break;

                var largest = Items[largestIndex];
                var child = Items[childIndex];
                if (Comparer.Compare(largest, child) < 0)
                    largestIndex = childIndex;
            }

            if (largestIndex == index)
                return;

            CommonHelper.Swap(ref Items[index], ref Items[largestIndex]);
            MaxHeapify(largestIndex);
        }
    }

    public class PriorityQueue<T>
    {
        private BinaryHeap<T> _heap;

        public PriorityQueue(int size = 0, IComparer<T> comparer = null)
        {
            if (size <= 0)
                size = 100;

            _heap = new BinaryHeap<T>(new T[size], comparer) { HeapSize = 0 };
        }

        public int Count => _heap.HeapSize;

        public T GetMax()
        {
            CheckNotEmpty();
            return _heap.Items[0];
        }

        public T ExtractMax()
        {
            CheckNotEmpty();

            var result = _heap.Items[0];
            _heap.Items[0] = _heap.Items[--_heap.HeapSize];
            _heap.MaxHeapify(0);

            return result;
        }

        public void Insert(T item)
        {
            if (_heap.HeapSize == _heap.Items.Length)
            {
                var heap = new BinaryHeap<T>(new T[_heap.HeapSize * 2], _heap.Comparer) { HeapSize = _heap.HeapSize };
                Array.Copy(_heap.Items, heap.Items, _heap.Items.Length);
                _heap = heap;
            }

            _heap.Items[_heap.HeapSize] = item;
            IncreaseKey(_heap.HeapSize++, item);
        }

        private void IncreaseKey(int index, T value)
        {
            while (index > 0)
            {
                var parentIndex = (index - 1) / 2;
                var parent = _heap.Items[parentIndex];

                if (_heap.Comparer.Compare(parent, value) >= 0)
                    return;

                CommonHelper.Swap(ref _heap.Items[index], ref _heap.Items[parentIndex]);
                index = parentIndex;
            }
        }

        private void CheckNotEmpty()
        {
            if (_heap.HeapSize == 0)
                throw new ArgumentOutOfRangeException("Priority queue is empty");
        }
    }
}
