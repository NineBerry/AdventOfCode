namespace Day14
{
    internal class Program
    {
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day14\Sample.txt";
        private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day14\Full.txt";

        static void Main()
        {
            // Initialize game grid
            string[] lines = GetLines(fileName);

            // Part 1
            Grid grid = BuildGrid(lines);
            grid.TiltGrid(Direction.North);
            long totalLoad = grid.CalculateTotalLoad();
            Console.WriteLine("Part 1: " + totalLoad);

            DateTime start = DateTime.Now;

            // Part 2
            // Get fresh version of rocks list
            Grid grid2 = BuildGrid(lines);
            IterateSpinCycle(grid2, 1_000_000_000);

            var totalLoad2 = grid2.CalculateTotalLoad();
            Console.WriteLine("Part 2: " + totalLoad2);

            DateTime end = DateTime.Now;
            var duration = (end - start).TotalMilliseconds;

            Console.WriteLine(duration + " ms");

            Console.ReadLine();
        }

        private static Grid BuildGrid(string[] lines)
        {
            Grid grid = new Grid { Height = lines.Length, Width = lines.First().Length };

            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    char tile = lines[y][x];

                    if (tile == Rock.FixedRockChar) grid.AddRock(new Rock { Movable = false, X = x, Y = y });
                    if (tile == Rock.MovingRockChar) grid.AddRock(new Rock { Movable = true, X = x, Y = y });
                }
            }

            return grid;
        }

        private static void IterateSpinCycle(Grid grid, int iterationsToRun)
        {
            Dictionary<string, int> seenGrids = new();

            for (int iteration = 0; iteration < iterationsToRun; iteration++)
            {
                string before = grid.ToString();

                if (seenGrids.TryGetValue(before, out int foundIteration))
                {
                    // We found a loop
                    int loopLength = iteration - foundIteration;
                    int targetOffSetWithinLoop = (iterationsToRun - iteration) % loopLength;

                    IterateSpinCycle(grid, targetOffSetWithinLoop);
                    break;

                    // Alternative: 
                    // int wantedCycle = foundIteration + targetOffSetWithinLoop;
                    // seenGrids.Where(p => p.Value == wantedCycle);
                    // But then we need to rebuild this to return the found grid
                }

                seenGrids.Add(before, iteration);
                SpinCycle(grid);
            }
        }

        private static void SpinCycle(Grid grid)
        {
            grid.TiltGrid(Direction.North);
            grid.TiltGrid(Direction.West);
            grid.TiltGrid(Direction.South);
            grid.TiltGrid(Direction.East);
        }

        private static string[] GetLines(string fileName)
        {
            return File.ReadAllLines(fileName);
        }
    }

    public record struct Rock
    {
        public Rock() { }

        public static readonly char MovingRockChar = 'O';
        public static readonly char FixedRockChar = '#';

        public bool Movable { get; init; }

        public int X { get; set; }
        public int Y { get; set; }

        public char AsChar => Movable ? MovingRockChar : FixedRockChar;
    }

    public class Grid
    {
        private List<Rock> rocks = new List<Rock>();

        public IEnumerable<Rock> Rocks => rocks;

        public int Height { get; init; }
        public int Width { get; init; }

        public void AddRock(Rock rock) => rocks.Add(rock);

        public void TiltGrid(Direction direction)
        {
            List<Rock> newRocks = new List<Rock>();

            switch (direction)
            {
                case Direction.North:
                    for (int col = 0; col < Width; col++)
                    {
                        var rocksInCol = rocks.Where(r => r.X == col).ToArray().OrderBy(r => r.Y);
                        var movedRocks = MoveRocksAsFarAsPossible(rocksInCol, modifyX: false, direction: +1, Height);
                        newRocks.AddRange(movedRocks);
                    }
                    break;
                case Direction.South:
                    for (int col = 0; col < Width; col++)
                    {
                        var rocksInCol = rocks.Where(r => r.X == col).ToArray().OrderByDescending(r => r.Y);
                        var movedRocks = MoveRocksAsFarAsPossible(rocksInCol, modifyX: false, direction: -1, Height);
                        newRocks.AddRange(movedRocks);
                    }
                    break;
                case Direction.West:
                    for (int row = 0; row < Width; row++)
                    {
                        var rocksInRow = rocks.Where(r => r.Y == row).ToArray().OrderBy(r => r.X);
                        var movedRocks = MoveRocksAsFarAsPossible(rocksInRow, modifyX: true, direction: +1, Width);
                        newRocks.AddRange(movedRocks);
                    }
                    break;
                case Direction.East:
                    for (int row = 0; row < Width; row++)
                    {
                        var rocksInRow = rocks.Where(r => r.Y == row).ToArray().OrderByDescending(r => r.X);
                        var movedRocks = MoveRocksAsFarAsPossible(rocksInRow, modifyX: true, direction: -1, Width);
                        newRocks.AddRange(movedRocks);
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid direction");
            }

            rocks = newRocks;
        }

        private static IEnumerable<Rock> MoveRocksAsFarAsPossible(IEnumerable<Rock> rocks, bool modifyX, int direction, int length)
        {
            int nextEmptyPosition = direction > 0 ? 0 : length - 1;
            List<Rock> result = new List<Rock>();

            foreach (Rock rock in rocks)
            {
                if (rock.Movable)
                {
                    Rock movedRock = modifyX ? rock with { X = nextEmptyPosition } : rock with { Y = nextEmptyPosition };
                    nextEmptyPosition += direction;
                    result.Add(movedRock);
                }
                else
                {
                    nextEmptyPosition = modifyX ? rock.X + direction : rock.Y + direction;
                    result.Add(rock);
                }
            }

            return result;
        }

        public long CalculateTotalLoad() =>
            Rocks
            .Where(r => r.Movable)
            .Sum(r => Height - r.Y);


        private char[] emptyMap = null;

        public override string ToString()
        {
            if (emptyMap == null)
            {
                string line = new string(Enumerable.Repeat('.', Width).ToArray());
                emptyMap = string.Join('\n', Enumerable.Range(0, Height).Select(s => line)).ToCharArray();
            }

            char[] map = (char[])emptyMap.Clone();

            foreach (var rock in Rocks)
            {
                // +1 for line break
                map[rock.Y * (Width + 1) + rock.X] = rock.AsChar;
            }

            return new string(map);
        }
    }

    public enum Direction
    {
        None,
        South,
        West,
        North,
        East
    }
}