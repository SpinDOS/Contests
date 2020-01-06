using System;
using System.Linq;

namespace C.PetyaAndExam
{
    internal static class Program
    {
        public static void Main()
        {
            var m = int.Parse(Console.ReadLine());
            for (var i = 0; i < m; i++)
            {
                var examInfo = ReadInput();
                var solution = new Solver(examInfo).Solve();
                Console.WriteLine(solution);
            }
        }

        private static ExamInfo ReadInput()
        {
            var examInfo = new ExamInfo();
         
            var ntab = Console.ReadLine().Split();

            examInfo.Problems = new ProblemInfo[int.Parse(ntab[0])];
            examInfo.Time = int.Parse(ntab[1]);
            examInfo.EasyProblemSolveTime = int.Parse(ntab[2]);
            examInfo.HardProblemSolveTime = int.Parse(ntab[3]);

            foreach (var isHard in Console.ReadLine().Split().Select((value, i) => new {value, i}))
                examInfo.Problems[isHard.i].IsHard = isHard.value == "1";

            foreach (var mandatorySolveTime in Console.ReadLine().Split().Select((value, i) => new {value, i}))
                examInfo.Problems[mandatorySolveTime.i].MandatorySolveTime = int.Parse(mandatorySolveTime.value);

            return examInfo;
        }
    }

    internal sealed class Solver
    {
        private readonly ExamInfo _examInfo;
        private int _hardProblemsLeft, _easyProblemsLeft;
        private int _problemIndex;
        private int _timeToSatisfyMandatories;

        public Solver(ExamInfo examInfo) => _examInfo = examInfo;

        public int Solve()
        {
            _hardProblemsLeft = _examInfo.Problems.Count(it => it.IsHard);
            _easyProblemsLeft = _examInfo.Problems.Length - _hardProblemsLeft;
            
            if (CanSolveAll())
                return _examInfo.Problems.Length;
            
            Array.Sort(_examInfo.Problems, (x, y) => x.MandatorySolveTime.CompareTo(y.MandatorySolveTime));
            
            _problemIndex = 0;
            _timeToSatisfyMandatories = 0;

            var maxPoint = 0;
            
            do
                maxPoint = Math.Max(maxPoint, CalculateCurrentPoint());
            while (MoveToNextMandatoryTimePoint());

            return maxPoint;
        }

        private bool CanSolveAll()
        {
            int timeToSolveAll;
            try
            {
                timeToSolveAll = checked(_easyProblemsLeft * _examInfo.EasyProblemSolveTime +
                                         _hardProblemsLeft * _examInfo.HardProblemSolveTime);
            }
            catch (OverflowException)
            {
                return false;
            }
            
            return timeToSolveAll <= _examInfo.Time;
        }

        private int CalculateCurrentPoint()
        {
            var freeTime= _examInfo.Problems[_problemIndex].MandatorySolveTime - 1 - _timeToSatisfyMandatories;
            if (freeTime < 0)
                return -1;
                
            var easyPoints = Math.Min(_easyProblemsLeft, freeTime / _examInfo.EasyProblemSolveTime);
            freeTime -= easyPoints * _examInfo.EasyProblemSolveTime;
            
            var hardPoints = Math.Min(_hardProblemsLeft, freeTime / _examInfo.HardProblemSolveTime);

            return _problemIndex + easyPoints + hardPoints;
        }

        private bool MoveToNextMandatoryTimePoint()
        {
            var curMandatoryTime = _examInfo.Problems[_problemIndex].MandatorySolveTime;
            while (true)
            {
                var curProblem = _examInfo.Problems[_problemIndex];

                if (curProblem.MandatorySolveTime != curMandatoryTime)
                    return true;
                
                if (++_problemIndex == _examInfo.Problems.Length)
                    return false;

                if (curProblem.IsHard)
                {
                    _timeToSatisfyMandatories += _examInfo.HardProblemSolveTime;
                    --_hardProblemsLeft;
                }
                else
                {
                    _timeToSatisfyMandatories += _examInfo.EasyProblemSolveTime;
                    --_easyProblemsLeft;
                }

                if (_timeToSatisfyMandatories > _examInfo.Time)
                    return false;
            }
        }
    }

    internal struct ExamInfo
    {
        public ProblemInfo[] Problems { get; set; }
        public int Time { get; set; }
        public int EasyProblemSolveTime { get; set; }
        public int HardProblemSolveTime { get; set; }
    }

    internal struct ProblemInfo
    {
        public bool IsHard { get; set; }
        public int MandatorySolveTime { get; set; }
    }
}