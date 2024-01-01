using System.Drawing;
using System.Text.RegularExpressions;

string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day18\Sample.txt";
// string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day18\Full.txt";


List<Cell> polyLine = new List<Cell>();
Point currentPoint = new Point(0, 0);

string[] lines = File.ReadAllLines(fileName);
foreach (string trench in lines)
{
    currentPoint = DigTrench(currentPoint, polyLine, trench);
}

if (currentPoint != new Point(0, 0)) throw new ApplicationException("Line not closed");

// Get bounds and move cells inside normalized grid 
// so that all coordinates are positive and there is at least
// one empty row and column at each edge
var bounds = GetBounds(polyLine);
(polyLine, int width, int height) = NormalizePoints(polyLine, bounds);
var boundsNormalized = GetBounds(polyLine);

Console.WriteLine(bounds);
Console.WriteLine(polyLine.Count);
Console.WriteLine(boundsNormalized);
Console.WriteLine($"Width {width} Height {height}");


Grid grid = new Grid(polyLine, width, height);
grid.FillInsideOutside();

int trenchesAndInside = grid.CountTrenchesAndInside();
Console.WriteLine($"Trenches And Inside {trenchesAndInside}");


Console.WriteLine(grid);

Console.ReadLine();


(int MinX, int MinY, int MaxX, int MaxY) GetBounds(List<Cell> polyLine)
{
    int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;

    foreach (var cell in polyLine)
    {
        minX = Math.Min(minX, cell.Point.X);
        minY = Math.Min(minY, cell.Point.Y);
        maxX = Math.Max(maxX, cell.Point.X);
        maxY = Math.Max(maxY, cell.Point.Y);
    }

    return (minX, minY, maxX, maxY);
}

(List<Cell> polyLine, int width, int height) NormalizePoints(List<Cell> polyLine, (int MinX, int MinY, int MaxX, int MaxY) bounds)
{
    List<Cell> normalizedPolyLine = new();

    int addX = 0 - bounds.MinX + 1;
    int addY = 0 - bounds.MinY + 1;
    int width = bounds.MaxX - bounds.MinX + 3;
    int height = bounds.MaxY - bounds.MinY + 3;

    Point addPoint = new Point(addX, addY);
    foreach (var cell in polyLine)
    {
        normalizedPolyLine.Add(cell with { Point = cell.Point.Add(addPoint) });
    }

    return (normalizedPolyLine, width, height);

}


Point DigTrench(Point currentPoint, List<Cell> polyLine, string trench)
{

    Direction direction = (Direction)trench[0];
    int distance = int.Parse(Regex.Match(trench, "[0-9]+").Value);

    string hexColor = Regex.Match(trench, "[0-9a-f]{6}").Value;
    ColorConverter converter = new ColorConverter();
    Color color = (Color)converter.ConvertFromString("#" + hexColor)!;

    for (int i = 0; i < distance; i++)
    {
        currentPoint = currentPoint.GetNeightboringPoint(direction);

        Cell cell = new Cell { Point = currentPoint, Color = color, Tile = Tile.Trench };
        polyLine.Add(cell);
    }

    return currentPoint;
}


class Grid
{
    public readonly int Width;
    public readonly int Height;

    private Dictionary<Point, Cell> grid = new();

    public Grid(List<Cell> cells, int width, int height)
    {
        Width = width;
        Height = height;

        foreach (Cell cell in cells)
        {
            grid.Add(cell.Point, cell);
        }
    }

    public void FillInsideOutside()
    {
        FloodFill(new Point(0, 0), Tile.Outside);
        FillGroundInside();
    }

    private void FloodFill(Point startPoint, Tile setTile)
    {
        Stack<Point> toCheck = new();

        toCheck.Push(startPoint);

        while (toCheck.TryPop(out var point))
        {

            var cell = GetCell(point);

            if (cell.Tile == Tile.Ground)
            {
                grid[point] = cell with { Tile = setTile };

                var neighbors = AllDirections.Select(direction => point.GetNeightboringPoint(direction));

                foreach (var neighbor in neighbors)
                {
                    if (!IsInGrid(neighbor)) continue;
                    toCheck.Push(neighbor);
                }
            }
        }
    }

    private void FillGroundInside()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {

                var cell = GetCell(new Point(x, y));
                if (cell.Tile == Tile.Ground)
                {
                    grid[cell.Point] = cell with { Tile = Tile.Inside };
                }
            }
        }
    }

    public int CountTrenchesAndInside()
    {
        return grid.Values.Count(cell => cell.Tile is Tile.Trench or Tile.Inside);
    }

    public bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }
    private static Direction[] AllDirections = [Direction.East, Direction.North, Direction.South, Direction.West];

    private Cell GetCell(Point point)
    {
        if (!IsInGrid(point))
        {
            throw new ArgumentException("point outside grid");
        }

        if (!grid.TryGetValue(point, out var cell))
        {
            cell = new Cell { Point = point, Color = Color.Empty, Tile = Tile.Ground };
            grid.Add(point, cell);
        }

        return cell;
    }

    private char[] emptyMap = [];

    public override string ToString()
    {
        if (emptyMap is [])
        {
            string line = new string(Enumerable.Repeat('.', Width).ToArray());
            emptyMap = string.Join('\n', Enumerable.Range(0, Height).Select(s => line)).ToCharArray();
        }

        char[] map = (char[])emptyMap.Clone();

        foreach (var cell in grid.Values)
        {
            map[cell.Point.Y * (Width + 1) + cell.Point.X] = (char)cell.Tile;
        }

        return new string(map);
    }

}

record class Cell
{
    public Point Point { get; set; }
    public Color Color { get; set; }
    public Tile Tile { get; set; }
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
    public Point Add(Point added)
    {
        return new Point(X + added.X, Y + added.Y);
    }
}

enum Direction
{
    None = '\0',
    South = 'D',
    West = 'L',
    North = 'U',
    East = 'R'
}

enum Tile
{
    Ground = '.',
    Trench = '#',
    Inside = 'I',
    Outside = 'O'
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
