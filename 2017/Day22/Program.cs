// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day22\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day22\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(lines));
    Console.WriteLine("Part 2: " + Part2(lines));
    Console.ReadLine();
}

long Part1(string[] lines)
{
    Part1Grid grid = new Part1Grid(lines);
    Point position = grid.StartPoint;
    Direction direction = Direction.North;

    int countInfected = 0;

    foreach(var i in Enumerable.Range(1, 10_000))
    {
        if(grid.Burst(ref position, ref direction))
        {
            countInfected++;
        }
    }

    return countInfected;
}

long Part2(string[] lines)
{
    Part2Grid grid = new Part2Grid(lines);
    Point position = grid.StartPoint;
    Direction direction = Direction.North;

    int countInfected = 0;

    foreach (var i in Enumerable.Range(1, 10_000_000))
    {
        if (grid.Burst(ref position, ref direction))
        {
            countInfected++;
        }
    }

    return countInfected;
}

class Part1Grid
{
    public Part1Grid(string[] lines)
    {
        Height = lines.Length;
        Width = lines.First().Length;

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                char tile = lines[y][x];
                if (tile == '#') Infected.Add(new Point(x, y));
            }
        }

        StartPoint = new Point((Width / 2), (Height / 2));
    }

    public bool Burst(ref Point position, ref Direction direction)
    {
        bool infected = Infected.Contains(position);
        direction = infected ? direction.TurnRight() : direction.TurnLeft();

        if (infected)
        {
            Infected.Remove(position);
        }
        else
        {
            Infected.Add(position);
        }

        position = position.GetNeightboringPoint(direction);

        return !infected;
    }

    public Point StartPoint;
    private HashSet<Point> Infected = new();

    public readonly int Height;
    public readonly int Width;
}


class Part2Grid
{
    public Part2Grid(string[] lines)
    {
        Height = lines.Length;
        Width = lines.First().Length;

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                char tile = lines[y][x];
                if (tile == '#') CellState.Add(new Point(x, y), State.Infected);
            }
        }

        StartPoint = new Point((Width / 2), (Height / 2));
    }

    public bool Burst(ref Point position, ref Direction direction)
    {
        State currentState = GetCellState(position);

        (direction, var newState) = currentState switch
        {
            State.Clean => (direction.TurnLeft(), State.Weakened),
            State.Weakened => (direction, State.Infected),
            State.Infected => (direction.TurnRight(), State.Flagged),
            State.Flagged => (direction.Opposite(), State.Clean),
        };

        SetCellState(position, newState);

        position = position.GetNeightboringPoint(direction);

        return currentState == State.Weakened;
    }

    public Point StartPoint;
    private Dictionary<Point, State> CellState = new();

    private State GetCellState(Point point)
    {
        if(CellState.TryGetValue(point, out var state))
        {
            return state;
        }

        return State.Clean;
    }

    private void SetCellState(Point point, State state)
    {
        CellState[point] = state;
    }

    public readonly int Height;
    public readonly int Width;

    public enum State
    {
        Clean, Weakened, Infected, Flagged
    }
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
}
enum Direction
{
    None,
    South,
    West,
    North,
    East,
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

    public static Direction Opposite(this Direction direction)
    {
        return direction switch
        {
            Direction.South => Direction.North,
            Direction.West => Direction.East,
            Direction.North => Direction.South,
            Direction.East => Direction.West,
            Direction.None => Direction.None,
            _ => throw new ArgumentException("Direction", nameof(direction))
        };
    }

}
