// #define Sample

using Rectangle = (Point CornerA, Point CornerB, long Area);
using Line = (Point Start, Point End);

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day09\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day09\Full.txt";
#endif

    MovieTheater movieTheater = new MovieTheater(File.ReadAllLines(fileName));

    Console.WriteLine("Part 1: " + movieTheater.GetBiggestRectangle());
    Console.WriteLine("Part 2: " + movieTheater.GetBiggestRectangleInsideLoop());
    Console.ReadLine();
}

// idea for Part2: 
// Sort all possible rectangles by area descending and check to find first that works
// To check, check its four corners and four edges.
// The corners must be inside the loop that is they must be on a border or we must hit a border going in all four directions 
// The edges must not intersect with a border

public class MovieTheater
{
    private readonly Point[] redTiles;
    private readonly Rectangle[] rectangles;
    private List<Line> verticalLinesWithoutCorners = [];
    private List<Line> horizontalLinesWithoutCorners = [];
    private List<Line> verticalLinesWithCorners = [];
    private List<Line> horizontalLinesWithCorners = [];

    public MovieTheater(string[] lines)
    {
        redTiles = lines.Select(Point.Parse).ToArray();
        rectangles = MakeRectangles(redTiles).OrderByDescending(r => r.Area).ToArray();
        AddBorderLines();
    }

    public long GetBiggestRectangle()
    {
        return rectangles.First().Area;
    }

    public long GetBiggestRectangleInsideLoop()
    {
        return rectangles.First(IsInsideLoop).Area;
    }

    private bool IsInsideLoop(Rectangle rectangle)
    {
        long minRectangleX = Math.Min(rectangle.CornerA.X, rectangle.CornerB.X);
        long minRectangleY = Math.Min(rectangle.CornerA.Y, rectangle.CornerB.Y);
        long maxRectangleX = Math.Max(rectangle.CornerA.X, rectangle.CornerB.X);
        long maxRectangleY = Math.Max(rectangle.CornerA.Y, rectangle.CornerB.Y);

        // Check corners inside loop
        if (!IsPointInsideLoop(new Point(minRectangleX, minRectangleY))) return false;
        if (!IsPointInsideLoop(new Point(minRectangleX, maxRectangleY))) return false;
        if (!IsPointInsideLoop(new Point(maxRectangleX, minRectangleY))) return false;
        if (!IsPointInsideLoop(new Point(maxRectangleX, maxRectangleY))) return false;

        // Check edges inside loop
        if (!IsEdgeInsideLoop(new Point(minRectangleX + 1, minRectangleY), new Point(maxRectangleX - 1, minRectangleY), Direction.Right)) return false;
        if (!IsEdgeInsideLoop(new Point(minRectangleX + 1, maxRectangleY), new Point(maxRectangleX - 1, maxRectangleY), Direction.Right)) return false;

        if (!IsEdgeInsideLoop(new Point(minRectangleX, minRectangleY + 1), new Point(minRectangleX, maxRectangleY - 1), Direction.Down)) return false;
        if (!IsEdgeInsideLoop(new Point(maxRectangleX, minRectangleY + 1), new Point(maxRectangleX, maxRectangleY - 1), Direction.Down)) return false;

        return true;
    }

    private bool IsEdgeInsideLoop(Point start, Point end, Direction direction)
    {
        // We need to use lines without corners, because intersecting with corners
        // does not leave the loop

        if(direction is Direction.Down)
        {
            return horizontalLinesWithoutCorners.All(l => !Intersects(l, (start, end)));
        }
        else
        {
            return verticalLinesWithoutCorners.All(l => !Intersects((start, end), l));
        }
    }

    bool Intersects(Line horizontal, Line vertical)
    {
        bool xInRange = horizontal.Start.X <= vertical.Start.X && vertical.Start.X <= horizontal.End.X;
        bool yInRange = vertical.Start.Y <= horizontal.Start.Y && horizontal.Start.Y <= vertical.End.Y;

        return xInRange && yInRange;
    }

    private Dictionary<Point, bool> pointsInLoopCache = [];

    private bool IsPointInsideLoop(Point point)
    {
        if (pointsInLoopCache.TryGetValue(point, out var result)) return result;

        result = horizontalLinesWithCorners.Any(l => Intersects(l, (new Point(point.X, 0), point))) 
            && horizontalLinesWithCorners.Any(l => Intersects(l, (point, new Point(point.X, long.MaxValue))))
            && verticalLinesWithCorners.Any(l => Intersects((new Point(0, point.Y), point), l))
            && verticalLinesWithCorners.Any(l => Intersects((point, new Point(long.MaxValue, point.Y)), l));

        pointsInLoopCache.Add(point, result);
        
        return result;
    }

    private Rectangle[] MakeRectangles(Point[] tiles)
    {
        List<Rectangle> result = [];

        for (int i = 0; i < tiles.Length - 1; i++)
        {
            for (int j = i + 1; j < tiles.Length; j++)
            {
                var area = Point.CalculateAreaFromTwoPoints(tiles[i], tiles[j]);
                result.Add((tiles[i], tiles[j], area));
            }
        }

        return result.ToArray();
    }

    void AddBorderLines()
    {
        foreach (var pair in redTiles.Zip(redTiles.Skip(1)))
        {
            AddBorderLine(pair.First, pair.Second);
        }
        AddBorderLine(redTiles.Last(), redTiles.First());
    }

    void AddBorderLine(Point first, Point second)
    {
        if (first.Y == second.Y)
        {
            var minX = Math.Min(first.X, second.X);
            var maxX = Math.Max(first.X, second.X);

            horizontalLinesWithoutCorners.Add((new Point(minX + 1, first.Y), new Point(maxX - 1, first.Y)));
            horizontalLinesWithCorners.Add((new Point(minX, first.Y), new Point(maxX, first.Y)));
        }
        else
        {
            var minY = Math.Min(first.Y, second.Y);
            var maxY = Math.Max(first.Y, second.Y);

            verticalLinesWithoutCorners.Add((new Point(first.X, minY + 1), new Point(first.X, maxY - 1)));
            verticalLinesWithCorners.Add((new Point(first.X, minY), new Point(first.X, maxY)));
        }
    }
}

record struct Point(long X, long Y)
{
    public static Point Parse(string input)
    {
        var numbers = input.Split(",").Select(long.Parse).ToArray();
        return new(numbers[0], numbers[1]);
    }

    public static long CalculateAreaFromTwoPoints(Point a, Point b)
    {
        long xDistance = Math.Abs(a.X - b.X) + 1;
        long yDistance = Math.Abs(a.Y - b.Y) + 1;

        return xDistance * yDistance;
    }

    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.Left => this with { X = X - 1 },
            Direction.Right => this with { X = X + 1 },
            Direction.Down => this with { Y = Y + 1 },
            Direction.Up => this with { Y = Y - 1 },

            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }
}

enum Direction
{
    Left,
    Right,
    Up,
    Down
}