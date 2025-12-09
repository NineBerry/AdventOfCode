// #define Sample

using Rectangle = (Point CornerA, Point CornerB, long Area);

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
// Create set of border points
// Sort all possible rectangles by area descending and check to find first that works
// To check, check its four corners and four edges.
// The corners must be inside the loop that is they must be on a border or we must hit a border going in all four directions 
// When checking the edges, only check points after a border. Only those could be outside if we start inside

// Idea for performance improvement:
// Store border sections as lines and then try to intersect
// those with edges to test instead of testing edges by going tile to tile


public class MovieTheater
{
    private readonly Point[] redTiles;
    private readonly Rectangle[] rectangles;
    private readonly HashSet<Point> borderTiles = [];

    private readonly long minX;
    private readonly long maxX;
    private readonly long minY;
    private readonly long maxY;

    public MovieTheater(string[] lines)
    {
        redTiles = lines.Select(Point.Parse).ToArray();

        minX = redTiles.Min(t => t.X);
        maxX = redTiles.Max(t => t.X);
        minY = redTiles.Min(t => t.Y);
        maxY = redTiles.Max(t => t.Y);

        rectangles = MakeRectangles(redTiles).OrderByDescending(r => r.Area).ToArray();
        FillBorderTiles();
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
        if (!IsEdgeInsideLoop(new Point(minRectangleX, minRectangleY), new Point(maxRectangleX, minRectangleY), Direction.Right)) return false;
        if (!IsEdgeInsideLoop(new Point(minRectangleX, maxRectangleY), new Point(maxRectangleX, maxRectangleY), Direction.Right)) return false;

        if (!IsEdgeInsideLoop(new Point(minRectangleX, minRectangleY), new Point(minRectangleX, maxRectangleY), Direction.Down)) return false;
        if (!IsEdgeInsideLoop(new Point(maxRectangleX, minRectangleY), new Point(maxRectangleX, maxRectangleY), Direction.Down)) return false;
        return true;
    }

    private bool IsEdgeInsideLoop(Point start, Point end, Direction direction)
    {
        Point traverse = start;

        while (traverse != end)
        {
            Point next = traverse.GetNeightboringPoint(direction);
            if (borderTiles.Contains(traverse))
            {
                if(!IsPointInsideLoop(next)) 
                {
                    return false;
                }
            }
            traverse = next;
        }

        return true;
    }

    private Dictionary<Point, bool> pointsInLoopCache = [];

    private bool IsPointInsideLoop(Point point)
    {
        if (pointsInLoopCache.TryGetValue(point, out var result)) return result;

        result = (HitsBorder(point, Direction.Up)
        && HitsBorder(point, Direction.Down)
        && HitsBorder(point, Direction.Left)
        && HitsBorder(point, Direction.Right));

        pointsInLoopCache.Add(point, result);
        return result;

        bool HitsBorder(Point point, Direction direction)
        {
            Point traverse = point;
            while (!IsOutsideMovieTheater(traverse))
            {
                if (borderTiles.Contains(traverse))
                {
                    return true;
                }
                traverse = traverse.GetNeightboringPoint(direction);
            }

            return false;
        }
    }

    private bool IsOutsideMovieTheater(Point point)
    {
        return point.X < minX || point.X > maxX || point.Y < minY || point.Y > maxY;
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

    void FillBorderTiles()
    {
        foreach (var pair in redTiles.Zip(redTiles.Skip(1)))
        {
            FillBorderTilesLine(pair.First, pair.Second);
        }
        FillBorderTilesLine(redTiles.Last(), redTiles.First());
    }

    void FillBorderTilesLine(Point first, Point second)
    {
        if (first.Y == second.Y)
        {
            var minX = Math.Min(first.X, second.X);
            var maxX = Math.Max(first.X, second.X);

            for (long i = minX; i <= maxX; i++)
            {
                borderTiles.Add(new Point(i, first.Y));
            }
        }
        else
        {
            var minY = Math.Min(first.Y, second.Y);
            var maxY = Math.Max(first.Y, second.Y);

            for (long i = minY; i <= maxY; i++)
            {
                borderTiles.Add(new Point(first.X, i));
            }
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