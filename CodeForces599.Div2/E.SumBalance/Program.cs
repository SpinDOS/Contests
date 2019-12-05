using System;
using System.Collections.Generic;
using System.Linq;
 
using BoxId = System.Byte;
 
namespace E.SumBalance
{
    internal static class Program
    {
        public static void Main()
        {
            var (totalSum, boxes, numberToBoxMap) = ReadInput();
            var solution = new Solver(totalSum, boxes, numberToBoxMap).Solve();
            Console.WriteLine(solution != null ? "Yes" : "No");
            foreach (var numberMove in solution ?? Enumerable.Empty<NumberMove>())
                Console.WriteLine($"{numberMove.Number} {numberMove.TargetBox + 1}");
        }
 
        private static (long totalSum, Box[] boxes, Dictionary<int, BoxId> numberToBoxMap) ReadInput()
        {
            var numberToBoxMap = new Dictionary<int, BoxId>(15 * 5000);
 
            var k = BoxId.Parse(Console.ReadLine());
            
            var totalSum = 0L;
            var boxes = new Box[k];
            
            for (BoxId i = 0; i < k; i++)
            {
                var numStrs = Console.ReadLine().Split();
 
                var boxSum = 0L;
                var numbers = new List<int>(numStrs.Length - 1);
                foreach (var num in numStrs.Skip(1).Select(int.Parse))
                {
                    boxSum += num;
                    numbers.Add(num);
                    numberToBoxMap[num] = i;
                }
 
                totalSum += boxSum;
                boxes[i] = new Box(i, boxSum, numbers);
            }
 
            return (totalSum, boxes, numberToBoxMap);
        }
    }
 
    internal sealed class Solver
    {
        private readonly int _k;
        private readonly long _totalSum, _targetSum;
 
        private readonly Box[] _boxes;
        private readonly Dictionary<int, BoxId> _numberToBoxMap;
        private readonly List<MoveGroup>[] _moveGroups;
 
        public Solver(long totalSum, Box[] boxes, Dictionary<int, BoxId> numberToBoxMap)
        {
            _k = boxes.Length;
            _totalSum = totalSum;
            _boxes = boxes;
            _numberToBoxMap = numberToBoxMap;
            
            _targetSum = _totalSum / _k;
            _moveGroups = new List<MoveGroup>[_k];
        }
 
        public NumberMove[] Solve()
        {
            if (_totalSum % _k != 0)
                return null;
 
            for (BoxId boxId = 0; boxId < _k; boxId++)
                _moveGroups[boxId] = FindMoveGroups(_boxes[boxId]);
 
            var resultMoveGroups = new List<MoveGroup>(_k);
            
            if (!TryFindResultMoveGroups(BoxGroup.Empty, resultMoveGroups))
                return null;
            
            var result = new NumberMove[_k];
            foreach (var moveGroup in resultMoveGroups)
                ExpandMoveGroup(moveGroup, result);
 
            return result;
        }
 
        private List<MoveGroup> FindMoveGroups(Box box)
        {
            var initialGroup = BoxGroup.Elementary[box.Id];
            var sumDiff = _targetSum - box.Sum;
            
            var usedGroups = new HashSet<int>();
            var moveGroups = new List<MoveGroup>(box.Numbers.Count);
            
            foreach (var number in box.Numbers)
            {
                var boxGroup = FindSelfContainingBoxGroup(initialGroup, number, sumDiff + number);
                if (boxGroup != BoxGroup.Empty && usedGroups.Add(boxGroup.Flags))
                    moveGroups.Add(new MoveGroup(box.Id, boxGroup, number));
            }
 
            return moveGroups;
        }
 
        private BoxGroup FindSelfContainingBoxGroup(BoxGroup boxGroup, long startBoxNumber, long searchForNumber)
        {
            while (searchForNumber != startBoxNumber)
            {
                if (searchForNumber < int.MinValue || searchForNumber > int.MaxValue)
                    return BoxGroup.Empty;
                
                if (!_numberToBoxMap.TryGetValue((int)searchForNumber, out var boxId))
                    return BoxGroup.Empty;
 
                var newGroup = BoxGroup.Elementary[boxId];
                if (newGroup.Intersects(boxGroup))
                    return BoxGroup.Empty;
 
                boxGroup = boxGroup.Merge(newGroup);
                searchForNumber = _targetSum - (_boxes[boxId].Sum - searchForNumber);
            }
 
            return boxGroup;
        }
 
        private bool TryFindResultMoveGroups(BoxGroup currentUsed, List<MoveGroup> moveGroups)
        {
            var boxId = Array.FindIndex(BoxGroup.Elementary, it => !it.Intersects(currentUsed));
            if (boxId < 0 || boxId >= _k)
                return true;
 
            foreach (var moveGroup in _moveGroups[boxId])
            {
                if (currentUsed.Intersects(moveGroup.BoxGroup))
                    continue;
                
                if (TryFindResultMoveGroups(currentUsed.Merge(moveGroup.BoxGroup), moveGroups))
                {
                    moveGroups.Add(moveGroup);
                    return true;
                }
            }
                
 
            return false;
        }
 
        private void ExpandMoveGroup(MoveGroup moveGroup, NumberMove[] result)
        {
            var targetBoxId = moveGroup.BoxId;
            var searchFor = moveGroup.InitialMoveNumber;
 
            while (true)
            {
                searchFor = (int)(_targetSum - (_boxes[targetBoxId].Sum - searchFor));
                if (searchFor == moveGroup.InitialMoveNumber)
                    break;
 
                var sourceBoxId =  _numberToBoxMap[searchFor];
                result[sourceBoxId] = new NumberMove(searchFor, targetBoxId);
                targetBoxId = sourceBoxId;
            }
            
            result[moveGroup.BoxId] = new NumberMove(moveGroup.InitialMoveNumber, targetBoxId);
        }
    }
 
    struct BoxGroup
    {
        public static BoxGroup[] Elementary { get; } =
            Enumerable.Range(0, 15).Select(it => new BoxGroup(1 << it)).ToArray();
        
        public static BoxGroup Empty { get; } = new BoxGroup(0);
        
        private BoxGroup(int flags) => Flags = flags;
        
        public int Flags { get; }
        
        public BoxGroup Merge(BoxGroup other) => new BoxGroup(Flags | other.Flags);
        public bool Intersects(BoxGroup other) => (Flags & other.Flags) != 0;
 
        public static bool operator==(BoxGroup lhs, BoxGroup rhs) => lhs.Flags == rhs.Flags;
        public static bool operator!=(BoxGroup lhs, BoxGroup rhs) => lhs.Flags != rhs.Flags;
    }
 
    struct MoveGroup
    {
        public BoxId BoxId { get; }
        public BoxGroup BoxGroup { get; }
        public int InitialMoveNumber { get; }
 
        public MoveGroup(BoxId boxId, BoxGroup boxGroup, int initialMoveNumber)
        {
            BoxId = boxId;
            BoxGroup = boxGroup;
            InitialMoveNumber = initialMoveNumber;
        }
    }
 
    struct Box
    {
        public BoxId Id { get; }
        public long Sum { get; }
        public List<int> Numbers { get; }
 
        public Box(BoxId id, long sum, List<int> numbers)
        {
            Id = id;
            Sum = sum;
            Numbers = numbers;
        }
    }
 
    struct NumberMove
    {
        public int Number { get; }
        public BoxId TargetBox { get; }
 
        public NumberMove(int number, BoxId targetBox)
        {
            Number = number;
            TargetBox = targetBox;
        }
    }
}