using System.Text.RegularExpressions;
using Map = System.Collections.Generic.Dictionary<string, (string Left, string Right)>;

namespace Day08
{
    internal class Program
    {
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day08\Sample.txt";
        private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day08\Full.txt";

        static void Main()
        {
            var lines = File.ReadAllLines(fileName);
            var directions = lines.First();
            Map map = lines.Skip(2).Select(ParseMapLine).ToDictionary();


            Console.WriteLine("Part 1:" + Part1(directions, map));
            Console.WriteLine("Part 2:" + Part2(directions, map));

            Console.ReadLine();
        }

        static long Part1(string directions, Map map)
        {
            return FindSteps("AAA", directions, map, "ZZZ");

        }
        static long Part2(string directions, Map map)
        {
            var startPositions = map.Keys.Where(s => s.EndsWith('A'));
            var steps = startPositions.Select(p => FindSteps(p, directions, map, "Z"));
            return steps.Aggregate(LCM);
        }

        static long FindSteps(string start, string directions, Map map, string endsWith)
        {
            string position = start;
            int steps = 0;
            int directionsIndex = 0;

            while (!position.EndsWith(endsWith))
            {
                position = directions[directionsIndex] == 'L' ? map[position].Left : map[position].Right;

                steps++;
                directionsIndex++;

                if (directionsIndex == directions.Length)
                {
                    directionsIndex = 0;
                }
            }

            return steps;
        }

        private static (string Source, (string Left, string Right) Target) ParseMapLine(string mapLine)
        {
            var matches = Regex.Matches(mapLine, "[A-Z0-9]{3}");

            if(matches.Count == 3)
            {
                return (matches[0].Value, (matches[1].Value, matches[2].Value));
            }

            throw new Exception("Invalid Input");
        }
        
        static long LCM(long a, long b)
        {
            return Math.Abs(a * b) / GCD(a, b);
        }
        
        static long GCD(long a, long b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }
    }
}
