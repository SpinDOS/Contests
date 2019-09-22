using System;

namespace C.AlmostEqual
{
    internal static class Program
    {
        public static void Main()
        {
            var n = int.Parse(Console.ReadLine());
            if (n % 2 == 0)
                Console.WriteLine("NO");
            else
            {
                Console.WriteLine("YES");
                var circle = FillCircle(n);
                Console.WriteLine(string.Join(" ", circle));
            }
        }

        private static int[] FillCircle(int n)
        {
            var arr = new int[2 * n];
            
            var small = 1;
            var great = 2 * n;

            for (var i = 0; ; )
            {
                arr[i] = small++;
                arr[i + n] = small++;
                i++;
                
                if (small >= great)
                    break;
                
                arr[i] = great--;
                arr[i + n] = great--;
                i++;

                if (small >= great)
                    break;
            }

            return arr;
        }
    }
}