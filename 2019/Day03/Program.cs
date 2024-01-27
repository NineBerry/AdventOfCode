// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day03\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day03\Full.txt";
#endif

    List<Point>[] paths = File.ReadAllLines(fileName).Select(Point.MakePoints).ToArray();

    Console.WriteLine("Part 1: " + Part1(paths[0], paths[1]));
    Console.WriteLine("Part 2: " + Part2(paths[0], paths[1]));
    Console.ReadLine();
}

int Part1(List<Point> path1, List<Point> path2)
{
    Point start = new Point(0, 0);
    
    return path1.Intersect(path2).Min(p => p.ManhattanDistance(start));
}

int Part2(List<Point> path1, List<Point> path2)
{
    return path1.Intersect(path2).Min(GetSteps);

    int GetSteps(Point point)
    {
        return path1.IndexOf(point) + path2.IndexOf(point) + 2;
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

    public static List<Point> MakePoints(string input)
    {
        Point point = new Point(0, 0);  

        List<Point> points = [];
        foreach (var turn in input.Split(","))
        {
            Direction direction = (Direction)turn[0];
            int distance = int.Parse(turn.Substring(1));

            foreach(var _ in Enumerable.Range(1, distance))
            {
                point = point.GetNeightboringPoint(direction);
                points.Add(point);
            }
        }

        return points;
    }
}

enum Direction
{
    South = 'D',
    West = 'L',
    North = 'U',
    East = 'R'
}