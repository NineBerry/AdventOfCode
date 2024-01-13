// #define Sample

using System.Text;
using System.Text.RegularExpressions;

{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day10\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day10\Full.txt";
#endif

    MovingPoint[] points =
        File.ReadAllLines(fileName)
        .Select(s => new MovingPoint(s))
        .ToArray();

    var message = FindMessage(points);

    Console.WriteLine("Part 1: " + Part1(message));
    Console.WriteLine("Part 2: " + Part2(message));
    Console.ReadLine();
}

string Part1((HashSet<Point> Stars, int Duration) message)
{
    return ToString(message.Stars);
}

int Part2((HashSet<Point> Stars, int Duration) message)
{
    return message.Duration;
}

(HashSet<Point> Stars, int Duration) FindMessage(MovingPoint[] points)
{
    MovingPoint[] previous = [.. points];
    MovingPoint[] current = [.. points];
    int previousWidth = previous.Max(p => p.Position.X) - previous.Min(p => p.Position.X) + 1; ;
    int width = previousWidth;
    int seconds = 0;

    while (previousWidth >= width)
    {
        previous = [.. current];
        previousWidth = width;

        current = previous.Select(p => p.Move()).ToArray();
        width = current.Max(p => p.Position.X) - current.Min(p => p.Position.X) + 1;
        seconds++;
    }

    var stars = previous.Select(p => p.Position).ToHashSet();

    return (stars, seconds - 1);
}


string ToString(HashSet<Point> stars)
{
    StringBuilder sb = new StringBuilder();
    
    int xMin = stars.Min(p => p.X) - 1;
    int xMax = stars.Max(p => p.X) + 1;
    int yMin = stars.Min(p => p.Y) - 1;
    int yMax = stars.Max(p => p.Y) + 1;

    sb.AppendLine();
    for (int y = yMin; y <= yMax; y++)
    {
        for (int x = xMin; x <= xMax; x++)
        {
            char ch = stars.Contains(new Point(x, y)) ? '█' : ' ';
            sb.Append(ch);
        }
        sb.AppendLine();
    }

    return sb.ToString();
}

public record Point(int X, int Y)
{
    internal Point Move(Point movement)
    {
        return new Point(X + movement.X, Y + movement.Y);
    }
}

public record MovingPoint
{
    public MovingPoint(string input)
    {
        int[] values = Regex.Matches(input, "-?\\d+").Select(m => int.Parse(m.Value)).ToArray();

        Position= new Point(values[0], values[1]);
        Movement = new Point(values[2], values[3]);
    }

    public Point Position;
    public Point Movement;

    public MovingPoint Move()
    {
        return this with { Position = Position.Move(Movement) };
    }
}