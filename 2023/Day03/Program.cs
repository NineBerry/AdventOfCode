using System.Text.RegularExpressions;

namespace Day03
{
    internal class Program
    {
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day03\Sample.txt";
        private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day03\Full.txt";

        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(fileName);

            Console.WriteLine("Part 1: " + Part1(lines));
            Console.WriteLine("Part 2: " + Part2(lines));
            Console.ReadLine();
        }

        private static int Part1(string[] lines)
        {
            int sum = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string prevLine = i > 0 ? lines[i - 1] : "";
                string nextLine = (i < lines.Length - 1) ? lines[i + 1] : "";
                string line = lines[i];

                sum += ProcessLinePart1(line, prevLine, nextLine);
            }

            return sum;
        }

        private static int ProcessLinePart1(string line, string prevLine, string nextLine)
        {
            int sum = 0;
            var matches = Regex.Matches(line, @"\d+");

            foreach (Match match in matches)
            {
                int startInclusive = Math.Max(0, match.Index - 1);
                int endInclusive = Math.Min(match.Index + match.Length, line.Length - 1);

                bool hasSymbol = HasSymbol(line, startInclusive, endInclusive)
                    || HasSymbol(prevLine, startInclusive, endInclusive)
                    || HasSymbol(nextLine, startInclusive, endInclusive);

                if (hasSymbol)
                {
                    sum += int.Parse(match.Value);
                }
            }

            return sum;
        }


        private static int Part2(string[] lines)
        {
            int sum = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string prevLine = i > 0 ? lines[i - 1] : "";
                string nextLine = (i < lines.Length - 1) ? lines[i + 1] : "";
                string line = lines[i];

                sum += ProcessLinePart2(line, prevLine, nextLine);
            }

            return sum;
        }

        private static int ProcessLinePart2(string line, string prevLine, string nextLine)
        {
            int sum = 0;

            var matches = Regex.Matches(line, @"\*");
            foreach (Match match in matches)
            {
                int index = match.Index;

                List<int> adjacent = new List<int>();

                adjacent.AddRange(GetAdjacentNumbers(line, index));
                adjacent.AddRange(GetAdjacentNumbers(prevLine, index));
                adjacent.AddRange(GetAdjacentNumbers(nextLine, index));

                if (adjacent.Count == 2)
                {
                    sum += adjacent[0] * adjacent[1];
                }
            }

            return sum;
        }

        private static int[] GetAdjacentNumbers(string line, int index)
        {
            List<int> result = new List<int>();

            var matches = Regex.Matches(line, @"\d+");

            foreach (Match match in matches)
            {
                int start = match.Index;
                int end = match.Index + match.Length - 1;

                if ((start == index + 1)    // Number starts directly after index
                    || (end == index - 1)   // Number ends directly after index
                    || (start <= index && end >= index)) // Number contains index
                    {
                    result.Add(int.Parse(match.Value));
                }
            }

            return result.ToArray();
        }

        private static bool HasSymbol(string s, int startInclusive, int endInclusive)
        {
            if(s == "") return false;

            string sub = s.Substring(startInclusive, endInclusive - startInclusive + 1);
            return HasSymbol(sub);
        }

        private static bool HasSymbol(string s)
        {
            return s.Any(IsSymbol);
        }

        private static bool IsSymbol(char ch)
        {
            if (ch == '.') return false;
            if (Char.IsAsciiDigit(ch)) return false;

            return true;
        }
    }
}
