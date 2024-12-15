// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day15\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2024\Day15\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);
    string[] gridLines = lines.TakeWhile(s => s != "").ToArray();
    Direction[] directions = Grid.ParseDirections(lines.SkipWhile(s => s != "").Skip(1).ToArray());

    Console.WriteLine("Part 1: " + Solve(gridLines, directions, stretched: false));
    Console.WriteLine("Part 2: " + Solve(gridLines, directions, stretched: true));
    Console.ReadLine();
}

long Solve(string[] gridLines, Direction[] directions, bool stretched)
{
    Grid grid = new Grid(gridLines, stretched);   
    grid.Move(directions);
    // Console.WriteLine(grid.DrawMap());
    return grid.GetBoxesGPSSum();
}

// Idea for part 2 is:
// * Adapt x coordinates during parsing of input
// * Duplicate Walls during parsing
// * In Boxes, we only keep the left part of the box
// * IsBox can check whether there is a box at a given Point
//   and if yes, also returns the left part of the box
// * In MoveBoxes we need to be able to decide which target
//   points to check based on direction we are going

class Grid
{
    private bool Stretched;
    private int Width;
    private int Height;

    private Point RobotPosition;
    private HashSet<Point> Walls = [];
    private HashSet<Point> Boxes = [];

    public Grid(string[] lines, bool stretched)
    {
        Stretched = stretched;
        int xFactor = stretched ? 2 : 1;
        
        Width = lines[0].Length * xFactor;
        Height = lines.Length;

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                Point point = new Point(x * xFactor, y);
                char tile = lines[y][x];

                switch (tile)
                {
                    case '.':
                        // Just ground, do nothing
                        break;
                    case '#':
                        Walls.Add(point);
                        if(Stretched) Walls.Add(point.GetNeightboringPoint(Direction.East));
                        break;
                    case 'O':
                        Boxes.Add(point);
                        break;
                    case '@':
                        RobotPosition = point;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void Move(Direction[] directionList)
    {
        foreach (var direction in directionList) Move(direction);
    }

    public void Move(Direction direction)
    {
        Point neighbor = RobotPosition.GetNeightboringPoint(direction);

        if (MoveBoxes(neighbor, direction, simulateOnly: true))
        {
            MoveBoxes(neighbor, direction, simulateOnly: false);
            RobotPosition = neighbor;
        }
    }

    private bool MoveBoxes(Point from, Direction direction, bool simulateOnly)
    {
        if (Walls.Contains(from)) return false;
        if (!IsBox(from, out Point fromActualPoint)) return true;

        Point actualTarget = fromActualPoint.GetNeightboringPoint(direction);
        List<Point> nextToMove = [actualTarget];

        if (Stretched)
        {
            switch (direction)
            {
                case Direction.South:
                case Direction.North:
                    // If we go north or south, we also need to check right part of box
                    nextToMove.Add(actualTarget.GetNeightboringPoint(Direction.East));
                    break;
                case Direction.East:
                    // If we go east, we skip right part of box
                    nextToMove.Remove(actualTarget);
                    nextToMove.Add(actualTarget.GetNeightboringPoint(Direction.East));
                    break;
            }
        }

        bool result = true;
        foreach (var nextPart in nextToMove)
        {
            result = result && MoveBoxes(nextPart, direction, simulateOnly); ;
        }

        if (!simulateOnly && result)
        {
            Boxes.Remove(fromActualPoint);
            Boxes.Add(actualTarget);
        }

        return result;
    }

    private bool IsBox(Point point, out Point actualBox)
    {
        if (Boxes.Contains(point))
        {
            actualBox = point;
            return true;
        }

        if (Stretched)
        {
            var west = point.GetNeightboringPoint(Direction.West);
            if (Boxes.Contains(west))
            {
                actualBox = west;
                return true;
            }
        }

        actualBox = new();
        return false;
    }

    public long GetBoxesGPSSum()
    {
        return Boxes.Sum(b => b.GPS);
    }

    private char[]? emptyMap = null;
    public string DrawMap()
    {
        if (emptyMap == null)
        {
            string line = new string(Enumerable.Repeat('.', Width).ToArray());
            emptyMap = string.Join('\n', Enumerable.Range(0, Height).Select(s => line)).ToCharArray();
        }

        char[] map = (char[])emptyMap.Clone();

        foreach (var wall in Walls)
        {
            map[wall.Y * (Width + 1) + wall.X] = '#';
        }

        foreach (var box in Boxes)
        {
            if (Stretched)
            {
                map[box.Y * (Width + 1) + box.X] = '[';
                map[box.Y * (Width + 1) + box.X + 1] = ']';
            }
            else
            {
                map[box.Y * (Width + 1) + box.X] = 'O';
            }
        }

        map[RobotPosition.Y * (Width + 1) + RobotPosition.X] = '@';

        return new string(map);
    }

    public static Direction[] ParseDirections(string[] input)
    {
        return input.SelectMany(ParseDirections).ToArray();
    }

    private static Direction[] ParseDirections(string input)
    {
        return input.Select(ch => (Direction)ch).ToArray();
    }
}

enum Direction
{
    South = 'v',
    West = '<',
    North = '^',
    East = '>',
}

record struct Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction, int count = 1)
    {
        return direction switch
        {
            Direction.South => this with { Y = Y + count },
            Direction.West => this with { X = X - count },
            Direction.North => this with { Y = Y - count },
            Direction.East => this with { X = X + count },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public long GPS => 100 * Y + X;
}
