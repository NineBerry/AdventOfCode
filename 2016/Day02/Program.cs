// #define Sample
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day02\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2016\Day02\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(string[] lines)
{
    Point point = new Point(1, 1);
    string result = "";

    foreach (string line in lines)
    {
        foreach(Direction direction in line)
        {
            Point candidate = point.GetNeightboringPoint(direction, 1);
            if (candidate.IsValid())
            {
                point = candidate;
            }
        }

        result += point.GetPart1Code(); 
    }

    return long.Parse(result);
}

string Part2(string[] lines)
{
    Point point = new Point(-2, 0);
    string result = "";

    foreach (string line in lines)
    {
        foreach (Direction direction in line)
        {
            Point candidate = point.GetNeightboringPoint(direction, 1);
            if (candidate.GetPart2Code() != '0')
            {
                point = candidate;
            }
        }

        result += point.GetPart2Code();
    }

    return result;
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

    public bool IsValid() 
    { 
        return (X is >= 0 and  <= 2) && (Y is >= 0 and <= 2); 
    }

    public char GetPart1Code()
    {
        return (X, Y) switch
        {
            (0, 0) => '1',
            (0, 1) => '4',
            (0, 2) => '7',
            (1, 0) => '2',
            (1, 1) => '5',
            (1, 2) => '8',
            (2, 0) => '3',
            (2, 1) => '6',
            (2, 2) => '9',
            _ => '0'
        };
    }

    public char GetPart2Code()
    {
        return (X, Y) switch
        {
            (0, -2) => '1',
            (-1, -1) => '2',
            (0, -1) => '3',
            (1, -1) => '4',
            (-2, 0) => '5',
            (-1, 0) => '6',
            (0, 0) => '7',
            (1, 0) => '8',
            (2, 0) => '9',
            (-1, 1) => 'A',
            (0, 1) => 'B',
            (1, 1) => 'C',
            (0, 2) => 'D',
            _ => '0'
        };
    }
}
enum Direction
{
    None = '\0',
    South = 'D',
    West = 'L',
    North = 'U',
    East = 'R'
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
