using System.Text;

using Grid = System.Text.StringBuilder[];

namespace Day14
{
    internal class Program
    {
        private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day14\Sample.txt";
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day14\Full.txt";

        private static int Width { get; set; } = 0;
        private static int Height { get; set; } = 0;
        private static Grid gameGrid = [];

        private record struct Point(int X, int Y);
        private static Point[] AllPoints = [];

        static void Main()
        {
            // Initialize game grid
            gameGrid = GetLines(fileName).Select(s => new StringBuilder(s)).ToArray();
            Width = gameGrid.First().Length;
            Height = gameGrid.Length;

            // Initialize list of all points
            AllPoints = Enumerable.Range(0, Width)
                .Select(x =>
                    Enumerable.Range(0, Height).Select(y => new Point(x, y)))
                .SelectMany(p => p)
                .ToArray();

            // Part 1
            TiltGrid(Direction.North);
            long totalLoad = CalculateTotalLoad(gameGrid);
            Console.WriteLine("Part 1: " + totalLoad);

            // Part 2

            // Get fresh version of grid
            gameGrid = GetLines(fileName).Select(s => new StringBuilder(s)).ToArray();

            long resultPart2 = Part2();
            Console.WriteLine("Part 2: " + resultPart2);

            Console.ReadLine();
        }

        private static long Part2()
        {
            Dictionary<string, int> seenGrids = new();

            const int iterationsToRun = 1_000_000_000;

            for (int iteration = 0; iteration < iterationsToRun; iteration++)
            {
                string before = GetGridString(gameGrid);

                if (seenGrids.TryGetValue(before, out int foundIteration))
                {
                    // We found a loop
                    int loopLength = iteration - foundIteration;
                    int targetOffSetWithinLoop = (iterationsToRun - iteration) % loopLength;

                    for (int l = 0; l < targetOffSetWithinLoop; l++)
                    {
                        SpinCycle();
                    }

                    break;
                }

                seenGrids.Add(before, iteration);
                SpinCycle();
            }

            long totalLoad2 = CalculateTotalLoad(gameGrid);
            return totalLoad2;
        }

        private static void SpinCycle()
        {
            TiltGrid(Direction.North);
            TiltGrid(Direction.West);
            TiltGrid(Direction.South);
            TiltGrid(Direction.East);
        }


        public class Rock
        {
            public int[] Coordinates { get; } = new int[2];
            public bool Movable { get; init;}
        }

        private static string GetGridString(Grid grid) => string.Join("", grid.Select(sb => sb.ToString()));

        private static void TiltGrid(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    // row by row from top to bottom
                    for (int row = 0; row < Height; row++)
                    {
                        MoveRocksAsFarAsPossible(AllPoints.Where(p => p.Y == row), direction);
                    }
                    break;
                case Direction.South:
                    // row by row from bottom to top
                    for (int row = Height - 1; row >= 0; row--)
                    {
                        MoveRocksAsFarAsPossible(AllPoints.Where(p => p.Y == row), direction);
                    }
                    break;
                case Direction.West:
                    // column by column from left to right
                    for (int column = 0; column < Width; column++)
                    {
                        MoveRocksAsFarAsPossible(AllPoints.Where(p => p.X == column), direction);
                    }
                    break;
                case Direction.East:
                    // column by column from right to left
                    for (int column = Width - 1; column >= 0; column--)
                    {
                        MoveRocksAsFarAsPossible(AllPoints.Where(p => p.X == column), direction);
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid direction");
            }
        }
        
        private static void MoveRocksAsFarAsPossible(IEnumerable<Point> positions, Direction direction)
        {
            foreach (var position in positions)
            {
                Point? currentPosition = position;
                
                while (currentPosition != null)
                {
                    currentPosition = MoveRock(currentPosition.Value, direction);
                }
            }
        }

        private static Point? MoveRock(Point position, Direction direction)
        {
            Tile tile = GetTile(position);

            // Only movable rocks can move
            if (tile != Tile.MovableRock) return null;

            Point targetPosition = Move(position, direction);
            
            // Can't move outside map
            if(!IsPointInGameMap(targetPosition)) return null;

            Tile targetTile = GetTile(targetPosition);
            
            // Way is blocked by other rock
            if(targetTile != Tile.Ground) return null;

            // Actual move
            SetTile(position, Tile.Ground);
            SetTile(targetPosition, Tile.MovableRock);

            return targetPosition;
        }

        private static Tile GetTile(Point point)
        {
            if (!IsPointInGameMap(point)) throw new ArgumentException("Point outside map");

            return (Tile) gameGrid[point.Y][point.X];
        }

        private static void SetTile(Point point, Tile tile)
        {
            if (!IsPointInGameMap(point)) throw new ArgumentException("Point outside map");

            gameGrid[point.Y][point.X] = (char) tile;
        }

        public static long CalculateTotalLoad(Grid grid)
        {
            return AllPoints
                .Where(p => GetTile(p) == Tile.MovableRock)
                .Sum(P => Height - P.Y);
        }

        private static bool IsPointInGameMap(Point point) =>
            point.X >= 0 && point.X < Width 
            && point.Y >= 0 && point.Y < Height;

        private static Point Move(Point point, Direction direction)
        {
            switch (direction)
            {
                case Direction.South:
                    return point with { Y = point.Y + 1 };
                case Direction.West:
                    return point with { X = point.X - 1 };
                case Direction.North:
                    return point with { Y = point.Y - 1 };
                case Direction.East:
                    return point with { X = point.X + 1 };
                default:
                    throw new ArgumentException("Unknown direction", nameof(direction));
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

        public enum Tile
        {
            Ground = '.',
            MovableRock = 'O',
            FixedRock = '#'
        }
        private static string[] GetLines(string fileName)
        {
            return File.ReadAllLines(fileName);
        }
    }
}
