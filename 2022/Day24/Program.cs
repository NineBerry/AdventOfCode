// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day24\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day24\Full.txt";
#endif

    var lines = File.ReadAllLines(fileName);
    Grid grid = new Grid(lines);

    Console.WriteLine("Part 1: " + Part1(grid));
    Console.WriteLine("Part 2: " + Part2(grid));
    Console.ReadLine();
}

long Part1(Grid grid)
{
    return grid.GetShortestTime(grid.Start, grid.End, 0);
}

long Part2(Grid grid)
{
    var firstTime = grid.GetShortestTime(grid.Start, grid.End, 0);
    var backTime = grid.GetShortestTime(grid.End, grid.Start, firstTime);
    var SecondTime = grid.GetShortestTime(grid.Start, grid.End, backTime);

    return SecondTime;
}

class Grid
{
    public readonly Point Start;
    public readonly Point End;
    private Dictionary<int, HashSet<Point>> BlizzardsAtStep = [];
    private HashSet<Point> Walls = [];

    private int Width;
    private int Height;
    private int CycleLength;

    public Grid(string[] lines)
    {
        Width = lines[0].Length;
        Height = lines.Length;
        CycleLength = (Width - 2) * (Height - 2);

        List<(Point Point, Direction Direction)> initialBlizzards = [];

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                Point point = new Point(x, y);
                char tile = lines[y][x];

                if (AllDirections.Any(d => d == (Direction)tile))
                {
                    initialBlizzards.Add((point, (Direction)tile));
                }

                if (tile == '#') Walls.Add(point);

                if (y == 0 && tile == '.') Start = point;
                if (y == Height - 1 && tile == '.') End = point;

            }
        }

        InitBlizzardsAtStep(initialBlizzards);
    }

    private void InitBlizzardsAtStep(List<(Point Point, Direction Direction)> blizzards)
    {
        BlizzardsAtStep[0] = blizzards.Select(b => b.Point).ToHashSet();

        foreach(var step in Enumerable.Range(1, CycleLength - 1))
        {
            blizzards = PerformStep(blizzards);
            BlizzardsAtStep[step] = blizzards.Select(b => b.Point).ToHashSet();
        }
    }

    private List<(Point Point, Direction Direction)> PerformStep(List<(Point Point, Direction Direction)> blizzards)
    {
        List <(Point Point, Direction Direction)> result = [];

        foreach (var blizzard in blizzards)
        {
            Point newPoint = blizzard.Point.GetNeightboringPoint(blizzard.Direction);

            if (newPoint.X == 0) newPoint.X = Width - 2;
            if (newPoint.X == Width - 1) newPoint.X = 1;
            if (newPoint.Y == 0) newPoint.Y = Height - 2;
            if (newPoint.Y == Height - 1) newPoint.Y = 1;

            result.Add((newPoint, blizzard.Direction));
        }

        return result;
    }

    HashSet<Point> GetBlizzardsAtStep(int step)
    {
        int cycleStep = step % CycleLength;

        return BlizzardsAtStep[cycleStep];
    }

    public int GetShortestTime(Point from, Point to, int startTime)
    {
        HashSet<(Point Point, int Step)> seen = [];
        PriorityQueue<(Point Point, int Step), int> queue = new();

        queue.Enqueue((from, startTime), startTime);

        while (queue.TryDequeue(out var current, out _))
        {
            if (seen.Contains(current)) continue;
            seen.Add(current);

            if (current.Point == to) return current.Step;

            int newStep = current.Step + 1;

            foreach (var direction in AllDirections)
            {
                Point newPoint = current.Point.GetNeightboringPoint(direction);
                TryEnqueue(newPoint, newStep);
            }

            TryEnqueue(current.Point, newStep);

            void TryEnqueue(Point point, int step)
            {
                if (Walls.Contains(point)) return;
                if (!IsInGrid(point)) return;
                if (GetBlizzardsAtStep(step).Contains(point)) return;
                queue.Enqueue((point, step), step);
            }
        }

        return 0;
    }


    private bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }

    private static Direction[] AllDirections = [Direction.North, Direction.East, Direction.South, Direction.West];
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
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }
}

enum Direction
{
    South = 'v',
    West = '<',
    North = '^',
    East = '>',
}
