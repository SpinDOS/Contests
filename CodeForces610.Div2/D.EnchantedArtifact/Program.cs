using System;
using System.Text;

namespace D.EnchantedArtifact
{
    internal static class Program
    {
        public static void Main()
        {
            new Solver().Solve();
        }
    }

    internal sealed class Solver
    {
        private int _lastVerdict;
        private readonly StringBuilder _spell = new StringBuilder(300);
        
        public void Solve()
        {
            PrefillWithB();

            for (var i = 0; i < _spell.Length && !Finished; i++)
                GuessChar(i);
        }

        private bool Finished => _lastVerdict <= 0;

        private void PrefillWithB()
        {
            _spell.Append('a');
            Ask();
            if (Finished)
                return;

            _spell.Clear();
            _spell.Append('b', _lastVerdict);
            Ask();

            if (!Finished)
                _spell.Append('b');
        }

        private void GuessChar(int index)
        {
            var prevVerdict = _lastVerdict;
            _spell[index] = 'a';
            Ask();
            if (_lastVerdict < prevVerdict)
                return;

            _spell[index] = 'b';
            _lastVerdict = prevVerdict;
        }

        private void Ask()
        {
            Console.WriteLine(_spell);
            Console.Out.Flush();
            _lastVerdict = int.Parse(Console.ReadLine());
        }
    }
}