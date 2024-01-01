using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Day01
{
    internal class Program
    {
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day01\Sample.txt";
        private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day01\Full.txt";

        private static string[] digitNames = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(fileName);

            Console.WriteLine("Part1: " + Part1(lines));
            Console.WriteLine("Part2: " + Part2(lines));
            Console.ReadLine();
        }

        private static long Part1(string[] input)
        {
            return input.Sum(l => ProcessLine(l, useNames: false));
        }
        private static long Part2(string[] input)
        {
            return input.Sum(l => ProcessLine(l, useNames: true));
        }

        private static int ProcessLine(string line, bool useNames=false)
        {
            string regEx = useNames ? "|" + string.Join("|", digitNames) : "";
            regEx = @"(?=(\d" + regEx + "))";
            var matches = Regex.Matches(line, regEx);
            
            string first = matches.First().Groups[1].Value;
            string last = matches.Last().Groups[1].Value;

            first = TranslateDigit(first);
            last = TranslateDigit(last);

            string combined = first + last;
            return int.Parse(combined);
        }

        private static string TranslateDigit(string s)
        {
            int index = Array.IndexOf(digitNames, s);
            if (index >= 0)
            {
                return (index + 1).ToString();
            }
            else
            {
                return s;
            }
        }
    }
}
