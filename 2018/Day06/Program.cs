// #define Sample

using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day06\Sample.txt";
    int distanceLimit = 32;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day06\Full.txt";
    int distanceLimit = 10000;
#endif

    PointWithOwner[] pointsWithOwner = 
        File.ReadLines(fileName)
        .Select((line, index) => Grid.ParsePointWithOwner(line, index + 1))
        .ToArray();

    Grid grid = new Grid();

    PointWithOwner[] frontPoints = [.. pointsWithOwner];
    HashSet<int> growingOwners = [];

    foreach(var counter in Enumerable.Range(1, distanceLimit / pointsWithOwner.Length))
    {
        (frontPoints, growingOwners) = grid.MoveOneStep(frontPoints);
    }

    Console.WriteLine("Part 1: " + Part1(grid, pointsWithOwner, growingOwners));
    Console.WriteLine("Part 2: " + Part2(grid, pointsWithOwner, distanceLimit));
    Console.ReadLine();
}
long Part1(Grid grid, PointWithOwner[] pointsWithOwner, HashSet<int> growingOwners)
{
    return
        pointsWithOwner
        .Select(p => p.Owner)
        .Except(growingOwners)
        .Select(grid.GetOwnedCells)
        .Max();
}

int Part2(Grid grid, PointWithOwner[] pointsWithOwner, int limit)
{
    var originalPoints = pointsWithOwner.Select(p => p.Point).ToArray();

    return 
        grid
        .GetAllPoints()
        .Where(p => originalPoints.Select(op => op.ManhattanDistance(p)).Sum() < limit)
        .Count();
}


class Grid
{
    Dictionary<Point, int> cells = [];

    public Grid()
    {
    }

    public (PointWithOwner[] FrontPoints, HashSet<int> GrowingOwners) MoveOneStep(PointWithOwner[] frontPoints)
    {
        foreach (var point in frontPoints)
        {
            cells[point.Point] = point.Owner;
        }

        HashSet<PointWithOwner> possibleNewPoints = [];

        // Get all possible new points
        foreach(var sourcePoint in frontPoints) 
        { 
            foreach(var direction in AllDirections)
            {
                var possiblePoint = sourcePoint.Point.GetNeightboringPoint(direction);
                if (CanMoveToCell(possiblePoint))
                {
                    possibleNewPoints.Add(new PointWithOwner(possiblePoint, sourcePoint.Owner));
                }
            }
        }

        HashSet<int> growingOwners = possibleNewPoints.Select(p => p.Owner).Where(i => i != 0).ToHashSet();

        // points appearing multiple times, set owner to 0
        var multipleHitPoints = possibleNewPoints.GroupBy(a => a.Point).Where(group => group.Count() > 1).Select(group => group.Key).ToHashSet();
        possibleNewPoints = possibleNewPoints.Where(p => !multipleHitPoints.Contains(p.Point)).ToHashSet();
        foreach(var multiHitPoint in multipleHitPoints)
        {
            possibleNewPoints.Add(new PointWithOwner(multiHitPoint, 0));
        }

        foreach(var point in possibleNewPoints)
        {
            cells[point.Point] = point.Owner;
        }

        return (possibleNewPoints.ToArray(), growingOwners);
    }


    public int GetOwnedCells(int owner)
    {
        return cells.Where(pair => pair.Value == owner).Count();
    }

    public Point[] GetAllPoints()
    {
        return cells.Keys.ToArray();
    }

    private bool CanMoveToCell(Point point)
    {
        return !cells.ContainsKey(point);
    }

    public Direction[] AllDirections = [Direction.West, Direction.South, Direction.North, Direction.East];

    public static PointWithOwner ParsePointWithOwner(string line, int owner)
    {
        int[] values = Regex.Matches(line, "\\d+").Select(m => int.Parse(m.Value)).ToArray();
        return new PointWithOwner(new Point(values[0], values[1]), owner);
    }
}


record struct Point(int X, int Y)
{
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

    public int ManhattanDistance(Point point)
    {
        var xDistance = Math.Abs(point.X - X);
        var yDistance = Math.Abs(point.Y - Y);
        return xDistance + yDistance;
    }
}

record struct PointWithOwner(Point Point, int Owner)
{
}

enum Direction
{
    South,
    West,
    North,
    East
}