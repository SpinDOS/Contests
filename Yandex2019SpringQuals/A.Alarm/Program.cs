using System;
using System.Collections.Generic;
using System.Linq;

namespace A.Alarm
{
    class Program
    {
        static void Main(string[] args)
        {
            var nxk = Console.ReadLine().Split();
            var x = ulong.Parse(nxk[1]);
            var k = ulong.Parse(nxk[2]);

            var nonIntersectingAlarms = new Dictionary<ulong, ulong>();
            foreach (var alarmStr in Console.ReadLine().Split())
            {
                var alarm = ulong.Parse(alarmStr);
                var alarmGroup = alarm % x;

                ulong minStart;
                if (!nonIntersectingAlarms.TryGetValue(alarmGroup, out minStart) || minStart > alarm)
                    nonIntersectingAlarms[alarmGroup] = alarm;
            }

            var alarmStarts = nonIntersectingAlarms.Values.OrderBy(t0 => t0).ToList();
            Console.WriteLine(Solve(x, k, alarmStarts));
        }

        private static ulong Solve(ulong x, ulong k, List<ulong> alarmStarts)
        {
            alarmStarts.Add(ulong.MaxValue);
            var currentTime = 0UL;
            var runningAlarms = 0;

            while (true)
            {
                var runningAlarmsUL = (ulong) runningAlarms;
                var periodBound = (alarmStarts[runningAlarms] / x) * x;
                var runnedAlarms = (periodBound - currentTime) / x * runningAlarmsUL;
                
                if (runnedAlarms >= k)
                {
                    var fullCicles = k / runningAlarmsUL;
                    if (k % runningAlarmsUL == 0)
                        fullCicles--;
                    
                    k -= fullCicles * runningAlarmsUL;
                    currentTime += fullCicles * x;
                    break;
                }

                k -= runnedAlarms;
                currentTime = periodBound;
                var possibleRunningAlarms = runningAlarms;
                periodBound += x;
                while (alarmStarts[possibleRunningAlarms] < periodBound)
                    possibleRunningAlarms++;
                
                if (k <= (ulong) possibleRunningAlarms)
                    break;
                else
                    runningAlarms = possibleRunningAlarms;
            }

            var nextPeriodBound = currentTime + x;
            return currentTime + alarmStarts
                .Where(it => it < nextPeriodBound)
                .Select(it => it % x)
                .OrderBy(it => it)
                .ElementAt((int)(k - 1));
        }
    }
}