// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day19\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day19\Full.txt";
#endif

    string[] lines = File.ReadAllLines(fileName);

    Grid grid = new Grid(lines);
    var walkResult = grid.Walk();

    Console.WriteLine("Part 1: " + walkResult.Letters);
    Console.WriteLine("Part 2: " + walkResult.Steps);
    Console.ReadLine();
}

class Grid
{
    private string[] grid;

    public Grid(string[] text)
    {
        grid = [.. text];
        Width = grid.First().Length;
        Height = grid.Length;

        StartPoint = 
            Enumerable.Range(0, Width)
            .Select(x => new Point(x, 0))
            .Where(p => GetTile(p) == Tile.PathNS)
            .Single();
    }

    public readonly int Height;
    public readonly int Width;

    public Point StartPoint;

    public Tile GetTile(Point point)
    {
        if (!IsInGrid(point)) return Tile.Ground;

        return (Tile)grid[point.Y][point.X];
    }
    public bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }

    public (string Letters, int Steps) Walk()
    {
        string letters = "";
        int steps = 0;

        Point point = StartPoint;
        Direction direction = Direction.South;

        Tile tile = GetTile(point);

        while(tile != Tile.Ground)
        {
            if((char)tile is >= 'A' and <= 'Z')
            {
                letters += (char)tile;
            }
            
            (point, direction) = WalkStep(point, direction, tile);
            tile = GetTile(point);
            steps++;
        }

        return (letters, steps);
    }

    private (Point Point, Direction Direction) WalkStep(Point point, Direction direction, Tile tile)
    {
        switch (tile)
        {
            case Tile.Ground:
                throw new ApplicationException("Unexpected Ground");
            case Tile.Crossing:
                var nextDirection = direction.TurnLeft();
                var nextPoint = point.GetNeightboringPoint(nextDirection);
                
                if(GetTile(nextPoint) != Tile.Ground)
                {
                    direction = nextDirection;
                    point = nextPoint;
                }
                else
                {
                    direction = direction.TurnRight();
                    point = point.GetNeightboringPoint(direction);
                }
                break;
            default:
                point = point.GetNeightboringPoint(direction);
                break;
        }

        return (point, direction);
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
    East
}

enum Tile
{
    Ground = ' ',
    PathNS = '|',
    PathWE = '-',
    Crossing = '+',
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