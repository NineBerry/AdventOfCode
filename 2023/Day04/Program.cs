using System.Text.RegularExpressions;

namespace Day04
{
    internal class Program
    {
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day04\Sample.txt";
        private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day04\Full.txt";

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
            foreach (string line in lines)
            {
                int points = 1 << (MatchingNumbers(line) - 1);
                sum += points;
            }

            return sum;
        }

        private static int Part2(string[] lines)
        {
            int[] cardCounts = new int[lines.Length];


            for (int i = 0; i < lines.Length; i++)
            {
                cardCounts[i]++;
                string line = lines[i];

                int points = MatchingNumbers(line);

                for (int offset = 1; offset <= points; offset++)
                {
                    cardCounts[i + offset] += cardCounts[i];
                }
            }

            int sum = cardCounts.Sum();
            return sum;
        }

        private static int MatchingNumbers(string line)
        {
            line = Regex.Replace(line, ".*:", "");

            var parts = line.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            HashSet<int> winning = GetNumbers(parts[0]);
            HashSet<int> having = GetNumbers(parts[1]);

            var wins = winning.Intersect(having);
            int winCounts = wins.Count();

            return winCounts;
        }

        private static HashSet<int> GetNumbers(string text)
        {
            var matches = Regex.Matches(text, @"\d+");
            return matches.Select(s => int.Parse(s.Value)).ToHashSet();
        }
    }
}
