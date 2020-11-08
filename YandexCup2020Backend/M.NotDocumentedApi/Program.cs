using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace M.NotDocumentedApi
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var lhs = JToken.Parse(Console.ReadLine());
            var rhs = JToken.Parse(Console.ReadLine());
            var result = new Solver().Solve(lhs, rhs);
            Console.WriteLine(result != null ? "YES" : "NO");
            if (result != null)
                Console.WriteLine(result);
        }
    }

    internal sealed class Solver
    {
        public string? Solve(JToken lhs, JToken rhs)
        {
            var lhsTrie = BuildTrie(lhs);
            return FindAnswer(rhs, lhsTrie);
        }

        private string? FindAnswer(JToken rootJson, TrieNode rootTrie)
        {
            string? result = null;
            Visit(rootTrie, rootJson, (trie, str) =>
            {
                if (result == null || result.Length < str.Length)
                    if (trie.StringChildren.Contains(str))
                        result = str;
            });
            return result;
        }

        private TrieNode BuildTrie(JToken rootJson)
        {
            var rootTrie = new TrieNode();
            Visit(rootTrie, rootJson, (trie, str) => trie.StringChildren.Add(str));
            return rootTrie;
        }

        private void Visit(TrieNode rootTrie, JToken rootJson, Action<TrieNode, string> onString)
        {
            var queue = new Queue<(TrieNode trieNode, JToken token)>();
            queue.Enqueue((rootTrie, rootJson));

            while (queue.Count > 0)
            {
                var (trie, token) = queue.Dequeue();
                if (token.Type == JTokenType.String)
                {
                    trie.StringChildren ??= new HashSet<string>();
                    var strValue = (string) ((JValue) token).Value;
                    onString.Invoke(trie, strValue);
                }
                else if (token.Type == JTokenType.Array)
                {
                    var arrayChild = trie.ArrayChild ??= new TrieNode();
                    foreach (var arrayItem in (JArray)token)
                        queue.Enqueue((arrayChild, arrayItem));
                }
                else if (token.Type == JTokenType.Object)
                {
                    var objChildren = trie.ObjectChildren ??= new Dictionary<string, TrieNode>();
                    foreach (var keyValue in (JObject) token)
                    {
                        if (!objChildren.TryGetValue(keyValue.Key, out var keyChild))
                            keyChild = objChildren[keyValue.Key] = new TrieNode();
                        queue.Enqueue((keyChild, keyValue.Value));
                    }
                }
            }
        }

        private sealed class TrieNode
        {
            public Dictionary<string, TrieNode> ObjectChildren { get; set; }
            public TrieNode ArrayChild { get; set; }
            public HashSet<string> StringChildren { get; set; }
        }
    }
}