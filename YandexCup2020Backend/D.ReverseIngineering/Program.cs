using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace D.ReverseIngineering
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var request = JsonSerializer.Deserialize<Request>(Console.ReadLine());
            Console.WriteLine(GetAnswer(request.Message, request.Number));
        }
/*
 
{"message": "a", "number": 10001} -> {"key_1": 40004, "key_2": "10%", "key_3": 11, "key_4": "a", "key_5": 1, "space_6": 0, "const_7": "0x40", "key_8": "1100001"}
{"message": "a", "number": 10002} -> {"key_1": 40008, "key_2": "10%", "key_3": 12, "key_4": "a", "key_5": 1, "space_6": 0, "const_7": "0x40", "key_8": "1100001"}
{"message": "a", "number": 10502} -> {"key_1": 42008, "key_2": "10%", "key_3": 125, "key_4": "a", "key_5": 1, "space_6": 0, "const_7": "0x40", "key_8": "1100001"}
{"message": "z", "number": 10502} -> {"key_1": 42008, "key_2": "10%", "key_3": 125, "key_4": "z", "key_5": 1, "space_6": 0, "const_7": "0x40", "key_8": "1111010"}
{"message": "z", "number": 145432} -> {"key_1": 581728, "key_2": "14%", "key_3": 123445, "key_4": "z", "key_5": 1, "space_6": 0, "const_7": "0x40", "key_8": "1111010"}
{"message": "abcd", "number": 145432} -> {"key_1": 581728, "key_2": "14%", "key_3": 123445, "key_4": "a", "key_5": 4, "space_6": 0, "const_7": "0x40", "key_8": "1100001"}
{"message": "dcba", "number": 145432} -> {"key_1": 581728, "key_2": "14%", "key_3": 123445, "key_4": "d", "key_5": 4, "space_6": 0, "const_7": "0x40", "key_8": "1100100"}
{"message": "dcbazzz", "number": 145432} -> {"key_1": 581728, "key_2": "14%", "key_3": 123445, "key_4": "d", "key_5": 7, "space_6": 0, "const_7": "0x40", "key_8": "1100100"}
{"message": "dc  bazz z", "number": 145432} -> {"key_1": 581728, "key_2": "14%", "key_3": 123445, "key_4": " ", "key_5": 10, "space_6": 3, "const_7": "0x40", "key_8": "1100100"}
{"message": "dc  bazz z", "number": 2147483647} -> {"key_1": 8589934588, "key_2": "21%", "key_3": 1234446778, "key_4": "z", "key_5": 10, "space_6": 3, "const_7": "0x40", "key_8": "1100100"}
{"message": "dc  bazz z", "number": 9223372036854775807} -> {"key_1": -4, "key_2": "92%", "key_3": 22233345567777889, "key_4": "z", "key_5": 10, "space_6": 3, "const_7": "0x40", "key_8": "1100100"}
{"message": "dc  bazz z", "number": 3074457345618258602} -> {"key_1": -6148914691236517208, "key_2": "30%", "key_3": 12233444555667788, "key_4": " ", "key_5": 10, "space_6": 3, "const_7": "0x40", "key_8": "1100100"}
{"message": "message", "number": 3074457345618258602} -> {"key_1": -6148914691236517208, "key_2": "30%", "key_3": 12233444555667788, "key_4": "s", "key_5": 7, "space_6": 0, "const_7": "0x40", "key_8": "1101101"}
{"message": "     ", "number": 1090604} -> {"key_1": 4362416, "key_2": "10%", "key_3": 1469, "key_4": " ", "key_5": 5, "space_6": 5, "const_7": "0x40", "key_8": "100000"}
{"message": "Dc  baZz z", "number": 3074457345618258602} -> {"key_1": -6148914691236517208, "key_2": "30%", "key_3": 12233444555667788, "key_4": " ", "key_5": 10, "space_6": 3, "const_7": "0x40", "key_8": "1000100"}
{"message": "dC  baZz z", "number": 3074457345618258602} -> {"key_1": -6148914691236517208, "key_2": "30%", "key_3": 12233444555667788, "key_4": " ", "key_5": 10, "space_6": 3, "const_7": "0x40", "key_8": "1100100"}
{"message": "DC  BAZZ Z", "number": 3074457345618258602} -> {"key_1": -6148914691236517208, "key_2": "30%", "key_3": 12233444555667788, "key_4": " ", "key_5": 10, "space_6": 3, "const_7": "0x40", "key_8": "1000100"}
{"message": "DC  BAZZ Z", "number": 1000000} -> {"key_1": 4000000, "key_2": "10%", "key_3": 1, "key_4": "D", "key_5": 10, "space_6": 3, "const_7": "0x40", "key_8": "1000100"}
{"message": "DC  BAZZ Z", "number": 99999} -> {"key_1": 399996, "key_2": "99%", "key_3": 99999, "key_4": "Z", "key_5": 10, "space_6": 3, "const_7": "0x40", "key_8": "1000100"}
{"message": "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", "number": 1001}
 -> {"key_1": 4004, "key_2": "10%", "key_3": 11, "key_4": "s", "key_5": 1301, "space_6": 27, "const_7": "0x40", "key_8": "1100001"}
{"message": "abcd\"def'", "number": 145432} -> {"key_1": 581728, "key_2": "14%", "key_3": 123445, "key_4": "b", "key_5": 9, "space_6": 0, "const_7": "0x40", "key_8": "1100001"}
{"message": "'abcd\"def'", "number": 145432} -> {"key_1": 581728, "key_2": "14%", "key_3": 123445, "key_4": "b", "key_5": 10, "space_6": 0, "const_7": "0x40", "key_8": "100111"}
{"message": "'abcd\"de\nf'", "number": 145432} -> {"key_1": 581728, "key_2": "14%", "key_3": 123445, "key_4": "a", "key_5": 11, "space_6": 0, "const_7": "0x40", "key_8": "100111"}
{"message": "'abcd\"de\n\tf'", "number": 145432} -> {"key_1": 581728, "key_2": "14%", "key_3": 123445, "key_4": "d", "key_5": 12, "space_6": 0, "const_7": "0x40", "key_8": "100111"}
{"message": "\t'abcd\"de\n\tf'", "number": 145432} -> {"key_1": 581728, "key_2": "14%", "key_3": 123445, "key_4": "'", "key_5": 13, "space_6": 0, "const_7": "0x40", "key_8": "1001"}
{"message": " ", "number": 145432} -> {"key_1": 581728, "key_2": "14%", "key_3": 123445, "key_4": " ", "key_5": 1, "space_6": 1, "const_7": "0x40", "key_8": "100000"}
{"message": " ", "number": 145432, "key": 4} -> {"key_1": 581728, "key_2": "14%", "key_3": 123445, "key_4": " ", "key_5": 1, "space_6": 1, "const_7": "0x40", "key_8": "100000"}
{"message": "'abcd\"De\nf'", "number": 1024} -> {"key_1": 4096, "key_2": "10%", "key_3": 124, "key_4": "a", "key_5": 11, "space_6": 0, "const_7": "0x40", "key_8": "100111"}
{"message": "'abяcd\"De\nf'", "number": 1024} -> {"key_1": 4096, "key_2": "10%", "key_3": 124, "key_4": "
", "key_5": 13, "space_6": 0, "const_7": "0x40", "key_8": "100111"}
{"message": "абв", "number": 1024} -> {"key_1": 4096, "key_2": "10%", "key_3": 124, "key_4": "Ð", "key_5": 6, "space_6": 0, "const_7": "0x40", "key_8": "11010000"}
{"message": "б", "number": 1024} -> {"key_1": 4096, "key_2": "10%", "key_3": 124, "key_4": "Ð", "key_5": 2, "space_6": 0, "const_7": "0x40", "key_8": "11010000"}
{"message": "в", "number": 1024} -> {"key_1": 4096, "key_2": "10%", "key_3": 124, "key_4": "Ð", "key_5": 2, "space_6": 0, "const_7": "0x40", "key_8": "11010000"}
{"message": "bб", "number": 1001} -> {"key_1": 4004, "key_2": "10%", "key_3": 11, "key_4": "±", "key_5": 3, "space_6": 0, "const_7": "0x40", "key_8": "1100010"}
{"message": "bб", "number": 1002} -> {"key_1": 4008, "key_2": "10%", "key_3": 12, "key_4": "b", "key_5": 3, "space_6": 0, "const_7": "0x40", "key_8": "1100010"}
{"message": "bб", "number": 1003} -> {"key_1": 4012, "key_2": "10%", "key_3": 13, "key_4": "Ð", "key_5": 3, "space_6": 0, "const_7": "0x40", "key_8": "1100010"}

*/
        private static string GetAnswer(string message, long number)
        {
            const string pattern =
                @"{{""key_1"": {0}, ""key_2"": ""{1}"", ""key_3"": {2}, ""key_4"": ""{3}"", ""key_5"": {4}, ""space_6"": {5}, ""const_7"": ""{6}"", ""key_8"": ""{7}""}}";
            
            var messageBytes = Encoding.UTF8.GetBytes(message);
            
            var key1 = 4 * number;
            var key2 = number.ToString().Substring(0, 2) + "%";
            var key3 = string.Join("", number.ToString().Where(it => it != '0').OrderBy(it => it));
            var key4 = Convert.ToChar(messageBytes[(int)(number % messageBytes.Length)]);
            var key5 = messageBytes.Length;
            var space6 = message.Count(it => it == ' ');
            var const7 = "0x40";
            var key8 = GetBits(messageBytes[0]);

            var answer = string.Format(pattern, key1, key2, key3, key4, key5, space6, const7, key8);
            // Console.WriteLine(answer);
            var bytes = Encoding.UTF8.GetBytes(answer);
            return Convert.ToBase64String(bytes);
            Encoding.UTF7.GetString(new byte[1]);
        }

        private static string GetBits(byte b)
        {
            var bits = new List<char>(8);
            for (; b > 0; b >>= 1)
                bits.Add((b & 1) == 1 ? '1' : '0');

            bits.Reverse();
            return new string(bits.ToArray());
        }
        
        private sealed class Request
        {
            [JsonPropertyName("message")]
            public string Message { get; set; }
            
            [JsonPropertyName("number")]
            public long Number { get; set; }
        }
    }
}