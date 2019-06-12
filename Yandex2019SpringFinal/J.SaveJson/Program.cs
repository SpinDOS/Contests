using System;
using System.Linq;

namespace J.SaveJson
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var str = Console.ReadLine();
            try 
            { 
                new Validator(str).Validate();
            }
            catch (JsonException e)
            {
                Console.WriteLine(str.Insert(e.Position, e.Expected.ToString()));
            }
        }

        private class Validator
        {
            private readonly string _str;
            private int _index;

            public Validator(string str) { _str = str; }

            public void Validate()
            {
                ReadDictOrString();
                throw new JsonException('{', 0);
            }

            private void MustBeString()
            {
                var start = _index;

                if (_str[start] != '"')
                    throw new JsonException('"', _index);

                _index++;
                while (_index < _str.Length)
                {
                    var ch = _str[_index];
                    if (ch == '"')
                    {
                        if (start == _index)
                            throw new JsonException('a', _index);
                        
                        _index++;
                        return;
                    }

                    if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || char.IsDigit(ch))
                        _index++;
                    else
                        break;
                }
                
                throw new JsonException('"', _index);
            }

            private void ReadDict()
            {
                if (_index == _str.Length || _str[_index] == ',')
                    throw new JsonException('}', _index);
                
                if (_str[_index] == '}')
                {
                    _index++;
                    return;
                }
                
                while (true)
                {
                    MustBeString();
                    if (_str[_index] != ':')
                        throw new JsonException(':', _index);

                    var valueStartIndex = ++_index;
                    ReadDictOrString();
                    
                    if (_index == _str.Length)
                        throw new JsonException('}', _index);

                    switch (_str[_index])
                    {
                        case '}':
                            _index++;
                            return;
                        case ',':
                            _index++;
                            break;
                        case ':':
                            throw new JsonException('{', valueStartIndex);
                        default:
                            throw new JsonException(',', _index);
                    }
                }
            }
            
            private void ReadDictOrString()
            {
                switch (_str[_index])
                {
                case '{':
                    _index++;
                    ReadDict();
                    break;
                case '}':
                    throw new JsonException('{', _index);
                default:
                    MustBeString();
                    break;
                }
            }
        }

        private class JsonException : Exception
        {
            public JsonException(char expected, int position)
            {
                Expected = expected;
                Position = position;
            }
            
            public char Expected { get; }
            public int Position { get; }
        }
    }
}