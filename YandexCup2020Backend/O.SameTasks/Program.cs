using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace O.SameTasks
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            for (var i = 0; i < 10 ; i++)
            Console.WriteLine($"{Console.ReadLine()} {Console.ReadLine()}");
            return;
            var solver = new Solver();
            var output = new StringBuilder();
            while (true)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;
                
                if (line == "add")
                    solver.Add(Console.ReadLine(), Console.ReadLine());
                else
                {
                    var siteProblemId = Console.ReadLine().Split();
                    solver.Find(siteProblemId[0], siteProblemId[1], output);
                }
            }

            Console.Write(output.ToString());
        }
    }

    internal sealed class Solver
    {
        private readonly Dictionary<ProblemFullId, MatchingProblemsSet> _matchingProblemsSets = 
            new Dictionary<ProblemFullId, MatchingProblemsSet>();
        
        private readonly Dictionary<BookId, BookTrie> _bookTries = new Dictionary<BookId, BookTrie>();
        
        public void Add(string site, string bookStr)
        {
            var book = JsonConvert.DeserializeObject<Book>(bookStr);
            var bookId = CalculateBookId(book);
            if (!_bookTries.TryGetValue(bookId, out var bookRootTrie))
                _bookTries[bookId] = bookRootTrie = new BookTrie();
            
            var queue = new Queue<(Section section, BookTrie trie)>();
            queue.Enqueue((book, bookRootTrie));
            while (queue.Count > 0)
            {
                var (section, trie) = queue.Dequeue();
                if (section.Problems != null)
                {
                    while (trie.Problems.Count < section.Problems.Count)
                        trie.Problems.Add(new MatchingProblemsSet() { ProblemFullIds = new List<ProblemFullId>() });
                    for (var i = 0; i < section.Problems.Count; i++)
                    {
                        var problemFullId = new ProblemFullId() {Site = site, ProblemId = section.Problems[i].Id};
                        trie.Problems[i].ProblemFullIds.Add(problemFullId);
                        _matchingProblemsSets.Add(problemFullId, trie.Problems[i]);
                    }
                }

                if (section.Sections != null)
                {
                    while (trie.InnerSections.Count < section.Sections.Count)
                        trie.InnerSections.Add(new BookTrie());
                    for (var i = 0; i < section.Sections.Count; i++)
                        queue.Enqueue((section.Sections[i], trie.InnerSections[i]));
                }
            }
        }

        public void Find(string site, string problemId, StringBuilder output)
        {
            var fullProblemId = new ProblemFullId() {Site = site, ProblemId = problemId,};
            if (!_matchingProblemsSets.TryGetValue(fullProblemId, out var matchingProblems))
            {
                output.AppendLine("0");
                return;
            }

            output.AppendLine((matchingProblems.ProblemFullIds.Count - 1).ToString());
            foreach (var otherProblemId in matchingProblems.ProblemFullIds)
                if (otherProblemId.Site != site)
                    output.AppendLine($"{otherProblemId.Site} {otherProblemId.ProblemId}");
        }

        private static BookId CalculateBookId(Book book)
        {
            var bookId = new BookId() { SectionsStructure = new List<int>()};
            
            var queue = new Queue<Section>();
            queue.Enqueue(book);
            
            while (queue.Count > 0)
            {
                var section = queue.Dequeue();
                
                var sectionSize = section.Problems?.Count ?? section.Sections.Count;
                bookId.SectionsStructure.Add(sectionSize);
                bookId.Hash = HashCode.Combine(bookId.Hash, sectionSize);

                foreach (var innerSection in section.Sections ?? Enumerable.Empty<Section>())
                    queue.Enqueue(innerSection);
            }

            return bookId;
        }
        
        private struct MatchingProblemsSet
        {
            public List<ProblemFullId> ProblemFullIds { get; set; }
        }

        private sealed class BookTrie
        {
            public List<BookTrie> InnerSections { get; } = new List<BookTrie>();
            public List<MatchingProblemsSet> Problems { get; } = new List<MatchingProblemsSet>();
        }

        private struct BookId : IEquatable<BookId>
        {
            [JsonIgnore]
            public List<int> SectionsStructure { get; set; }
            [JsonIgnore] public int Hash { get; set; }
            public bool Equals(BookId other) => Hash == other.Hash && SectionsStructure.SequenceEqual(other.SectionsStructure);
            public override bool Equals(object obj) => obj is BookId bookId && Equals(bookId);
            public override int GetHashCode() => Hash;
        }

        private struct ProblemFullId : IEquatable<ProblemFullId>
        {
            public string Site { get; set; }
            public string ProblemId { get; set; }
            public bool Equals(ProblemFullId other) => Site == other.Site && ProblemId == other.ProblemId;
            public override bool Equals(object obj) => obj is ProblemFullId other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(Site, ProblemId);
        }
    }

    internal sealed class Problem
    {
        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }
    }

    internal class Section
    {
        [JsonProperty("title", Required = Required.Always)]
        public string Title { get; set; }
        
        [JsonProperty("sections", Required = Required.Default)]
        public List<Section> Sections { get; set; }
        
        [JsonProperty("problems", Required = Required.Default)]
        public List<Problem> Problems { get; set; }
    }

    internal sealed class Book : Section
    {
    }
}