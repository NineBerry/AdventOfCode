// #define Sample

{
#if Sample
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2018\Day13\Sample.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2018\Day13\Sample2.txt";
#else
    string fileNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2018\Day13\Full.txt";
    string fileNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2018\Day13\Full.txt";
#endif

    Grid gridPart1 = new Grid(File.ReadAllLines(fileNamePart1));
    Console.WriteLine("Part 1: " + Part1(gridPart1));

    Grid gridPart2 = new Grid(File.ReadAllLines(fileNamePart2));
    Console.WriteLine("Part 2: " + Part2(gridPart2));
    Console.ReadLine();
}

string Part1(Grid grid)
{
    Point point = grid.SimulateUntilCrash();
    return point.ToString();
}

string Part2(Grid grid)
{
    Point point = grid.SimulateUntilLastCar();
    return point.ToString();
}

class Grid
{
    private string[] grid;

    public readonly int Height;
    public readonly int Width;

    public Cart[] Carts;

    public Grid(string[] text)
    {
        grid = [.. text];
        Width = grid.First().Length;
        Height = grid.Length;

        Carts = InitCars();
    }

    private Cart[] InitCars()
    {
        List<Cart> carts = [];

        for(int x=0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Point point = new Point(x, y);
                Tile tile = GetTile(point);

                foreach(var direction in AllDirections)
                {
                    if((char)tile == (char)direction)
                    {
                        carts.Add(new Cart { Point = point, Direction = (Direction)tile});
                    }
                }
            }
        }

        return carts.ToArray();  
    }


    public Tile GetTile(Point point)
    {
        return (Tile)grid[point.Y][point.X];
    }

    internal Point SimulateUntilCrash()
    {
        while (true)
        {
            var sortedCarts = Carts.OrderBy(c => c.Point.Y).ThenBy(c => c.Point.X);

            foreach (var car in sortedCarts)
            {
                car.Move(this);

                var crash =
                    Carts
                    .Select(c => c.Point)
                    .GroupBy(p => p)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);

                if (crash.Any()) return crash.First();
            }
        }
    }

    internal Point SimulateUntilLastCar()
    {
        while (Carts.Length > 1)
        {
            var sortedCarts = Carts.OrderBy(c => c.Point.Y).ThenBy(c => c.Point.X);

            foreach (var car in sortedCarts)
            {
                if (car.Removed) continue;
                    
                car.Move(this);

                var crash =
                    Carts
                    .Where(c => !c.Removed)
                    .GroupBy(p => p.Point)
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g.AsEnumerable())
                    .ToArray();

                if (crash.Any())
                {
                    foreach(var c in crash)
                    {
                        c.Removed = true;
                    }
                }
            }

            Carts = Carts.Where(c => !c.Removed).ToArray();
        }

        return Carts.First().Point;
    }

    public Direction[] AllDirections = [Direction.West, Direction.South, Direction.East, Direction.North];
}

record class Cart
{
    public Point Point;
    public Direction Direction;
    public TurnMode TurnMode = TurnMode.TurnLeft;
    public bool Removed = false;

    public void Move(Grid grid)
    {
        Tile tile = grid.GetTile(Point);

        switch (tile)
        {
            case Tile.Crossing:
                Direction = Direction.ApplyTurnMode(TurnMode);
                TurnMode = TurnMode.Next();
                break;
            case Tile.Slash:
                Direction = Direction.ApplySlash();
                break;
            case Tile.BackSlash:
                Direction = Direction.ApplyBackSlash();
                break;
        }

        Point = Point.GetNeightboringPoint(Direction);
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

    public override string ToString()
    {
        return $"{X},{Y}";
    }
}

enum TurnMode
{
    TurnLeft,
    GoStraight,
    TurnRight
}

enum Direction
{
    South = 'v',
    West = '<',
    North = '^',
    East = '>'
}

enum Tile
{
    Crossing = '+',
    Slash = '/',
    BackSlash = '\\'
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

    public static Direction ApplySlash(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.East,
            Direction.West => Direction.South,
            Direction.South => Direction.West,
            Direction.East => Direction.North,
            _ => throw new ArgumentException("Invalid direction", nameof(direction)),
        };
    }

    public static Direction ApplyBackSlash(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.West,
            Direction.West => Direction.North,
            Direction.South => Direction.East,
            Direction.East => Direction.South,
            _ => throw new ArgumentException("Invalid direction", nameof(direction)),
        };
    }

    public static Direction ApplyTurnMode(this Direction direction, TurnMode turnMode)
    {
        return turnMode switch
        {
            TurnMode.TurnLeft => direction.TurnLeft(),
            TurnMode.TurnRight => direction.TurnRight(),
            TurnMode.GoStraight => direction,
            _ => throw new ArgumentException("Invalid TurnMode", nameof(turnMode)),
        };
    }

    public static TurnMode Next(this TurnMode mode)
    {
        return mode switch
        {
            TurnMode.TurnLeft => TurnMode.GoStraight,
            TurnMode.GoStraight => TurnMode.TurnRight,
            TurnMode.TurnRight => TurnMode.TurnLeft,
            _ => throw new ArgumentException("Unknown TurnMode", nameof(mode)),
        };
    }

}