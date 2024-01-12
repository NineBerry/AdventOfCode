// #define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day13\Sample.txt";
    Point targetPoint = new Point(7, 4);
    int maxSteps = 2;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day13\Full.txt";
    Point targetPoint = new Point(31, 39);
    int maxSteps = 50;
#endif

    int favoriteNumber = int.Parse(File.ReadAllText(fileName));
    Grid grid = new Grid(favoriteNumber);
    Console.WriteLine("Part 1: " + Part1(grid, targetPoint));

    grid = new Grid(favoriteNumber);
    Console.WriteLine("Part 2: " + Part2(grid, maxSteps));
    Console.ReadLine();
}

long Part1(Grid grid, Point targetPoint)
{
    Point start = new Point(1, 1);
    var path = grid.RunDykstra(start, targetPoint);
    return path.Count - 1;
}

long Part2(Grid grid, int maxSteps)
{
    Point start = new Point(1, 1);
    HashSet<Point> visited = grid.VisitAll(start, maxSteps);
    return visited.Count;
}

class Grid
{
    private int FavoriteNumber;
    Dictionary<Point, bool> freeCells = [];

    public Grid(int favoriteNumber)
    {
        FavoriteNumber = favoriteNumber;
    }

    public List<Point> RunDykstra(Point start, Point end)
    {
        var queue = new PriorityQueue<(Point NextNode, HashSet<Point> Visited, List<Point> CurrentPath), int>();

        queue.Enqueue((start, [], [start]), 0);

        while (queue.TryDequeue(out var path, out _))
        {
            if (path.NextNode == end)
            {
                return path.CurrentPath;
            }

            if (path.Visited.Contains(path.NextNode)) continue;
            
            if (!CanMoveToCell(path.NextNode)) continue;

            foreach (var direction in AllDirections)
            {
                Point next = path.NextNode.GetNeightboringPoint(direction);
                queue.Enqueue((next, [.. path.Visited, path.NextNode], [.. path.CurrentPath, next]), path.CurrentPath.Count);
            }
        }

        return [];
    }

    public HashSet<Point> VisitAll(Point start, int maxSteps)
    {
        var queue = new PriorityQueue<(Point NextNode, HashSet<Point> Visited, List<Point> CurrentPath), int>();

        queue.Enqueue((start, [], [start]), 0);

        while (queue.TryDequeue(out var path, out int steps))
        {
            if (steps > maxSteps) break;

            if (path.Visited.Contains(path.NextNode)) continue;

            if (!CanMoveToCell(path.NextNode)) continue;

            foreach (var direction in AllDirections)
            {
                Point next = path.NextNode.GetNeightboringPoint(direction);
                queue.Enqueue((next, [.. path.Visited, path.NextNode], [.. path.CurrentPath, next]), path.CurrentPath.Count);
            }
        }

        return freeCells.Where(pair => pair.Value).Select(pair => pair.Key).ToHashSet();
    }


    private bool CanMoveToCell(Point point)
    {
        if (point.X < 0) return false;
        if (point.Y < 0) return false;

        if(!freeCells.TryGetValue(point, out bool result))
        {
            result = point.CalculateFree(FavoriteNumber);
            freeCells.Add(point, result);
        }

        return result;
    }

    public Direction[] AllDirections = [Direction.West, Direction.South, Direction.North, Direction.East];
}

record struct Point(int X, int Y)
{
    public bool CalculateFree(int favoriteNumber)
    {
        int number = X * X + 3 * X + 2 * X * Y + Y + Y * Y;
        number += favoriteNumber;

        int countOnes = 0;

        while (number > 0) 
        {
            countOnes += number % 2;
            number /= 2;
        }

        return countOnes % 2 == 0;
    }

    public Point GetNeightboringPoint(Direction direction, int distance = 1)
    {
        return direction switch
        {
            Direction.South => this with { Y = this.Y + distance },
            Direction.West => this with { X = this.X - distance },
            Direction.North => this with { Y = this.Y - distance },
            Direction.East => this with { X = this.X + distance },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

}

enum Direction
{
    South,
    West,
    North,
    East
}