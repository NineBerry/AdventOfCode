// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\FlipFlop\2025\Day04\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\FlipFlop\2025\Day04\Full.txt";
#endif

    Point[] trash = File.ReadAllLines(fileName).Select(Point.Parse).ToArray();
    Console.WriteLine("Part 1: " + Part1(trash));
    Console.WriteLine("Part 2: " + Part2(trash));
    Console.WriteLine("Part 3: " + Part3(trash));

    Console.ReadLine();
}

long Part1(Point[] trash)
{
    Point[] full = [new Point(0, 0), .. trash];
    return full.Zip(trash).Select(pair => pair.First.ManhattanDistance(pair.Second)).Sum();
}

long Part2(Point[] trash)
{
    Point[] full = [new Point(0, 0), .. trash];
    return full.Zip(trash).Select(pair => pair.First.ChebyshevDistance(pair.Second)).Sum();
}

long Part3(Point[] trash)
{
    Point startPoint = new Point(0, 0);
    
    var orderedTrash = trash.OrderBy(p => startPoint.ManhattanDistance(p)).ToArray();
    
    Point[] full = [startPoint, .. orderedTrash];
    return full.Zip(orderedTrash).Select(pair => pair.First.ChebyshevDistance(pair.Second)).Sum();
}

record struct Point(int X, int Y)
{
    public int ManhattanDistance(Point point)
    {
        var xDistance = Math.Abs(point.X - X);
        var yDistance = Math.Abs(point.Y - Y);
        return xDistance + yDistance;
    }
    public int ChebyshevDistance(Point point)
    {
        var xDistance = Math.Abs(point.X - X);
        var yDistance = Math.Abs(point.Y - Y);
        return Math.Max(xDistance, yDistance);
    }

    public static Point Parse(string s)
    {
        int[] values = s.Split(',').Select(int.Parse).ToArray();
        return new Point(values[0], values[1]);
    }
}
