using System;
using System.Collections.Generic;
using System.Linq;

namespace D.Exceptions
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var x = int.Parse(Console.ReadLine());
            var n = int.Parse(Console.ReadLine());

            var solver = new Solver(n);
            var functions = ParseFunctionBodies(n, solver.FunctionBodies);

            foreach (var functionName in functions)
            {
                var exceptions = solver.Solve(functionName);
                var t = Math.Min(x, exceptions.Count);

                Console.WriteLine(functionName + ":");
                Console.WriteLine(t);
                if (t != 0)
                    Console.WriteLine(string.Join(" ", exceptions.Take(t)));

//                foreach (var exc in exceptions.Take(t))
//                    Console.WriteLine(exc);
            }
        }

        private static List<string> ParseFunctionBodies(int n, Dictionary<string, List<Instruction>> functionBodies)
        {
            var parser = new Parser();
            var functions = new List<string>(n);
            
            for (var i = 0; i < n; i++)
            {
                var instructions = new List<Instruction>();
                var functionName = parser.ParseFunction(instructions);
                
                functions.Add(functionName);
                functionBodies.Add(functionName, instructions);
            }

            return functions;
        }
    }
    
    internal abstract class Instruction { }

    internal sealed class MaybeThrow : Instruction
    {
        public string ExceptionName { get; set; }
    }

    internal sealed class TrySuppress : Instruction
    {
        public List<Instruction> InnerInstructions { get; } = new List<Instruction>();
        public HashSet<string> Suppress { get; } = new HashSet<string>();
    }

    internal sealed class FunctionCall : Instruction
    {
        public string FunctionName { get; set; }
    }

    internal class Solver
    {
        private static readonly SortedSet<string> EmptySet = new SortedSet<string>();

        private readonly List<IsCacheFunction> _currentInspectingFunctions;

        public Dictionary<string, List<Instruction>> FunctionBodies { get; }
        
        public Dictionary<string, SortedSet<string>> FunctionThrows { get; }

        public Solver(int n)
        {
            FunctionBodies = new Dictionary<string, List<Instruction>>(n);
            FunctionThrows = new Dictionary<string, SortedSet<string>>(n);
            _currentInspectingFunctions = new List<IsCacheFunction>(n);
        }

        public SortedSet<string> Solve(string functionName)
        {
            SortedSet<string> result;
            if (FunctionThrows.TryGetValue(functionName, out result))
                return result;

            return SolveInternal(functionName);
        }

        private SortedSet<string> SolveInternal(string functionName)
        {
            var isCacheFunction = new IsCacheFunction() { FunctionName = functionName };
            _currentInspectingFunctions.Add(isCacheFunction);
            
            var result = InspectBlock(FunctionBodies[functionName]);
            
            _currentInspectingFunctions.RemoveAt(_currentInspectingFunctions.Count - 1);
            if (!isCacheFunction.SkipSave)
                FunctionThrows.Add(functionName, result);

            return result;
        }

        private SortedSet<string> InspectBlock(List<Instruction> instructions)
        {
            var exceptions = new SortedSet<string>();
            
            foreach (var instruction in instructions)
            {
                var mayThrow = instruction as MaybeThrow;
                if (mayThrow != null)
                {
                    exceptions.Add(mayThrow.ExceptionName);
                    continue;
                }

                var trySuppress = instruction as TrySuppress;
                if (trySuppress != null)
                {
                    var innerExceptions = InspectBlock(trySuppress.InnerInstructions);
                    innerExceptions.ExceptWith(trySuppress.Suppress);
                    exceptions.UnionWith(innerExceptions);
                    continue;
                }

                var functionName = ((FunctionCall)instruction).FunctionName;
                var exceptionsOfFunction = HandleFunctionCall(functionName);
                exceptions.UnionWith(exceptionsOfFunction);
            }

            return exceptions;
        }

        private SortedSet<string> HandleFunctionCall(string functionName)
        {
            SortedSet<string> exceptionsOfFunction;
            if (FunctionThrows.TryGetValue(functionName, out exceptionsOfFunction))
                return exceptionsOfFunction;

            var isAlreadyInspectingThisFunction = false;
            
            foreach (var inspectingFunction in _currentInspectingFunctions)
            {
                inspectingFunction.SkipSave |= isAlreadyInspectingThisFunction;
                isAlreadyInspectingThisFunction = isAlreadyInspectingThisFunction ||
                                                  inspectingFunction.FunctionName == functionName;
            }

            return isAlreadyInspectingThisFunction? EmptySet : SolveInternal(functionName);
        }

        private class IsCacheFunction
        {
            public string FunctionName;
            public bool SkipSave;
        }
    }

    internal class Parser
    {
        private string currentString = string.Empty;
        private int position;
        
        public string ParseFunction(List<Instruction> instructions)
        {
            ValidateNextToken("func");
            var functionName = ReadNextToken();
            ValidateBraces();
            ParseBracketsBlock(instructions);
            return functionName;
        }

        private void ParseBracketsBlock(List<Instruction> instructions)
        {
            ValidateNextToken("{");
            string nextToken = null;

            var openedBrackets = 1;
            while (openedBrackets > 0)
            {
                var token = nextToken;
                if (token == null)
                    token = ReadNextToken();
                else
                    nextToken = null;
                
                switch (token)
                {
                case "{":
                    openedBrackets++;
                    break;
                case "}":
                    openedBrackets--;
                    break;
                case "maybethrow":
                    instructions.Add(new MaybeThrow() { ExceptionName = ReadNextToken() });
                    break;
                case "try":
                    var trySuppress = new TrySuppress();
                    instructions.Add(trySuppress);
                    ParseBracketsBlock(trySuppress.InnerInstructions);
                    ValidateNextToken("suppress");
                    do
                    {
                        trySuppress.Suppress.Add(ReadNextToken());
                        nextToken = ReadNextToken();
                    }
                    while (nextToken == ",");
                    
                    break;
                default:
                    instructions.Add(new FunctionCall() { FunctionName = token });
                    ValidateBraces();
                    break;
                }
            }
        }

        private string ReadNextToken()
        {
            while (true)
            {
                if (position == currentString.Length)
                {
                    currentString = Console.ReadLine();
                    position = 0;
                    continue;
                }
                
                var ch = currentString[position++];
                if (ch == '(' || ch == ')' || ch == '{' || ch == '}' || ch == ',')
                    return ch.ToString();

                if (IsWordPart(ch))
                    break;
            }
            
            var start = position - 1;
            while (position < currentString.Length && IsWordPart(currentString[position]))
                position++;

            return currentString.Substring(start, position - start);
        }

        private void ValidateBraces()
        {
            ValidateNextToken("(");
            ValidateNextToken(")");
        }

        private void ValidateNextToken(string expected)
        {
            var token = ReadNextToken();
            if (expected != token)
                throw new InvalidOperationException(string.Format("Unexpected token: '{0}' (expected '{1}')", token, expected));
        }

        private static bool IsWordPart(char ch)
        {
            if (ch == '_' || (ch >= '0' && ch <= '9'))
                return true;
            
            ch = char.ToUpper(ch);
            return ch >= 'A' && ch <= 'Z';
        }
    }
}