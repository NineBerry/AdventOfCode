using System.IO;
using System.Text.RegularExpressions;

/// We build a polyline from the given instructions.
/// 
/// Because the line has no width, our border cells are sometimes
/// on the inside and sometimes on the outside of the polygon created
/// by the polyline.
/// 
/// To calculate the whole area we calculate the area of the polygon using a
/// standard algorithm and then add those border cells outside the polygon.
/// 
/// See "Schema.png" for illustrations.
/// 
/// Whether the outside of the polygon is on the right or left of our traversal 
/// can be determined by looking at the direction of the line in any of the edge
/// rows or columns. We use the top row (minimum Y)


// string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day18\Sample.txt";
string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day18\Full.txt";

string[] input = File.ReadAllLines(fileName);

long part1 = SolvePuzzle(input, LineParserPart1);
Console.WriteLine("Part 1: " + part1);

long part2 = SolvePuzzle(input, LineParserPart2);
Console.WriteLine("Part 2: " + part2);

Console.ReadLine();

long SolvePuzzle(string[] input, Func<string, (Direction, long)> lineParser)
{
    DigState digState = new DigState();
    digState.CurrentPoint = new Point(0, 0);

    foreach (string line in input)
    {
        var (direction, distance) = lineParser(line);
        DigTrench(direction, distance, digState);
    }

    bool outsideIsRight = DecideWhetherOutsideIsRight(digState.polyLine);

    long outsideCellsToAdd = outsideIsRight ? digState.CellsRight : digState.CellsLeft;

    var area = PolygonArea(digState.polyLine.ToArray());
    var fullArea = area + outsideCellsToAdd;

    return fullArea;
}

void DigTrench(Direction direction, long distance, DigState state)
{
    var nextPoint = state.CurrentPoint.GetNeightboringPoint(direction, distance);
    state.polyLine.Add(nextPoint);

    (long left, long right) = CountLeftAndRightCells(state.PreviousDirection, direction, distance);

    state.CellsLeft += left;
    state.CellsRight += right;
    state.PreviousDirection = direction;
    state.CurrentPoint = nextPoint;
}

(long Left, long Right) CountLeftAndRightCells(Direction previousDirection, Direction direction, long  distance)
{
    (long Left, long Right) straight = direction switch
    {
        Direction.South or Direction.West => (distance - 1, 0),
        Direction.North or Direction.East => (0, distance - 1),
        _ => throw new ApplicationException("invalid direction")
    };

    (long Left, long Right) cornerCase = (previousDirection, direction) switch
    {
        (Direction.North, Direction.East or Direction.West) => (0, 1),

        (Direction.West, Direction.North or Direction.South) => (1, 0),

        (Direction.South, Direction.East) => (0, 1),
        (Direction.South, Direction.West) => (1, 0),


        (Direction.East, Direction.North) => (0, 1),
        (Direction.East, Direction.South) => (1, 0),

        (Direction.None, _) => (0, 0),

        _ => throw new ApplicationException("Unexpected sequence of directions")
    };

    (long Left, long Right) combined = 
        (straight.Left + cornerCase.Left, straight.Right + cornerCase.Right);

    return combined;
}

// Use top line to decide whether the inside is on the right or left side
// of the path we have traversed
static bool DecideWhetherOutsideIsRight(List<Point> polyLine)
{
    long minY = polyLine.Min(p => p.Y);

    var topPointIndex = polyLine.FindIndex(p => p.Y == minY);
    var topPointFirst = polyLine[topPointIndex];
    var topPointSecond = polyLine[topPointIndex + 1];

    bool outsideIsRight = topPointFirst.X > topPointSecond.X;
    return outsideIsRight;
}


// From https://stackoverflow.com/a/2432482/101087
long PolygonArea(Point[] polygon)
{
    int i, j;
    long area = 0;

    for (i = 0; i < polygon.Length; i++)
    {
        j = (i + 1) % polygon.Length;

        area += ((polygon[i].X) * (polygon[j].Y));
        area -= ((polygon[i].Y) * (polygon[j].X));
    }

    area /= 2;
    return (area < 0 ? -area : area);
}

(Direction, long) LineParserPart1(string line)
{
    Direction direction = (Direction)line[0];
    int distance = int.Parse(Regex.Match(line, "[0-9]+").Value);

    return (direction, distance);
}

(Direction, long) LineParserPart2(string line)
{
    string hexColor = Regex.Match(line, "[0-9a-f]{6}").Value;

    long distance = Convert.ToInt64(hexColor.Substring(0, 5), 16);
    Direction direction = hexColor[5] switch
    {
        '0' => Direction.East,
        '1' => Direction.South,
        '2' => Direction.West,
        '3' => Direction.North,
        _ => throw new ApplicationException("Unknown Direction")
    };

    return (direction, distance);
}

class DigState
{
    public long CellsRight = 0;
    public long CellsLeft = 0;
    public Direction PreviousDirection = Direction.None;
    public Point CurrentPoint;
    public List<Point> polyLine = new List<Point>();
}

record struct Point(long X, long Y)
{
    public Point GetNeightboringPoint(Direction direction, long distance)
    {
        return direction switch
        {
            Direction.South => this with { Y = this.Y + distance },
            Direction.West => this with { X = this.X - distance },
            Direction.North => this with { Y = this.Y - distance },
            Direction.East => this with { X = this.X + distance },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
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