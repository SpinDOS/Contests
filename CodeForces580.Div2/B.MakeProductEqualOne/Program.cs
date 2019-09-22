using System;
using System.Linq;

namespace B.MakeProductEqualOne
{
    internal static class Program
    {
        public static void Main()
        {
            Console.ReadLine(); // n
            var solver = new Solver();
            foreach (var a in Console.ReadLine().Split().Select(int.Parse))
                solver.AddNumber(a);

            Console.WriteLine(solver.GetTotalCost());
        }
    }

    internal sealed class Solver
    {
        private ulong _totalCost;
        private bool _oddNegativesCount;
        private bool _hasZero;

        public void AddNumber(int num)
        {
            if (num == 0)
            {
                _hasZero = true;
                _totalCost++;
                return;
            }
            
            if (num < 0)
            {
                _oddNegativesCount = !_oddNegativesCount;
                num = -num;
            }

            _totalCost += (ulong)(num - 1);
        }

        public ulong GetTotalCost()
        {
            if (!_hasZero && _oddNegativesCount)
                return _totalCost + 2;
            return _totalCost;
        }
    }
}