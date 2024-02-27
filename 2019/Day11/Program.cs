using System.Text;

{
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day11\Full.txt";
    long[] program = ReadProgram(fileName);

    Console.WriteLine("Part 1: " + Part1(program));
    Console.WriteLine("Part 2: \n" + Part2(program));

    Console.ReadLine();
}


long Part1(long[] program)
{
    Dictionary<Point, int> canvas = Paint(program, startWhite: false);
    return canvas.Keys.Count;   
}

string Part2(long[] program)
{
    Dictionary<Point, int> canvas = Paint(program, startWhite: true);
    return ShowPainting(canvas);
}

static long[] ReadProgram(string fileNamePart2)
{
    return File.ReadAllText(fileNamePart2).Split(',').Select(long.Parse).ToArray();
}

Dictionary<Point, int> Paint(long[] program, bool startWhite)
{
    Dictionary<Point, int> canvas = [];

    Point point = new Point();
    Direction direction = Direction.North;

    if (startWhite)
    {
        canvas[point] = 1;
    }

    Computer computer = new Computer(program);

    computer.Input += (c) =>
    {
        canvas.TryGetValue(point, out var color);
        return color;
    };

    bool awaitingColor = true;
    computer.Output += (c, value) =>
    {
        if (awaitingColor)
        {
            canvas[point] = (int)value;
        }
        else
        {
            direction = value switch
            {
                0 => direction.TurnLeft(),
                1 => direction.TurnRight(),
                _ => throw new ApplicationException("Unexptected turn instruction")
            };
            point = point.GetNeightboringPoint(direction);
        }

        awaitingColor = !awaitingColor;
    };

    computer.Run();

    return canvas;
}

string ShowPainting(Dictionary<Point, int> canvas)
{
    StringBuilder sb = new StringBuilder();

    int xMin = canvas.Keys.Min(p => p.X);
    int xMax = canvas.Keys.Max(p => p.X);
    int yMin = canvas.Keys.Min(p => p.Y);
    int yMax = canvas.Keys.Max(p => p.Y);

    sb.AppendLine();
    for (int y = yMin; y <= yMax; y++)
    {
        for (int x = xMin; x <= xMax; x++)
        {
            canvas.TryGetValue(new Point(x, y), out var color);
            char ch = (color == 1) ? '█' : ' ';
            sb.Append(ch);
        }
        sb.AppendLine();
    }

    return sb.ToString();
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

public enum Direction
{
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
