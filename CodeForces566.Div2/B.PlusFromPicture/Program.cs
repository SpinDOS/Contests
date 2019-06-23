using System;

namespace B.PlusFromPicture
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var hStr = Console.ReadLine().Split()[0];
            var solver = new Solver(int.Parse(hStr));

            var ok = solver.ReadUntilFirstStar() &&
                     solver.ReadUntilRow() &&
                     solver.ReadUntilStarsEnd() &&
                     solver.ReadUntilEnd();
            
            StripInput(solver.H);
            
            Console.WriteLine(ok ? "YES" : "NO");
        }

        private static void StripInput(int h)
        {
            while (h-- > 0)
                Console.ReadLine();
        }
    }

    internal class Solver
    {
        private int column;
        
        public int H { get; private set; }
        
        public Solver(int h) => H = h;

        public bool ReadUntilFirstStar()
        {
            while (H-- > 0)
            {
                var str = Console.ReadLine();
                column = str.IndexOf('*');
                if (column >= 0)
                    return column > 0 && str.LastIndexOf('*') == column;
            }

            return false;
        }

        public bool ReadUntilRow()
        {
            while (H-- > 0)
            {
                var str = Console.ReadLine();
                var firstStar = str.IndexOf('*');
                var lastStar = str.LastIndexOf('*');

                if (firstStar == column && firstStar == lastStar)
                    continue;
                
                for (var i = firstStar + 1; i < lastStar; i++)
                {
                    if (str[i] != '*')
                        return false;
                }

                return firstStar < column && lastStar > column;
            }

            return false;
        }

        public bool ReadUntilStarsEnd()
        {
            var hBeforeWhile = H;
            
            while (H-- > 0)
            {
                var str = Console.ReadLine();
                var firstStar = str.IndexOf('*');

                if (firstStar < 0)
                    break;

                if (firstStar != column || str.LastIndexOf('*') != column)
                    return false;
            }

            return hBeforeWhile - H > 1;
        }

        public bool ReadUntilEnd()
        {
            while (H-- > 0)
            {
                if (Console.ReadLine().IndexOf('*') >= 0)
                    return false;
            }

            return true;
        }
    }
}