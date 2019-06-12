using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace E.Decode
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var rnd = new Random();
            while (true)
            {
                var chars = Enumerable.Range(0, 300_000)
                    .Select(_ =>
                    {
                        switch (rnd.Next(3))
                        {
                        case 0:
                            return '~';
                        case 1:
                            return '7';
                        default:
                            return 'E';
//                        case 1:
//                            return '0' + rnd.Next(10);
//                        default:
//                            return 'A' + rnd.Next(26);
                        }
                    })
                    .Select(it => (char) it)
                    .ToArray();
                
                var str = new string(chars);
                if (GetTime(str) > 1600)
                    Console.WriteLine(string.Join("", chars.Take(300)));
            }
        }

        private static double GetTime(string s)
        {
            var sw = Stopwatch.StartNew();
            var tildasToReplace = ReadInput(s);
            var replacements = new List<Replacement>(tildasToReplace.Count);
            
            var answer = 0;
            
            while (tildasToReplace.Count > 0)
            {
                replacements.Clear();
                foreach (var node in tildasToReplace)
                {
                    char newChar;
                    if (TryGetReplacement(node, out newChar))
                        replacements.Add(new Replacement() { TildaNode = node, NewChar = newChar });
                }
                
                if (replacements.Count == 0)
                    break;
                
                answer++;
                
                tildasToReplace.Clear();
                foreach (var replacement in replacements)
                {
                    var newTilda = replacement.PerformReplace();
                    if (newTilda != null)
                        tildasToReplace.Add(newTilda);
                }
            }

            return sw.ElapsedMilliseconds;
        }

        private static bool TryGetReplacement(CharNode tildaNode, out char newChar)
        {
            int x, y;
            if (!IsHex(tildaNode.Next?.Value, out x) || !IsHex(tildaNode.Next.Next?.Value, out y))
            {
                newChar = default(char);
                return false;
            }

            newChar = char.ToUpper((char) (16 * x + y));
            return true;
        }

        private static HashSet<CharNode> ReadInput(string str)
        {
//            var str = new char[300_000];
//            for (var i = 0; i < str.Length / 3; i++)
//            {
//                str[3 * i] = '~';
//                str[3 * i + 1] = '7';
//                str[3 * i + 2] = 'E';
//            }

            var result = new HashSet<CharNode>();
            
            var preFirst = new CharNode(0);
            var current = preFirst;

            var index = 0;
            foreach (var ch in str)
            {
                var next = new CharNode(index++) { Value = char.ToUpper(ch) };
                current.Next = next;
                next.Prev = current;
                current = next;
                
                if (current.Value == '~')
                    result.Add(current);
            }

            preFirst.Next.Prev = null;
            return result;
        }
        
        private static bool IsHexChar(char ch) { return ch >= 'A' && ch <= 'F'; }

        private static bool IsHex(char? nullableChar, out int val)
        {
            if (nullableChar == null)
            {
                val = 0;
                return false;
            }

            var ch = nullableChar.Value;
            
            if (char.IsDigit(ch))
            {
                val = ch - '0';
                return true;
            }
            
            if (IsHexChar(ch))
            {
                val = 10 + (ch - 'A');
                return true;
            }

            val = 0;
            return false;
        }

        private struct Replacement
        {
            public CharNode TildaNode { get; set; }
            public char NewChar { get; set; }
            
            public CharNode PerformReplace()
            {
                if (NewChar != '~' && !char.IsDigit(NewChar) && !IsHexChar(NewChar))
                    return null;

                TildaNode.Value = NewChar;
                TildaNode.Next = TildaNode.Next.Next.Next;
                if (TildaNode.Next != null)
                    TildaNode.Next.Prev = TildaNode;

                if (NewChar == '~')
                    return TildaNode;
                var prev = TildaNode.Prev;
                if (prev == null)
                    return null;
                if (prev.Value == '~')
                    return prev;

                var prevPrev = prev.Prev;
                return prevPrev?.Value == '~'? prevPrev : null;
            }
        }

        private class CharNode : IEquatable<CharNode>
        {
            private readonly int _hash;
            public CharNode(int index) { _hash = index; }
            
            public char Value { get; set; }
            
            public CharNode Next { get; set; }
            
            public CharNode Prev { get; set; }

            public override int GetHashCode() { return _hash; }
            public bool Equals(CharNode other) { return _hash == other?._hash; }
            public override bool Equals(object obj) => Equals(obj as CharNode);
        }
    }
}