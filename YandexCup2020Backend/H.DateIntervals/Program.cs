using System;
using System.Linq;
using System.Text;

namespace H.DateIntervals
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var intervalType = Console.ReadLine();
            var startEnd = Console.ReadLine().Split()
                .Select(it => DateTime.ParseExact(it, "yyyy-MM-dd", null))
                .ToArray();
            Console.WriteLine(Solve(intervalType, startEnd[0], startEnd[1]));
        }

        private static string Solve(string intervalType, DateTime start, DateTime end)
        {
            var intervalsCount = 0;
            var intervals = new StringBuilder();
            var intervalProvider = GetIntervalProvider(intervalType);

            var current = start;
            while (current <= end)
            {
                var currentEnd = intervalProvider.GetIntervalEnd(current);
                if (currentEnd > end)
                    currentEnd = end;
                ++intervalsCount;
                intervals.AppendLine($"{current:yyyy-MM-dd} {currentEnd:yyyy-MM-dd}");
                current = currentEnd.AddDays(1);
            }

            return $"{intervalsCount}{Environment.NewLine}{intervals}";
        }

        private static IIntervalProvider GetIntervalProvider(string intervalType)
        {
            switch (intervalType?.ToUpperInvariant())
            {
                case "WEEK":
                    return new WeekIntervalProvider();
                case "MONTH":
                    return new MonthIntervalProvider();
                case "QUARTER":
                    return new QuarterIntervalProvider();
                case "YEAR":
                    return new YearIntervalProvider();
                case "LAST_SUNDAY_OF_YEAR":
                    return new LastSundayOfYearIntervalProvider();
                default:
                    throw new Exception("Unknown interval type: " + intervalType);
            }
        }
    }

    internal interface IIntervalProvider
    {
        DateTime GetIntervalEnd(DateTime start);
    }
    
    internal sealed class WeekIntervalProvider : IIntervalProvider
    {
        public DateTime GetIntervalEnd(DateTime start)
        {
            var daysUntilWeekEnd = (7 - (int) start.DayOfWeek) % 7;
            return start.AddDays(daysUntilWeekEnd);
        }
    }
    
    internal sealed class MonthIntervalProvider : IIntervalProvider
    {
        public DateTime GetIntervalEnd(DateTime start)
        {
            var nextMonth = start.AddMonths(1);
            return nextMonth.AddDays(-nextMonth.Day);
        }
    }
    
    internal sealed class QuarterIntervalProvider : IIntervalProvider
    {
        private static readonly MonthIntervalProvider MonthIntervalProvider = new MonthIntervalProvider();
        public DateTime GetIntervalEnd(DateTime start)
        {
            var monthUntilQuarterEnd = (3 - (start.Month % 3)) % 3;
            return MonthIntervalProvider.GetIntervalEnd(start.AddMonths(monthUntilQuarterEnd));
        }
    }
    
    internal sealed class YearIntervalProvider : IIntervalProvider
    {
        public DateTime GetIntervalEnd(DateTime start) => new DateTime(start.Year, 12, 31);
    }
    
    internal sealed class LastSundayOfYearIntervalProvider : IIntervalProvider
    {
        private static readonly YearIntervalProvider YearIntervalProvider = new YearIntervalProvider();
        private static readonly WeekIntervalProvider WeekIntervalProvider = new WeekIntervalProvider();

        public DateTime GetIntervalEnd(DateTime start)
        {
            var lastSaturday = GetLastSaturdayOfYear(start);
            return start <= lastSaturday
                ? lastSaturday
                : GetLastSaturdayOfYear(start.AddDays(7));
        }

        private DateTime GetLastSaturdayOfYear(DateTime start)
        {
            var yearEnd = YearIntervalProvider.GetIntervalEnd(start);
            return WeekIntervalProvider.GetIntervalEnd(yearEnd.AddDays(-6)).AddDays(-1);
        }
    }
}