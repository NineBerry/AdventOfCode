// #define Sample
using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day02\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day02\Full.txt";
#endif

    var commands = File.ReadAllLines(fileName).Select(s => (Direction: (Direction)s[0], Distance: ExtractNumber(s))).ToArray();

    Console.WriteLine("Part 1: " + Part1(commands));
    Console.WriteLine("Part 2: " + Part2(commands));
    Console.ReadLine();
}

long Part1((Direction Direction, long Distance)[] commands)
{
    Point point = new Point(0, 0);

    foreach (var command in commands)
    {
        point = point.GetNeightboringPoint(command.Direction, command.Distance);
    }

    return point.X * point.Y;    
}

long Part2((Direction Direction, long Distance)[] commands)
{
    Point point = new Point(0, 0);
    long aim = 0;

    foreach (var command in commands)
    {
        switch (command.Direction)
        {
            case Direction.South:
                aim += command.Distance;
                break;
            case Direction.North:
                aim -= command.Distance;
                break;
            case Direction.East:
                point = point.GetNeightboringPoint(Direction.East, command.Distance);
                point = point.GetNeightboringPoint(Direction.South, aim * command.Distance);
                break;
        }
    }

    return point.X * point.Y;
}


long ExtractNumber(string s)
{
    return Regex.Matches(s, "\\d+").Select(m => long.Parse(m.Value)).First();
}

record struct Point(long X, long Y)
{
    public Point GetNeightboringPoint(Direction direction, long distance)
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
    South = 'd',
    West = 'b',
    North = 'u',
    East = 'f'
}
