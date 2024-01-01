// #define Sample
{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day03\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2015\Day03\Full.txt";
#endif

    string input = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(string input)
{
    Map map = new();

    Point current = new Point(0, 0);
    map.Add(current);

    foreach(var ch in input)
    {
        current = current.GetNeightboringPoint((Direction)ch);
        map.Add(current);
    }

    return map.Count();
}

long Part2(string input)
{
    Map map = new();

    Point currentSanta = new Point(0, 0);
    map.Add(currentSanta);
    Point currentRobo = new Point(0, 0);
    map.Add(currentRobo);

    bool robosTurn = false;

    foreach (var ch in input)
    {
        var direction = (Direction)ch;

        if (robosTurn)
        {
            currentRobo = currentRobo.GetNeightboringPoint(direction);
            map.Add(currentRobo);
        }
        else
        {
            currentSanta = currentSanta.GetNeightboringPoint(direction);
            map.Add(currentSanta);
        }

        robosTurn = !robosTurn;
    }

    return map.Count();
}

class Map : HashSet<Point>;

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
