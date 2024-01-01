using System.Text.RegularExpressions;
using static Day07.CamelCardsGame;

namespace Day07
{
    internal class Program
    {
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day07\Sample.txt";
        private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day07\Full.txt";

        static void Main()
        {
            string[] lines = File.ReadAllLines(fileName);

            Console.WriteLine("Part 1: " + Part1(lines));
            Console.WriteLine("Part 2: " + Part2(lines));
            Console.ReadLine();
        }

        private static long Part1(string[] lines)
        {
            return PlayGame(lines, false);
        }
        private static long Part2(string[] lines)
        {
            return PlayGame(lines, true);
        }

        private static long PlayGame(string[] input, bool useJoker)
        {
            List<Hand> hands = input.Select(line => new Hand(line, useJoker)).ToList();
            hands.Sort(new HandComparer());

            int rank = 1;
            long sum = 0;

            foreach (var hand in hands)
            {
                sum += rank * hand.Bid;
                rank++;
            }

            return sum;
        }
    }
}
