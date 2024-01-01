// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day01\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day01\Full.txt";
#endif

    string input = File.ReadAllText(fileName);

    var steps = Regex.Matches(input, "[RL][0-9]+").Select(m => m.Value).Select(s => (s[0], int.Parse(s.Substring(1))));


    Console.WriteLine("Part 1: " + Part1(steps));
    Console.WriteLine("Part 2: " + Part2(steps));
    Console.ReadLine();
}

long Part1(IEnumerable<(char Turn, int Distance)> steps)
{
    Point point = new Point(0,0);
    Direction direction = Direction.North;

    foreach (var step in steps)
    {
        direction = step.Turn switch
        {
            'R' => direction.TurnRight(),
            'L' => direction.TurnLeft(),
            _ => throw new ApplicationException("Unknown turn")
        };

        point = point.GetNeightboringPoint(direction, step.Distance);
    }

    Console.WriteLine(point);
    
    return Math.Abs(point.X) + Math.Abs(point.Y);
}

long Part2(IEnumerable<(char Turn, int Distance)> steps)
{
    HashSet<Point> visited = new HashSet<Point>();
    Point point = new Point(0, 0);
    Direction direction = Direction.North;

    foreach (var step in steps)
    {
        visited.Add(point);

        direction = step.Turn switch
        {
            'R' => direction.TurnRight(),
            'L' => direction.TurnLeft(),
            _ => throw new ApplicationException("Unknown turn")
        };

        for(int i=0; i < step.Distance; i++)
        {
            point = point.GetNeightboringPoint(direction, 1);
            if (visited.Contains(point)) goto found;
            visited.Add(point);
        }

    }

    found: Console.WriteLine(point);

    return Math.Abs(point.X) + Math.Abs(point.Y);
}

record struct Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction, int distance)
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
    None = '\0',
    South = 'v',
    West = '<',
    North = '^',
    East = '>'
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

    public static Direction Opposite(this Direction direction)
    {
        return direction switch
        {
            Direction.South => Direction.North,
            Direction.West => Direction.East,
            Direction.North => Direction.South,
            Direction.East => Direction.West,
            Direction.None => Direction.None,
            _ => throw new ArgumentException("Direction", nameof(direction))
        };
    }
}
