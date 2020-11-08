using System;
using System.Collections.Generic;
using System.Linq;

namespace Q.LimitQueries
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var firstLineParts = Console.ReadLine().Split().Select(int.Parse).ToArray();
            var solver = new Solver(firstLineParts[0], firstLineParts[1], firstLineParts[2]);
            while (true)
            {
                var timeUser = Console.ReadLine().Split().Select(int.Parse).ToArray();
                if (timeUser.Length == 1)
                    break;
                var request = new RequestInfo() {Time = timeUser[0], UserId = timeUser[1]};
                Console.WriteLine(solver.Request(request));
            }
        }
    }
    
    internal struct RequestInfo
    {
        public int Time { get; set; }
        public int UserId { get; set; }
    }

    internal sealed class Solver
    {
        private const int MaxRequests = 5 * 10 * 1000;
        private const int Ok = 200;
        private const int TooManyRequests = 429;
        private const int ServiceUnavailable = 503;
        
        private readonly int UserLimit;
        private readonly int ServiceLimit;
        private readonly int Duration;
        
        private int ServiceRunningRequests = 0;
        private Dictionary<int, int> UserRunningRequests = new Dictionary<int, int>(MaxRequests);
        private Queue<RequestInfo> RequestsQueue = new Queue<RequestInfo>(MaxRequests);

        public Solver(int userLimit, int serviceLimit, int duration)
        {
            UserLimit = userLimit;
            ServiceLimit = serviceLimit;
            Duration = duration;
        }
        
        public int Request(RequestInfo request)
        {
            TrimTimedOutRequests(request);
            
            int userRunningRequests;
            UserRunningRequests.TryGetValue(request.UserId, out userRunningRequests);

            if (userRunningRequests >= UserLimit)
                return TooManyRequests;
            if (ServiceRunningRequests >= ServiceLimit)
                return ServiceUnavailable;
            
            RequestsQueue.Enqueue(request);
            UserRunningRequests[request.UserId] = userRunningRequests + 1;
            ++ServiceRunningRequests;
            return Ok;
        }

        private void TrimTimedOutRequests(RequestInfo request)
        {
            var trimRequestsUntil = request.Time - Duration;
            while (RequestsQueue.Count > 0 && RequestsQueue.Peek().Time < trimRequestsUntil)
            {
                var timedOutRequest = RequestsQueue.Dequeue();
                --ServiceRunningRequests;
                --UserRunningRequests[timedOutRequest.UserId];
            }
        }
    }
}