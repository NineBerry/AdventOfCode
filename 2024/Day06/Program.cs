// #define Sample

using System.Diagnostics;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day06\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day06\Full.txt";
#endif
    var lines = File.ReadAllLines(fileName);
    Grid grid = new Grid(lines);

    var visitedDirections = grid.DoWalk(grid.MapStartPoint, grid.MapStartDirection);
    var visitedPoints = visitedDirections.Select(v => v.Point).ToHashSet();

    Console.WriteLine("Part 1: " + visitedPoints.Count);
    Console.WriteLine("Part 2: " + grid.CountPossibleBlocksToCreateLoop(visitedDirections, visitedPoints));
    Console.ReadLine();
}

// Part 2 
// 1630 too high
// 1629 too high

class Grid
{
    private HashSet<Point> Wall = [];

    private readonly int Height;
    private readonly int Width;

    public readonly Direction MapStartDirection;
    public readonly Point MapStartPoint;

    public Grid(string[] lines)
    {
        Height = lines.Length;
        Width = lines.First().Length;

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                Point point = new Point(x, y);
                char tile = lines[y][x];

                switch (tile)
                {
                    case '.':
                        // Just ground, do nothing
                        break;
                    case '#':
                        Wall.Add(point);
                        break;
                    case (char)Direction.North:
                    case (char)Direction.South:
                    case (char)Direction.West:
                    case (char)Direction.East:
                        MapStartDirection = (Direction) tile;
                        MapStartPoint = point;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public HashSet<(Point Point, Direction Direction)> DoWalk(Point start, Direction startDirection)
    {
        HashSet<(Point,Direction)> visited = [];

        Point currentPoint = start;
        Direction currentDirection = startDirection;

        while (IsInGrid(currentPoint))
        {
            // Cycle detected
            if (visited.Contains((currentPoint, currentDirection))) return [];
            visited.Add((currentPoint, currentDirection));
            
            Point nextPoint = currentPoint.GetNeightboringPoint(currentDirection);

            if (Wall.Contains(nextPoint))
            {
                currentDirection = currentDirection.TurnRight();
            }
            else
            {
                currentPoint = nextPoint;
            }
        }

        return visited;
    }

    public long CountPossibleBlocksToCreateLoop(
        HashSet<(Point Point, Direction Direction)> visitedDirections,
        HashSet<Point> visitedPoints)
    {
        HashSet<Point> possibleBlocks = [];

        foreach (var visited in visitedDirections)
        {
            var nextPoint = visited.Point.GetNeightboringPoint(visited.Direction);

            if (nextPoint == MapStartPoint) continue;
            if (Wall.Contains(nextPoint)) continue;
            if (possibleBlocks.Contains(nextPoint)) continue;
            if (!IsInGrid(nextPoint)) continue;

            // Possible performance improvement:
            // Use List instead of Hashset to remember path taken
            // Then here check whether nextPoint is already in the path taken so far
            // Then we can ignore it. Because we wouldn't have reached this point
            // if there had been a blocker in the path segment before.
            // Time machine paradox: If we travel back and change the past path... :)

            if (CheckBlockCreatesCycle(nextPoint, MapStartPoint, MapStartDirection))
            {
                possibleBlocks.Add(nextPoint);
            }
        }

        return possibleBlocks.Count;
    }

    private bool CheckBlockCreatesCycle(Point block, Point currentPoint, Direction currentDirection)
    {
        Debug.Assert(!Wall.Contains(block));
        Wall.Add(block);
        try
        {
            return !DoWalk(currentPoint, currentDirection).Any();
        }
        finally
        {
            Wall.Remove(block);
        }
    }

    private bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }
}
enum Direction
{
    None,
    South = 'v',
    West = '<',
    North = '^',
    East = '>',
}

record struct Point(int X, int Y)
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


static class Extensions
{
    public static Direction TurnRight(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.East,
            Direction.East => Direction.South,
            Direction.South => Direction.West,
            Direction.West => Direction.North,
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }
}
