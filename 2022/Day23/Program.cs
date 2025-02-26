// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day23\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day23\Full.txt";
#endif

    var lines = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(lines));
    Console.WriteLine("Part 2: " + Part2(lines));
    Console.ReadLine();
}

long Part1(string[] lines)
{
    Grid grid = new Grid(lines);    

    foreach(var _ in Enumerable.Range(0, 10))
    {
        grid.DoRound();
    }

    return grid.CountEmptyCells();
}

long Part2(string[] lines)
{
    Grid grid = new Grid(lines);

    foreach (var roundNumber in Enumerable.Range(1, int.MaxValue))
    {
        if (!grid.DoRound())
        {
            return roundNumber;
        };
    }

    return 0;
}


class Grid
{
    private HashSet<Point> Elves = [];

    public Grid(string[] lines)
    {
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                Point point = new Point(x, y);
                char tile = lines[y][x];
                if (tile == '#') Elves.Add(point);
            }
        }
    }

    public bool DoRound()
    {
        Dictionary<Point, Point> plannedMovements = [];
        Dictionary<Point, long> targetCounts = [];

        bool anyMoved = false;

        foreach (var elf in Elves)
        {
            var allNeightboringPoints = elf.GetAllNeightboringPoints();
            if (allNeightboringPoints.All(n => !Elves.Contains(n)))
            {
                plannedMovements[elf] = elf;
                targetCounts.Increase(elf, 1);
                continue;
            }

            bool moved = false;
            foreach (int offset in Enumerable.Range(0, 4))
            {
                Direction direction = directions[(startDirectionIndex + offset) % directions.Length];
                var neightboringPointsAtSide = elf.GetNeightboringPointsAtSide(direction);

                if (neightboringPointsAtSide.All(n => !Elves.Contains(n)))
                {
                    Point target = elf.GetNeightboringPoint(direction);
                    plannedMovements[elf] = target;
                    targetCounts.Increase(target, 1);
                    moved = true;
                    break;
                }
            }

            if (!moved)
            {
                plannedMovements[elf] = elf;
                targetCounts.Increase(elf, 1);
                continue;
            }

            anyMoved = anyMoved || moved;
        }

        HashSet<Point> movedElves = [];

        foreach (var movement in plannedMovements)
        {
            if (targetCounts[movement.Value] == 1)
            {
                movedElves.Add(movement.Value);
            }
            else
            {
                movedElves.Add(movement.Key);
            }
        }

        Elves = movedElves;

        startDirectionIndex++;
        if (startDirectionIndex == directions.Length) startDirectionIndex = 0;

        return anyMoved;
    }

    public long CountEmptyCells()
    {
        long minX = Elves.Min(e => e.X);
        long maxX = Elves.Max(e => e.X);
        long minY = Elves.Min(e => e.Y);
        long maxY = Elves.Max(e => e.Y);

        long width = maxX - minX + 1;
        long height = maxY - minY + 1;

        long area = width * height;
        return area - Elves.Count;
    }

    private int startDirectionIndex = 0;
    private Direction[] directions = [Direction.North, Direction.South, Direction.West, Direction.East];
}


record struct Point(long X, long Y)
{
    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.South => this with { Y = this.Y + 1 },
            Direction.West => this with { X = this.X - 1 },
            Direction.North => this with { Y = this.Y - 1 },
            Direction.East => this with { X = this.X + 1 },

            Direction.SouthWest => this with { X = this.X - 1, Y = this.Y + 1 },
            Direction.SouthEast => this with { X = this.X + 1, Y = this.Y + 1 },
            Direction.NorthEast => this with { X = this.X + 1, Y = this.Y - 1 },
            Direction.NorthWest => this with { X = this.X - 1, Y = this.Y - 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public Point[] GetNeightboringPointsAtSide(Direction direction)
    {
        return direction switch
        {
            Direction.South => [GetNeightboringPoint(Direction.South), GetNeightboringPoint(Direction.SouthWest), GetNeightboringPoint(Direction.SouthEast)],
            Direction.West => [GetNeightboringPoint(Direction.West), GetNeightboringPoint(Direction.SouthWest), GetNeightboringPoint(Direction.NorthWest)],
            Direction.North => [GetNeightboringPoint(Direction.North), GetNeightboringPoint(Direction.NorthEast), GetNeightboringPoint(Direction.NorthWest)],
            Direction.East => [GetNeightboringPoint(Direction.East), GetNeightboringPoint(Direction.SouthEast), GetNeightboringPoint(Direction.NorthEast)],

            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public Point[] GetAllNeightboringPoints()
    {
        var me = this;
        return AllDirections.Select(me.GetNeightboringPoint).ToArray();
    }

    private static Direction[] AllDirections = [Direction.North, Direction.East, Direction.South, Direction.West, Direction.SouthWest, Direction.SouthEast, Direction.NorthEast, Direction.NorthWest];

}

enum Direction
{
    None,
    South,
    West,
    North,
    East,
    NorthEast,
    NorthWest,
    SouthEast,
    SouthWest
}

static class Extensions
{
    public static void Increase(this Dictionary<Point, long> dictionary, Point key, long value)
    {
        dictionary.TryGetValue(key, out var currentValue);
        dictionary[key] = currentValue + value;
    }
}