using System.Text.RegularExpressions;

namespace Day09
{
    internal class Program
    {
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day09\Sample.txt";
        private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day09\Full.txt";

        static void Main()
        {
            var lines = File.ReadAllLines(fileName);

            long sumPrev = 0, sumNext = 0;
            foreach (var line in lines)
            {
                var (prev, next) =  ProcessLine(line.ToLowerInvariant());
                sumPrev += prev;
                sumNext += next;
            }

            Console.WriteLine("Result 1: " + sumNext);
            Console.WriteLine("Result 2: " + sumPrev);
            Console.ReadLine();
        }

        private static (long Prev, long Next) ProcessLine(string line)
        {
            long[] values = Regex.Matches(line, @"-?\d+").Select(m => long.Parse(m.Value)).ToArray();

            return Step(values);
        }

        private static (long Prev, long Next) Step(long[] values)
        {
            if (values is []) throw new ArgumentException("Empty Array");

            if (values.All(v => v == 0)) return (0, 0);

            if (values is [_]) throw new ArgumentException("Array with Length 1");

            long[] differences = GetDifferences(values);

            var (innerPrev, innerNext) = Step(differences);

            return (
                Prev: values.First() - innerPrev, 
                Next: values.Last() + innerNext);
        }

        private static long[] GetDifferences(long[] values)
        {
            List<long> differences = new List<long>();

            for (int i = 0; i < (values.Length - 1); i++)
            {
                differences.Add(values[i + 1] - values[i]);
            }

            return differences.ToArray();
        }
    }
}
