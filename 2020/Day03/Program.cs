// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day03\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2020\Day03\Full.txt";
#endif

    var lines = File.ReadAllLines(fileName);
    Field field = new Field(lines);

    Console.WriteLine("Part 1: " + Part1(field));
    Console.WriteLine("Part 2: " + Part2(field));
    Console.ReadLine();
}

long Part1(Field field)
{
    return field.SeenTreesWithSlope(3, 1);
}

long Part2(Field field)
{
    return
        field.SeenTreesWithSlope(1, 1) *
        field.SeenTreesWithSlope(3, 1) *
        field.SeenTreesWithSlope(5, 1) *
        field.SeenTreesWithSlope(7, 1) *
        field.SeenTreesWithSlope(1, 2);
}

public class Field
{
    public long Width;
    public long Height;

    private HashSet<Point> Trees = [];

    public Field(string[] lines)
    {
        Height = lines.Length;
        Width = lines[0].Length;

        InitTrees(lines);
    }

    private void InitTrees(string[] lines)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (lines[y][x] == '#')
                {
                    Trees.Add(new Point(x, y));
                }
            }
        }
    }

    public bool HasTree(Point point)
    {
        Point pointNormalized = new Point(point.X % Width, point.Y); 
        return Trees.Contains(pointNormalized);
    }

    public long SeenTreesWithSlope(int xDrift, int yDrift)
    {
        Point point = new Point(0, 0);
        int seenTrees = 0;

        while (point.Y < Height)
        {
            point.X += xDrift;
            point.Y += yDrift;

            if (HasTree(point)) seenTrees++;
        }

        return seenTrees;
    }
}

public record Point
{
    public Point(long x, long y)
    {
        X = x; 
        Y = y;   
    }

    public long X;
    public long Y;
}