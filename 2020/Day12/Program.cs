// #define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day12\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day12\Full.txt";
#endif

    string[] commands = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(commands));
    Console.WriteLine("Part 2: " + Part2(commands));
    Console.ReadLine();
}

long Part1(string[] commands)
{
    Point ship = new Point(0, 0);
    Direction direction = Direction.East;

    foreach (string command in commands)
    {
        char commandType = command[0];
        int value = int.Parse(command.Substring(1));

        switch (commandType)
        {
            case 'N':
            case 'S':
            case 'W':
            case 'E':
                ship = ship.GetNeightboringPoint((Direction)commandType, value);
                break;
            case 'R':
                direction = direction.TurnRight(value);
                break;
            case 'L':
                direction = direction.TurnLeft(value);
                break;
            case 'F':
                ship = ship.GetNeightboringPoint(direction, value);
                break;
        }
    }

    return ship.ManhattanDistance();
}

long Part2(string[] commands)
{
    Point ship = new Point(0, 0);
    Point waypoint = new Point(10, -1);

    foreach (string command in commands)
    {
        char commandType = command[0];
        int value = int.Parse(command.Substring(1));

        switch (commandType)
        {
            case 'N':
            case 'S':
            case 'W':
            case 'E':
                waypoint = waypoint.GetNeightboringPoint((Direction)commandType, value);
                break;
            case 'R':
                waypoint = waypoint.RotateRight(value);
                break;
            case 'L':
                waypoint = waypoint.RotateLeft(value);
                break;
            case 'F':
                ship = ship.MoveVector(waypoint, value);
                break;
        }
    }

    return ship.ManhattanDistance();
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

    public int ManhattanDistance()
    {
        return Math.Abs(Y) + Math.Abs(X);
    }

    public Point MoveVector(Point vector, int factor)
    {
        return new Point(X + factor * vector.X, Y + factor * vector.Y);
    }

    public Point RotateLeft(int degrees)
    {
        return RotateRight(360 - (degrees % 360));
    }

    public Point RotateRight(int degrees)
    {
        Point point = new Point(X, Y);

        foreach (var _ in Enumerable.Range(1, degrees / 90))
            point = new Point(-point.Y, point.X);
                
        return point;
    }
}

public enum Direction
{
    South = 'S',
    West = 'W',
    North = 'N',
    East = 'E',
}

public static class Extensions
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

    public static Direction TurnRight(this Direction direction, int value)
    {
        foreach (var _ in Enumerable.Range(1, value / 90))
            direction = direction.TurnRight();

        return direction;
    }
    public static Direction TurnLeft(this Direction direction, int value)
    {
        foreach (var _ in Enumerable.Range(1, value / 90))
            direction = direction.TurnLeft();

        return direction;
    }
}