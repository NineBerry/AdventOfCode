using System.Text.RegularExpressions;

namespace Day05
{
    internal class Program
    {
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day05\Sample.txt";
        private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day05\Full.txt";

        static void Main()
        {
            IEnumerable<string> lines = File.ReadAllLines(fileName);

            List<long> seeds = ReadNumbers(lines.First());
            List<Map> maps = ReadMaps(lines.Skip(2));

            Console.WriteLine("Part 1: " + Part1(seeds, maps));
            Console.WriteLine("Part 2: " + Part2(seeds, maps));
            Console.ReadLine();
        }

        private static long Part1(List<long> seeds, List<Map> maps)
        {
            HashSet<long> locations = new();

            foreach (long seed in seeds)
            {
                long value = seed;
                foreach (Map map in maps)
                {
                    value = map.Translate(value);
                }

                locations.Add(value);
            }

            return locations.Min();
        }

        /// <summary>
        /// Doing a reverse brute force. This is faster than
        /// brute forcing ahead but a solution using ranges
        /// would be a lot faster still.
        /// 
        /// Reverse: We need   60294664 iterations
        /// Ahead:   We need 2750967230 iterations
        /// </summary>
        private static long Part2(List<long> seeds, List<Map> maps)
        {
            long location = 1;

            List<Map> reverseMaps = (from m in maps select m).Reverse().ToList();

            while (true)
            {
                if (location % 1000000 == 0) Console.Write("\r" + location);
                long value = location;
                foreach (Map map in reverseMaps)
                {
                    value = map.ReverseTranslate(value);
                }

                if (ValueInRanges(value, seeds))
                {
                    Console.Write("\r");
                    break;
                }

                location++;
            }

            return location;
        }


        private static bool ValueInRanges(long value, List<long> seeds)
        {
            bool result = false;

            for (int i = 0; i < seeds.Count; i+= 2) 
            { 
                long start = seeds[i];
                long range = seeds[i+1];    

                if((value >= start) && (value < start + range))
                {
                    return true;
                }
            }

            return result;
        }

        private static List<Map> ReadMaps(IEnumerable<string> lines)
        {
            Map? currentMap = null;
            List<Map> list = new List<Map>();

            foreach (string line in lines)
            {
                if(line == "") continue;

                if (line.EndsWith(":"))
                {
                    currentMap = new Map();
                    list.Add(currentMap);
                }
                else
                {
                    var numbers = ReadNumbers(line);
                    currentMap?.AddRule(numbers[0], numbers[1], numbers[2]);
                }
            }

            return list;
        }

        private static List<long> ReadNumbers(string line)
        {
            return Regex.Matches(line, @"\d+").Select(m => m.Value).Select(s => long.Parse(s)).ToList();

        }

        public class Map
        {
            private List<(long toStart, long fromStart, long length)> rules = new();
            
            public long Translate(long from)
            {
                foreach (var rule in rules)
                {
                    if(from >= rule.fromStart && from <= rule.fromStart + rule.length - 1)
                    {
                        return rule.toStart + (from - rule.fromStart);
                    }
                }

                return from;
            }

            public long ReverseTranslate(long to)
            {
                foreach (var rule in rules)
                {
                    if (to >= rule.toStart && to <= rule.toStart + rule.length - 1)
                    {
                        return rule.fromStart + (to - rule.toStart);
                    }
                }

                return to;
            }

            public void AddRule(long toStart, long fromStart, long length)
            {
                rules.Add((toStart, fromStart, length));
            }
        }
    }
}
