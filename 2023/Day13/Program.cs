using Map = System.Collections.Generic.List<string>;

namespace Day13
{
    internal class Program
    {
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day13\Sample.txt";
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day13\Sample2.txt";
        private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day13\Full.txt";

        static void Main()
        {
            string[] lines = File.ReadAllLines(fileName);
            List<Map> maps = ParseMaps(lines).ToList();

            long sum1 = maps
                .Select(ProcessMapWithMirror)
                .Sum();
            
            Console.WriteLine("Part 1: " + sum1);

            long sum2 = maps
                .Select(ProcessMapWithSmudgeMirror)
                .Sum();
            
            Console.WriteLine("Part 2: " + sum2);

            Console.ReadLine();
        }

        #region Part 1

        private static int ProcessMapWithMirror(Map map)
        {
            return 100 * GetTopRowsAboveMirror(map) 
                + GetTopRowsAboveMirror(RotateMapClockWise(map));
        }

        private static int GetTopRowsAboveMirror(Map map)
        {
            IEnumerable<int> candidateRows = 
                map[..^1].Zip(map[1..])
                .Select((pair, index) => pair.First == pair.Second ? index : -1)
                .Where(i => i != -1);

            int mirror = candidateRows
                .FirstOrDefault(row => CheckIsMirrorAtRow(map, row), defaultValue: -1);

            return mirror + 1;
        }

        private static bool CheckIsMirrorAtRow(Map map, int mirrorRow)
        {
            int rowsToCheck = Math.Min(map.Count - (mirrorRow + 1), mirrorRow + 1);
            
            return Enumerable.Range(0, rowsToCheck)
                .All(i => map[mirrorRow - i] == map[mirrorRow + 1 + i]);
        }

        #endregion

        #region Part 2

        private static int ProcessMapWithSmudgeMirror(Map map)
        {
            return 100 * GetTopRowsAboveSmudgeMirror(map)
                + GetTopRowsAboveSmudgeMirror(RotateMapClockWise(map));
        }

        private static int GetTopRowsAboveSmudgeMirror(Map map)
        {
            IEnumerable<int> candidateRows = 
                map[..^1].Zip(map[1..])
                .Select((pair, index) => CountDifferences(pair.First, pair.Second) <= 1  ? index : -1)
                .Where(i => i != -1);

            int mirror = candidateRows.FirstOrDefault(row => CheckSmudgeMirrorAtRow(map, row), defaultValue: -1);

            return mirror + 1;
        }

        private static bool CheckSmudgeMirrorAtRow(Map map, int mirrorRow)
        {
            int rowsToCheck = Math.Min(map.Count - (mirrorRow + 1), mirrorRow + 1);

            bool mirrorFound = true;
            int smudges = 0;

            for (int i = 0; i < rowsToCheck; i++)
            {
                string aboveRow = map[mirrorRow - i];
                string belowRow = map[mirrorRow + 1 + i];

                int differences = CountDifferences(aboveRow, belowRow);
                
                if (differences > 1)
                {
                    mirrorFound = false;
                    break;
                }
                else
                {
                    smudges += differences;
                    if (smudges > 1) break;
                }
            }

            return mirrorFound && smudges == 1;
        }

        private static int CountDifferences(string first, string second)
        {
            if (first.Length != second.Length) throw new ArgumentException("String supposed to be equal length");

            return first.Zip(second)
                .Count(pair => pair.First != pair.Second);
        }


        #endregion

        #region Helper functions

        private static IEnumerable<Map> ParseMaps(string[] lines)
        {
            Map map = new Map();

            foreach (string line in lines)
            {
                if (line == "")
                {
                    yield return map;
                    map = new Map();
                }
                else
                {
                    map.Add(line);
                }
            }

            if (map.Count > 0) yield return map;
        }

        private static Map RotateMapClockWise(Map map)
        {
            string firstRow = map.First();

            var rotatedMap = firstRow.Select(
                    (column, columnIndex) => string.Join("", map.Select(row => row[columnIndex]))
                ).ToList();

            return rotatedMap;
        }

        #endregion
    }
}
