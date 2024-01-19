// #define Sample

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day03\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day03\Full.txt";
#endif

    int input = int.Parse(File.ReadAllText(fileName));

    Console.WriteLine("Part 1: " + Part1(input));
    Console.WriteLine("Part 2: " + Part2(input));
    Console.ReadLine();
}

long Part1(int seekSquareNumber)
{
    int squareNumber = 0;

    foreach (Point point in GetSpiralPoints())
    {
        squareNumber++;

        if(squareNumber == seekSquareNumber)
        {
            return Math.Abs(point.X) + Math.Abs(point.Y);
        }
    }

    return 0;
}


long Part2(int minValue)
{
    Dictionary<Point, int> valuesAtPoints = [];
    valuesAtPoints.Add(new Point(0, 0), 1);

    Direction[] allDirections = [
        Direction.East,
        Direction.North,
        Direction.South,
        Direction.West,
        Direction.NorthEast,
        Direction.SouthEast,
        Direction.NorthWest,
        Direction.SouthWest
    ];

    foreach (Point point in GetSpiralPoints().Skip(1))
    {
        int valueAtPoint = 
            allDirections
            .Select(point.GetNeightboringPoint)
            .Where(p => valuesAtPoints.ContainsKey(p))
            .Sum(p => valuesAtPoints[p]);
        
        valuesAtPoints.Add(point, valueAtPoint);

        if (valueAtPoint > minValue) return valueAtPoint;
    }

    return 0;
}


IEnumerable<Point> GetSpiralPoints()
{
    Point point = new Point(0, 0);
    Direction currentDirection = Direction.East;

    int turns = 0;
    int straightLength = 1;
    int straightSteps = 0;

    yield return point;

    while (true)
    {
        point = point.GetNeightboringPoint(currentDirection);
        yield return point;

        straightSteps++;
        if (straightSteps == straightLength)
        {
            currentDirection = currentDirection.TurnLeft();
            straightSteps = 0;
            turns++;

            if (turns == 2)
            {
                straightLength++;
                turns = 0;
            }
        }
    }
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

            Direction.SouthWest => this with { X = this.X - 1, Y = this.Y + 1 },
            Direction.SouthEast => this with { X = this.X + 1, Y = this.Y + 1 },
            Direction.NorthEast => this with { X = this.X + 1, Y = this.Y - 1 },
            Direction.NorthWest => this with { X = this.X - 1, Y = this.Y - 1 },
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
    East,
    NorthEast,
    NorthWest,
    SouthEast,
    SouthWest
}
static class Extensions
{
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
