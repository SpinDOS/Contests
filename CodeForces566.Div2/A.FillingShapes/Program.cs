using System;

namespace A.FillingShapes
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var n = uint.Parse(Console.ReadLine());
            Console.WriteLine(n % 2 == 1? 0 : Pow2(n / 2));
        }

        private static int Pow2(uint n)
        {
            var result = 1;
            var currentPow = 2;
            
            while (n > 0)
            {
                if ((n & 1) == 1)
                    result *= currentPow;
                
                currentPow *= currentPow;
                n >>= 1;
            }

            return result;
        }
    }
}