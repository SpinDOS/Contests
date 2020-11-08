using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Q.SortPositivesDescending
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            var address = Console.ReadLine();
            var port = ushort.Parse(Console.ReadLine());
            var a = int.Parse(Console.ReadLine());
            var b = int.Parse(Console.ReadLine());

            var httpAnswer = await MakeHttpGetRequest($"{address}:{port}?a={a}&b={b}");
            var sortedAnswer = WithSystemJson(httpAnswer).Where(it => it > 0).OrderByDescending(it => it);
            Console.WriteLine(string.Join(Environment.NewLine, sortedAnswer));
        }

        private static long[] WithJson(string httpAnswer)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<long[]>(httpAnswer);
        }

        private static long[] WithSystemJson(string httpAnswer)
        {
            return System.Text.Json.JsonSerializer.Deserialize<long[]>(httpAnswer);
        }

        private static IEnumerable<long> WithoutJson(string httpAnswer)
        {
            var numArray = new System.Text.StringBuilder();
            foreach (var ch in httpAnswer)
            {
                if (char.IsDigit(ch) || ch == '-')
                    numArray.Append(ch);
                else
                    numArray.Append(' ');
            }

            return numArray.ToString()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse);
        }

        private static async Task<string> MakeHttpGetRequest(string url, bool manualTest = false)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                throw new Exception("Failed to parse URI");

            using var httpClient = new HttpClient();
            using var httpResponse = await httpClient.GetAsync(uri);
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            return !manualTest ? responseString : @"[  
  8,  
  6,  
  -2,  
  2,  
  4,  
  17,  
  256,  
  1024,  
  -17,  
  -19  
]";
        }
    }
}