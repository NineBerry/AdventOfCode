namespace Day14
{
    internal class Program
    {
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day14\Sample.txt";
        private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day14\Full.txt";

        static void Main()
        {
            // Initialize game grid
            string lines = GetLines(fileName);

            // Part 1
            Grid grid = Grid.FromString(lines);
            grid.TiltGrid(Direction.North);
            long totalLoad = grid.CalculateTotalLoad();
            Console.WriteLine("Part 1: " + totalLoad);

            // Part 2
            // Get fresh version of rocks list
            Grid grid2 = Grid.FromString(lines);
            IterateSpinCycle(ref grid2, 1_000_000_000);
            var totalLoad2 = grid2.CalculateTotalLoad();
            Console.WriteLine("Part 2: " + totalLoad2);

            Console.ReadLine();
        }

        private static void IterateSpinCycle(ref Grid grid, int iterationsToRun)
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

                    // Find result from previous cycle
                    int wantedCycle = foundIteration + targetOffSetWithinLoop;
                    grid = Grid.FromString(seenGrids.Where(p => p.Value == wantedCycle).First().Key);

                    return;
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

        private static string GetLines(string fileName)
        {
            return File.ReadAllText(fileName);
        }
    }

    public record class Rock
    {
        public Rock() { }

        public static readonly char MovingRockChar = 'O';
        public static readonly char FixedRockChar = '#';

        public bool Movable { get; init; }

        public int X;
        public int Y;

        public char AsChar => Movable ? MovingRockChar : FixedRockChar;
    }

    public class Grid
    {
        public Grid(int width, int height, IEnumerable<Rock> knownRocks)
        {
            Width = width;
            Height = height;

            Rocks = new Rock[height, width];

            AddKnownRocks(Rocks, knownRocks);
        }

        public static Grid FromString(string text)
        {
            string[] lines = text.ReplaceLineEndings().Split(Environment.NewLine).ToArray();
            
            List<Rock> rocks = new();

            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    char tile = lines[y][x];

                    if (tile == Rock.FixedRockChar) rocks.Add(new Rock { Movable = false, X = x, Y = y });
                    if (tile == Rock.MovingRockChar) rocks.Add(new Rock { Movable = true, X = x, Y = y });
                }
            }

            return new Grid(lines.First().Length, lines.Length, rocks);
        }

        private void AddKnownRocks(Rock[,] rocks, IEnumerable<Rock> knownRocks)
        {
            foreach (var rock in knownRocks)
            {
                rocks[rock.Y, rock.X] = rock;
            }
        }

        public Rock[,] Rocks { get; private set; }

        public int Height { get; private set; }
        public int Width { get; private set; }

        public void TiltGrid(Direction direction)
        {
            Rock[,] newRocks = new Rock[Height, Width];

            switch (direction)
            {
                case Direction.North:
                    for (int col = 0; col < Width; col++)
                    {
                        var rocksInCol = GetColumn(Rocks, col);
                        var movedRocks = MoveRocksAsFarAsPossible(rocksInCol, modifyX: false, direction: +1, Height);
                        AddKnownRocks(newRocks, movedRocks);
                    }
                    break;
                case Direction.South:
                    for (int col = 0; col < Width; col++)
                    {
                        var rocksInCol = GetColumn(Rocks, col).Reverse();
                        var movedRocks = MoveRocksAsFarAsPossible(rocksInCol, modifyX: false, direction: -1, Height);
                        AddKnownRocks(newRocks, movedRocks);
                    }
                    break;
                case Direction.West:
                    for (int row = 0; row < Width; row++)
                    {
                        var rocksInRow = GetRow(Rocks, row);
                        var movedRocks = MoveRocksAsFarAsPossible(rocksInRow, modifyX: true, direction: +1, Width);
                        AddKnownRocks(newRocks, movedRocks);
                    }
                    break;
                case Direction.East:
                    for (int row = 0; row < Width; row++)
                    {
                        var rocksInRow = GetRow(Rocks, row).Reverse();
                        var movedRocks = MoveRocksAsFarAsPossible(rocksInRow, modifyX: true, direction: -1, Width);
                        AddKnownRocks(newRocks, movedRocks);
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid direction");
            }

            Rocks = newRocks;
        }

        public T[] GetColumn<T>(T[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }

        public static T[] GetRow<T>(T[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }

        private static IEnumerable<Rock> MoveRocksAsFarAsPossible(IEnumerable<Rock> rocks, bool modifyX, int direction, int length)
        {
            int nextEmptyPosition = direction > 0 ? 0 : length - 1;
            List<Rock> result = new List<Rock>();

            foreach (Rock rock in rocks)
            {
                if (rock is null) continue;

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

        public long CalculateTotalLoad()
        {
            int sum = 0;

            foreach (var rock in Rocks)
            {
                if(rock is null) continue;

                if(rock.Movable) sum += (Height - rock.Y);
            }

            return sum;
        }

        private char[] emptyMap = [];

        public override string ToString()
        {
            if (emptyMap is [])
            {
                string line = new string(Enumerable.Repeat('.', Width).ToArray());
                emptyMap = string.Join('\n', Enumerable.Range(0, Height).Select(s => line)).ToCharArray();
            }

            char[] map = (char[])emptyMap.Clone();

            foreach (var rock in Rocks)
            {
                if(rock is null) continue;
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