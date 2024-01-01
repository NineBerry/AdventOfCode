namespace Day11
{
    internal class Program
    {
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day11\Sample.txt";
        private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day11\Full.txt";

        private record struct Point(int X, int Y);

        static void Main()
        {
            string[] lines = GetLines(fileName);

            // Find all galaxies
            List<Point> galaxies = new ();
            for (int y = 0;  y < lines.Length; y++) { 
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] == '#')
                    {
                        galaxies.Add (new Point (x, y));
                    }
                }
            }

            // Get rows and columns that contain galaxies
            var rowsWithGalaxies = galaxies.Select(g => g.Y).ToHashSet();
            var columnsWithGalaxies = galaxies.Select(g => g.X).ToHashSet();

            var sum1 = CalculateDistance(galaxies, rowsWithGalaxies, columnsWithGalaxies, 2);
            Console.WriteLine("Part 1: " + sum1);
            
            var sum2 = CalculateDistance(galaxies, rowsWithGalaxies, columnsWithGalaxies, 1_000_000);
            Console.WriteLine("Part 2: " + sum2);
            
            Console.ReadLine();
        }

        /// <summary>
        /// Calculate distance between pairs of galaxies including growth in empty space
        /// </summary>
        private static long CalculateDistance(IEnumerable<Point> galaxies, HashSet<int> rowsWithGalaxies, HashSet<int> columnsWithGalaxies, int expansionRate)
        {
            var pairingGalaxies = new List<Point>(galaxies);

            long sum = 0;

            while (pairingGalaxies.Any())
            {
                Point galaxy1 = pairingGalaxies.Last();
                pairingGalaxies.Remove(galaxy1);

                foreach (Point galaxy2 in pairingGalaxies)
                {
                    sum += CalculateDistance(galaxy1, galaxy2, rowsWithGalaxies, columnsWithGalaxies, expansionRate);
                }
            }

            return sum;
        }

        /// <summary>
        /// Calculate distance between two galaxies including growth in empty space
        /// </summary>
        private static long CalculateDistance(Point galaxy1, Point galaxy2, HashSet<int> rowsWithGalaxies, HashSet<int> columnsWithGalaxies, int expansionRate)
        {
            long sum = CalculateDistance(galaxy1.X, galaxy2.X, columnsWithGalaxies, expansionRate) 
                + CalculateDistance(galaxy1.Y, galaxy2.Y, rowsWithGalaxies, expansionRate);
            return sum;
        }

        /// <summary>
        /// Calculate distance between two coordinate elements including growth in empty space
        /// </summary>
        private static long CalculateDistance(int value1, int value2, HashSet<int> valuesWithGalaxies, int expansionRate)
        {
            int min = Math.Min(value1, value2);
            int max = Math.Max(value1, value2);

            long sum = 0;

            for (int i = min;  i < max; i++) 
            {
                if (!valuesWithGalaxies.Contains(i))
                {
                    // Grow empty space
                    sum += expansionRate;
                }
                else
                {
                    // Non-Empty space, normal difference is 1
                    sum++;
                }
            }

            return sum;
        }

        private static string[] GetLines(string fileName)
        {
            return File.ReadAllLines(fileName);

        }


    }
}
