#define Sample

using System.Text.RegularExpressions;
using Instruction = (int Distance, char TurnDirection);

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day22\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day22\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);
    string[] gridLines = lines.TakeWhile(s => s != "").ToArray();
    Instruction[] instructions = ParseInstructions(lines.Last());

    Grid gridPart1 = new Grid(gridLines, foldCube: false);
    Console.WriteLine("Part 1: " + Solve(gridPart1, instructions));

    Grid gridPart2 = new Grid(gridLines, foldCube: true);
    Console.WriteLine("Part 2: " + Solve(gridPart2, instructions));

    Console.ReadLine();
}

long Solve(Grid grid, Instruction[] instructions)
{
    var endPosition = grid.PerformWalk(grid.Start, Direction.East, instructions);
    return 1000 * (endPosition.Point.Y + 1) + 4 * (endPosition.Point.X + 1) + (int)endPosition.Direction;
}

Instruction[] ParseInstructions(string line)
{
    return 
        Regex.Matches(line, "(\\d+)([RL])?")
        .Select(m => (int.Parse(m.Groups[1].Value), m.Groups[2].Value.FirstOrDefault(' ')))
        .ToArray();
}

public class Grid
{
    public Grid(string[] input, bool foldCube)
    {
        Width = input.Max(l => l.Length);
        Height = input.Length;
        Start = new Point(input.First().IndexOf('.'), 0);

        InitializeWalls(input);

        if (foldCube)
        {
            InitializeMazeFoldCube(input);
        }
        else
        {
            InitializeMaze(input);
        }
    }

    private void InitializeWalls(string[] input)
    {
        for (int x = -1; x <= Width; x++)
        {
            for (int y = -1; y <= Height; y++)
            {
                Point point = new Point(x, y);
                char ch = GetTile(input, point);

                if (ch == '#')
                {
                    Walls.Add(point);
                }
            }
        }
    }

    private char GetTile(string[] input, Point point)
    {
        if (!IsInGrid(point)) return ' ';
        return input[point.Y].PadRight(Width)[point.X];
    }

    private void InitializeMaze(string[] input)
    {
        List<(Point Point, Direction Direction, string Name)> portalEnds = [];

        for (int x = -1; x <= Width; x++)
        {
            for (int y = -1; y <= Height; y++)
            {
                Point point = new Point(x, y);
                char ch = GetTile(input, point);

                if (ch == ' ')
                {
                    char left = GetTile(input, point.GetNeightboringPoint(Direction.West));
                    char right = GetTile(input, point.GetNeightboringPoint(Direction.East));
                    char top = GetTile(input, point.GetNeightboringPoint(Direction.North));
                    char bottom = GetTile(input, point.GetNeightboringPoint(Direction.South));

                    if (right is '.' or '#')
                    {
                        string name = "y" + y;
                        portalEnds.Add((point.GetNeightboringPoint(Direction.East), Direction.West, name));
                    }
                    if (left is '.' or '#')
                    {
                        string name = "y" + y;
                        portalEnds.Add((point.GetNeightboringPoint(Direction.West), Direction.East, name));
                    }
                    if (bottom is '.' or '#')
                    {
                        string name = "x" + x;
                        portalEnds.Add((point.GetNeightboringPoint(Direction.South), Direction.North, name));
                    }
                    if (top is '.' or '#')
                    {
                        string name = "x" + x;
                        portalEnds.Add((point.GetNeightboringPoint(Direction.North), Direction.South, name));
                    }
                }
            }
        }

        portalEnds = portalEnds.OrderBy(e => e.Name).ToList();

        foreach (var portalEnd in portalEnds)
        {
            Portals.Add(
                (portalEnd.Point, portalEnd.Direction),
                portalEnds
                .Where(p => (p.Name == portalEnd.Name) && p.Point != portalEnd.Point)
                .Select(p => p.Point)
                .Single());
        }
    }

    private void InitializeMazeFoldCube(string[] input)
    {
        throw new NotImplementedException();
    }


    public int Height;
    public int Width;
    public Point Start = new Point(0 , 0);

    private HashSet<Point> Walls = [];
    private Dictionary<(Point Point, Direction Direction), Point> Portals = [];

    private bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }

    private Point GetNeighboringPoint(
        Point point, Direction direction,
        Dictionary<(Point Point, Direction Direction), Point> portals)
    {
        if (portals.TryGetValue((point, direction), out var portalEnd))
        {
            return portalEnd;
        }
        else
        {
            return point.GetNeightboringPoint(direction);
        }
    }

    public (Point Point, Direction Direction) PerformWalk(Point startPoint, Direction startDirection, Instruction[] instructions)
    {
        Point point = startPoint;
        Direction direction = startDirection;

        foreach(var instruction in instructions)
        {
            PerformInstruction(ref point, ref direction, instruction);
        }

        return (point, direction);
    }

    private void PerformInstruction(ref Point point, ref Direction direction, Instruction instruction)
    {
        foreach(var _ in Enumerable.Range(1, instruction.Distance))
        {
            var next = GetNeighboringPoint(point, direction, Portals);
            if (Walls.Contains(next)) break;
            point = next;
        }

        direction = instruction.TurnDirection switch
        {
            'R' => direction.TurnRight(),
            'L' => direction.TurnLeft(),
            _ => direction
        };
    }
}

public record Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.West => this with { X = this.X - 1 },
            Direction.East => this with { X = this.X + 1 },

            Direction.South => this with { X = this.X, Y = this.Y + 1 },
            Direction.North => this with { X = this.X, Y = this.Y - 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public static Direction[] AllDirections = [Direction.West, Direction.East, Direction.North, Direction.South];
}

public enum Direction
{
    West = 2,
    East = 0,
    North = 3,
    South = 1,
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