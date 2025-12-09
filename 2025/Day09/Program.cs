// #define Sample

using System.Text.RegularExpressions;
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
// Create sets of border points
// Sort all possible rectangles by area descending and check to find first that works
// To check, check its four corners and four edges.
// The corners must be inside the loop that is they must be on a border or we must hit a border going in all four directions 
// The edges may not cross a border (except directly neighboring borders)

// 4670137716 too high


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
        // Console.WriteLine(rectangle.Area);
        
        long minRectangleX = Math.Min(rectangle.CornerA.X, rectangle.CornerB.X);
        long minRectangleY = Math.Min(rectangle.CornerA.Y, rectangle.CornerB.Y);
        long maxRectangleX = Math.Max(rectangle.CornerA.X, rectangle.CornerB.X);
        long maxRectangleY = Math.Max(rectangle.CornerA.Y, rectangle.CornerB.Y);

        // Check corners inside loop
        if (!IsInsideLoop(new Point(minRectangleX, minRectangleY))) return false;
        if (!IsInsideLoop(new Point(minRectangleX, maxRectangleY))) return false;
        if (!IsInsideLoop(new Point(maxRectangleX, minRectangleY))) return false;
        if (!IsInsideLoop(new Point(maxRectangleX, maxRectangleY))) return false;

        // Check edges inside loop
        if (!CheckEdgeInsideLoop(new Point(minRectangleX, minRectangleY), new Point(maxRectangleX, minRectangleY), Direction.Right)) return false;
        if (!CheckEdgeInsideLoop(new Point(minRectangleX, maxRectangleY), new Point(maxRectangleX, maxRectangleY), Direction.Right)) return false;

        if (!CheckEdgeInsideLoop(new Point(minRectangleX, minRectangleY), new Point(minRectangleX, maxRectangleY), Direction.Down)) return false;
        if (!CheckEdgeInsideLoop(new Point(maxRectangleX, minRectangleY), new Point(maxRectangleX, maxRectangleY), Direction.Down)) return false;
        return true;
    }

    private bool CheckEdgeInsideLoop(Point start, Point end, Direction direction)
    {
        Point traverse = start;

        while (traverse != end)
        {
            Point next = traverse.GetNeightboringPoint(direction);
            if (borderTiles.Contains(traverse))
            {
                if(!IsInsideLoop(next)) return false;
            }
            traverse = next;
        }

        return true;
    }

    private Dictionary<Point, bool> pointsInLoop = [];

    private bool IsInsideLoop(Point point)
    {
        if (pointsInLoop.TryGetValue(point, out var result)) return result;

        result = (HitsBorder(point, Direction.Up)
        && HitsBorder(point, Direction.Down)
        && HitsBorder(point, Direction.Left)
        && HitsBorder(point, Direction.Right));

        pointsInLoop.Add(point, result);
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
                Point newBorderTile = new Point(i, first.Y);

                if (i != minX && i != maxX)
                {
                    if (borderTiles.Contains(newBorderTile.GetNeightboringPoint(Direction.Up))) Console.WriteLine("Neibor");
                    if (borderTiles.Contains(newBorderTile.GetNeightboringPoint(Direction.Down))) Console.WriteLine("Neibor");
                }

                borderTiles.Add(newBorderTile);
            }
        }
        else
        {
            var minY = Math.Min(first.Y, second.Y);
            var maxY = Math.Max(first.Y, second.Y);

            for (long i = minY; i <= maxY; i++)
            {
                Point newBorderTile = new Point(first.X, i);
                
                if (i != minY && i != maxY)
                {
                    if (borderTiles.Contains(newBorderTile.GetNeightboringPoint(Direction.Left))) Console.WriteLine("Neibor");
                    if (borderTiles.Contains(newBorderTile.GetNeightboringPoint(Direction.Right))) Console.WriteLine("Neibor");
                }
                
                borderTiles.Add(new Point(first.X, i));
            }
        }
    }

}

/*
long Part2(Point[] tiles, Rectangle[] rectangles)
{
    HashSet<Point> borderPoints = [];

    FillBorderPoints(tiles, borderPoints);

    var sortedRectangles = rectangles.OrderByDescending(r => r.Area);

    var biggest = sortedRectangles.First(r => RectangleFits(r, borderPoints));

    return biggest.Area;
}
*/

/*
bool RectangleFits(Rectangle rectangle, HashSet<Point> borderPoints)
{
    long minX = Math.Min(rectangle.CornerA.X, rectangle.CornerB.X);
    long minY = Math.Min(rectangle.CornerA.Y, rectangle.CornerB.Y);
    long maxX = Math.Max(rectangle.CornerA.X, rectangle.CornerB.X);
    long maxY = Math.Max (rectangle.CornerA.Y, rectangle.CornerB.Y);

    return
        CheckHorizontalEdge(new Point(minX, minY), new Point(maxX, minY), borderPoints)
        && CheckHorizontalEdge(new Point(minX, maxY), new Point(maxX, maxY), borderPoints)
        && CheckVerticalEdge(new Point(minX, minY), new Point(minX, maxY), borderPoints)
        && CheckVerticalEdge(new Point(maxX, minY), new Point(maxX, minY), borderPoints);
}

bool CheckHorizontalEdge(Point start, Point end, HashSet<Point> verticalBorders)
{
    return true;
}

bool CheckVerticalEdge(Point start, Point end, HashSet<Point> horizontalBorders)
{
    return true;

}


*/



record struct Point(long X, long Y)
{
    public static Point Parse(string input)
    {
        var numbers = Tools.ParseLongs(input);
        return new(numbers[0], numbers[1]);
    }

    public override string ToString()
    {
        return $"{X},{Y}";
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
 
static class Tools
{
    public static long[] ParseLongs(string input)
    {
        return
            Regex.Matches(input, @"\d+")
            .Select(m => long.Parse(m.Value))
            .ToArray();
    }
}
