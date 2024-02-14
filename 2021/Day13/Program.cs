// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day13\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day13\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);
    
    Point[] points = 
        input
        .TakeWhile(line => line != "")
        .Select(line => line.Split(',').Select(int.Parse))
        .Select(arr => new Point(arr.First(), arr.Last()))
        .ToArray();

    (char FoldCoordinate, int FoldPosition)[] foldInstructions = 
        input
        .SkipWhile(line => line != "")
        .Skip(1)
        .Select(s => (s[11], int.Parse(s.Substring(13))))
        .ToArray();

    Grid grid = new Grid(points);
    bool first = true;

    foreach (var instruction in foldInstructions)
    {
        grid.Fold(instruction.FoldCoordinate, instruction.FoldPosition);

        if (first)
        {
            Console.WriteLine("Part 1: " + grid.PointCount);
            first = false;
        }
    }


    Console.WriteLine("Part 2: \n" + grid.ToString());

    Console.ReadLine();
}

public class Grid
{
    public Grid(Point[] points)
    {
        Points = points.ToHashSet();
        UpdateBounds();
    }

    private void UpdateBounds()
    {
        Width = Points.Select(p => p.X).Max() + 1;
        Height = Points.Select(p => p.Y).Max() + 1;
    }

    public int Height;
    public int Width;

    private HashSet<Point> Points = [];

    private bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }

    public void Fold(char foldCoordinate, int foldPosition)
    {
        HashSet<Point> newPoints = [];

        foreach (var point in Points)
        {
            newPoints.Add(point.Fold(foldCoordinate, foldPosition));
        }

        Points = newPoints;
        UpdateBounds();
    }

    public long PointCount => Points.Count;

    private char[] emptyMap = null;
    public override string ToString()
    {
        if (emptyMap == null)
        {
            string line = new string(Enumerable.Repeat(' ', Width).ToArray());
            emptyMap = string.Join('\n', Enumerable.Range(0, Height).Select(s => line)).ToCharArray();
        }

        char[] map = (char[])emptyMap.Clone();

        foreach (var light in Points)
        {
            // +1 for line break
            map[light.Y * (Width + 1) + light.X] = '█';
        }

        return new string(map);
    }

}


public record Point(int X, int Y)
{
    public Point Fold(char foldCoordinate, int foldPosition)
    {
        return foldCoordinate switch
        {
            'x' => this with { X = FoldCoordinate(X, foldPosition) },
            'y' => this with { Y = FoldCoordinate(Y, foldPosition) },
            _ => throw new ArgumentException("Unknown Coordinate", nameof(foldCoordinate))
        };
    }

    private int FoldCoordinate(int coordinate, int foldPosition)
    {
        if (coordinate == foldPosition) throw new ApplicationException("Point on fold line");
        if (coordinate < foldPosition) return coordinate;
        return foldPosition - (coordinate - foldPosition);
    }
}

