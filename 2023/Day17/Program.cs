// string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day17\Sample.txt";
// string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day17\Sample2.txt";
string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day17\Full.txt";

string[] lines = File.ReadAllLines(fileName);
Grid grid = new Grid(lines);

Point targetPoint = new Point(grid.Width - 1, grid.Height - 1);

int part1 = grid.GetShortestPath(targetPoint, 1, 3);
Console.WriteLine("Part 1: " + part1);

int part2 = grid.GetShortestPath(targetPoint, 4, 10);
Console.WriteLine("Part 2: " + part2);

Console.ReadLine();

class Grid
{
    public readonly int Width;
    public readonly int Height;

    private Dictionary<Point, int> blockWeights = new();

    public Grid(string[] text)
    {
        Width = text.First().Length;
        Height = text.Length;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int weight = int.Parse("" + text[y][x]);

                Point point = new Point(x, y);
                blockWeights.Add(point, weight);
            }
        }
    }

    public int GetShortestPath(Point targetPoint, int minStraightMoves, int maxStraightMoves)
    {
        return RunDijkstraFor(new Point(0, 0), targetPoint, minStraightMoves, maxStraightMoves);
        
    }


    public int RunDijkstraFor(Point startPoint, Point targetPoint, int minStraightMoves, int maxStraightMoves)
    {
        // Using PriorityQueue is important for Dijkstra algorithm
        PriorityQueue<(Point Point, int CurrentWeight, Direction Direction, int StraightCounter), int> todoPaths = new();
        HashSet<(Point, Direction, int)> visited = new();

        // Init Start Points in all directions
        foreach (var direction in AllDirections())
        {
            todoPaths.Enqueue((startPoint, 0, direction, 0), 0);
        }

        while (true)
        {
            var path = todoPaths.Dequeue();

            // If we have reachted target with the right conditions, return result
            if(path.Point == targetPoint && path.StraightCounter >= minStraightMoves && path.StraightCounter <= maxStraightMoves)
            {
                return path.CurrentWeight;
            }

            // Never visit the same block twice with the same conditions
            if (visited.Contains((path.Point, path.Direction, path.StraightCounter))) continue;
            visited.Add((path.Point, path.Direction, path.StraightCounter));

            // Only turn after a minimum number of straight moves
            if (path.StraightCounter >= minStraightMoves)
            {
                PerformStep(path.Direction.TurnLeft(), 1);
                PerformStep(path.Direction.TurnRight(), 1);
            }

            // Don't continue straigt after a max number of straight moves
            if (path.StraightCounter < maxStraightMoves)
            {
                PerformStep(path.Direction, path.StraightCounter + 1);
            }

            void PerformStep(Direction nextDirection, int nextStraighCounter)
            {
                Point nextPoint = path.Point.GetNeightboringPoint(nextDirection);

                if (!IsInGrid(nextPoint)) return;

                int blockWeight = blockWeights[nextPoint];
                int newWeight = path.CurrentWeight + blockWeight;

                // Use newWeight as priority. Loop will always choose lowest newWright from todoPaths
                todoPaths.Enqueue((nextPoint, newWeight, nextDirection, nextStraighCounter), newWeight);
            }
        }
    }

    public bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }
    private static Direction[] AllDirections() => [Direction.East, Direction.North, Direction.South, Direction.West];
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

enum Direction
{
    None,
    South,
    West,
    North,
    East
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
