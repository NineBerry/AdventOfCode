// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\FlipFlop\2025\Day06\Sample.txt";
    int dimension = 8;
#else
    string fileName = @"D:\Dropbox\Work\FlipFlop\2025\Day06\Full.txt";
    int dimension = 1000;
#endif

    Point[] speeds = File.ReadAllLines(fileName).Select(Point.Parse).ToArray();
    Sky sky = new Sky(dimension, speeds);

    Console.WriteLine("Part 1: " + Part1(sky));
    Console.WriteLine("Part 2: " + Part2(sky));
    Console.WriteLine("Part 3: " + Part3(sky));

    Console.ReadLine();
}

long Part1(Sky sky)
{
    sky.Reset();
    
    sky.SimulateFlight(100);
    
    return sky.CountInFrame();
}

long Part2(Sky sky)
{
    sky.Reset();
    long sum = 0;
    
    foreach (var hour in Enumerable.Range(1, 1000))
    {
        sky.SimulateFlight(3600);
        sum+= sky.CountInFrame();
    }

    return sum;
}

long Part3(Sky sky)
{
    sky.Reset();
    long sum = 0;

    foreach (var year in Enumerable.Range(1, 1000))
    {
        sky.SimulateFlight(31556926);
        sum += sky.CountInFrame();
    }

    return sum;
}

class Sky
{
    private int Dimension;
    private Bird[] Birds;

    public Sky(int dimension, Point[] speeds)
    {
        this.Dimension = dimension;
        Birds = speeds.Select(speed => new Bird { Speed = speed, Position = new Point(0, 0)}).ToArray();
    }

    public void SimulateFlight(int seconds)
    {
        foreach(var bird in Birds)
        {
            bird.SimulateFlight(Dimension, seconds);
        }
    }

    public long CountInFrame()
    {
        long lowestCoordinateInFrame = Dimension / 4;
        long highestCoordinateInFrame = lowestCoordinateInFrame + (Dimension / 2) - 1;

        return Birds.Count(b => IsPointInFrame(b.Position));

        bool IsPointInFrame(Point point)
        {
            return IsCoordinateInFrame(point.X) && IsCoordinateInFrame(point.Y);
        }

        bool IsCoordinateInFrame(long coordinate)
        {
            return coordinate >= lowestCoordinateInFrame && coordinate <= highestCoordinateInFrame;
        }
    }

    public void Reset()
    {
        foreach (var bird in Birds)
        {
            bird.Position = new Point(0, 0);
        }
    }
}

class Bird
{
    public Point Speed;
    public Point Position;

    internal void SimulateFlight(int dimension, int seconds)
    {
        Position = new Point(Wrap(Position.X + Speed.X * seconds, dimension), Wrap(Position.Y + Speed.Y * seconds, dimension));
    }

    private long Wrap(long coordinate, long dimension)
    {
        return ActualMod(coordinate, dimension);
    }

    public static long ActualMod(long k, long n) { return ((k %= n) < 0) ? k + n : k; }

}

record struct Point(long X, long Y)
{
    public static Point Parse(string s)
    {
        int[] values = s.Split(',').Select(int.Parse).ToArray();
        return new Point(values[0], values[1]);
    }
}
