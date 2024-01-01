using System.Text.RegularExpressions;

namespace Day12
{
    internal class Program
    {
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day12\Sample.txt";
        private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day12\Full.txt";

        static void Main()
        {
            string[] lines = File.ReadAllLines(fileName);

            long sum1 = 0;
            foreach (var line in lines)
            {
                sum1 += ProcessLine(line, unfold: false);
            }

            Console.WriteLine("Part 1: " + sum1);

            long sum2 = 0;
            foreach (var line in lines)
            {
                sum2 += ProcessLine(line, unfold: true);
            }

            Console.WriteLine("Part 2: " + sum2);

            Console.ReadLine();
        }

        private static long ProcessLine(string line, bool unfold)
        {
            string[] splitted = line.Split(' ');
            string springs = splitted[0];
            int[] damagedGroupLengths = 
                Regex.Matches(splitted[1], @"\d+")
                .Select(m => int.Parse(m.Value))
                .ToArray();

            Dictionary<(int inputPosition, int currentGroupLength, int currentOrNextGroupPosition), long> cache = new();

            if (unfold)
            {
                string unfoldedSprings = string.Join('?', Enumerable.Repeat(springs, 5));
                int[] unfoldedDamagedGroupLengths = Enumerable.Repeat(damagedGroupLengths, 5).SelectMany(s => s).ToArray();

                unfoldedSprings +=  "."; // Always end out of group to make state machine easier

                return ParseAndCount(unfoldedSprings, 0, 0, 0, unfoldedDamagedGroupLengths, cache);
            }
            else 
            {
                springs += "."; // Always end out of group to make state machine easier
                
                return ParseAndCount(springs, 0, 0, 0, damagedGroupLengths, cache);
            }
        }

        private static long ParseAndCount(string input, int inputPosition, int currentGroupLength, int currentOrNextGroupPosition, int[] damagedGroupLengths,
            Dictionary<(int inputPosition, int currentGroupLength, int currentOrNextGroupPosition), long> cache)
        {
            
            if(cache.TryGetValue((inputPosition, currentGroupLength, currentOrNextGroupPosition), out var cached)) return cached;

            long result;
            
            // Use zero as token for End of Input
            char token = inputPosition < input.Length ? input[inputPosition] : '\0';

            // Filter ? here and branch
            if (token == '?')
            {
                result = HandleToken('.', input, inputPosition, currentGroupLength, currentOrNextGroupPosition, damagedGroupLengths, cache)
                    + HandleToken('#', input, inputPosition, currentGroupLength, currentOrNextGroupPosition, damagedGroupLengths, cache);
            }
            else
            {
                result =  HandleToken(token, input, inputPosition, currentGroupLength, currentOrNextGroupPosition, damagedGroupLengths, cache);
            }

            cache.Add((inputPosition, currentGroupLength, currentOrNextGroupPosition), result);

            return result;
        }

        private static long HandleToken(char token, string input, int inputPosition, int currentGroupLength, int currentOrNextGroupPosition, int[] damagedGroupLengths,
            Dictionary<(int inputPosition, int currentGroupLength, int currentOrNextGroupPosition), long> cache)
        {
            switch (token)
            {
                case '.':
                    if (currentGroupLength == 0)
                    {
                        // Intact outside damaged group, just continue scanning
                        return ParseAndCount(input, inputPosition + 1, currentGroupLength, currentOrNextGroupPosition, damagedGroupLengths, cache);
                    }
                    else
                    {
                        // closing group

                        // When invalid group length, stop
                        if (currentGroupLength != damagedGroupLengths[currentOrNextGroupPosition]) return 0;

                        // else look for next group
                        return ParseAndCount(input, inputPosition + 1, 0, currentOrNextGroupPosition + 1, damagedGroupLengths, cache);
                    }
                case '#':
                    if (currentGroupLength != 0)
                    {
                        // Damaged inside damaged group, continue scanning if possible
                        
                        // Check if group longer than allowed
                        if (currentGroupLength + 1 > damagedGroupLengths[currentOrNextGroupPosition]) return 0;

                        return ParseAndCount(input, inputPosition + 1, currentGroupLength + 1, currentOrNextGroupPosition, damagedGroupLengths, cache);
                    }
                    else
                    {
                        // starting new group
                        if (currentOrNextGroupPosition >= damagedGroupLengths.Length) return 0;

                        return ParseAndCount(input, inputPosition + 1, 1, currentOrNextGroupPosition, damagedGroupLengths, cache);
                    }
                case '?':
                    throw new ApplicationException("? not expected here");

                case '\0':
                    if (currentGroupLength != 0) throw new ApplicationException("Unexpected End of Input within group");
                    
                    // Check there are no missing groups
                    if (currentOrNextGroupPosition < damagedGroupLengths.Length) return 0;
                    
                    return 1;
                default: 
                    throw new ApplicationException("Unknown token " + token);
            }
        }
    }
}
