// #define Sample

using Path = System.Collections.Generic.List<(Point Point, Direction Direction)>;
using VisitedSet = System.Collections.Generic.HashSet<(Point, Direction)>;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day16\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day16\Full.txt";
#endif

    Grid grid = new(File.ReadAllLines(fileName));
    var lowestScorePaths = grid.FindLowestScorePaths();

    Console.WriteLine("Part 1: " + Part1(lowestScorePaths));
    Console.WriteLine("Part 2: " + Part2(lowestScorePaths));
    Console.ReadLine();
}

long Part1((Path[] Paths, long Score) lowestScorePaths)
{
    return lowestScorePaths.Score;
}


long Part2((Path[] Paths, long Score) lowestScorePaths)
{
    return lowestScorePaths.Paths
        .SelectMany(s => s.Select(p => p.Point))
        .Distinct()
        .Count();
}


class Grid
{
    private Point StartPosition;
    private Point EndPosition;
    private HashSet<Point> Walls = [];

    public Grid(string[] lines)
    {
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
                        Walls.Add(point);
                        break;
                    case 'S':
                        StartPosition = point;
                        break;
                    case 'E':
                        EndPosition = point;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // Ideas for performance improvement: 
    // ==================================
    // * Use the trick from 2023 Day 23 and first collapse map into minimal graph
    // * Treat Nort/South and West/East as one direction in visited and other collections

    internal (Path[] Paths, long Score) FindLowestScorePaths()
    {
        var queue = new PriorityQueue<(Path Path, VisitedSet Visited), long>();
        List<Path> paths = [];
        Dictionary<(Point, Direction), long> lowestScoreAt = [];

        queue.Enqueue(
            ([(StartPosition, Direction.East)], [(StartPosition, Direction.East)]), 0);

        long targetScore = long.MaxValue;

        while (queue.TryDequeue(out var current, out var score))
        {
            if(score > targetScore) break;

            var currentPosition = current.Path.Last();

            if (currentPosition.Point == EndPosition)
            {
                targetScore = score;
                paths.Add(current.Path);
            }

            CheckAdd(currentPosition.Direction.TurnLeft(), score + 1000 + 1);
            CheckAdd(currentPosition.Direction.TurnRight(), score + 1000 + 1);
            CheckAdd(currentPosition.Direction, score + 1);

            void CheckAdd(Direction nextDirection, long nextScore)
            {
                var next = (Point: currentPosition.Point.GetNeightboringPoint(nextDirection), Direction: nextDirection);

                if (IsWall(next.Point)) return;
                if (current.Visited.Contains(next)) return;

                if (lowestScoreAt.TryGetValue(next, out var seenScore))
                {
                    if (nextScore > seenScore) return;
                }
                lowestScoreAt[next] = nextScore;

                queue.Enqueue(
                ([.. current.Path, next],[.. current.Visited, next]), nextScore);
            }
        }

        return ([..paths], targetScore);
    }

    private bool IsWall(Point point) => Walls.Contains(point);
}

enum Direction
{
    South,
    West,
    North,
    East
}

record struct Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction, int count = 1)
    {
        return direction switch
        {
            Direction.South => this with { Y = Y + count },
            Direction.West => this with { X = X - count },
            Direction.North => this with { Y = Y - count },
            Direction.East => this with { X = X + count },
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

    public static Direction TurnLeft(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.West,
            Direction.West => Direction.South,
            Direction.South => Direction.East,
            Direction.East => Direction.North,
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }
}