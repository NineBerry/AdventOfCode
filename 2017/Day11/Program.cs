// #define Sample
{

#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day11\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2017\Day11\Full.txt";
#endif

    Direction[] directions = 
        File.ReadAllText(fileName)
        .Split(',')
        .Select(StringToDirection)
        .ToArray();

    var result = Walk(directions);

    Console.WriteLine("Part 1: " + result.EndDistance);
    Console.WriteLine("Part 2: " + result.MaxDistance);
    Console.ReadLine();
}

// Solutions based on https://www.redblobgames.com/grids/hexagons/

(int MaxDistance, int EndDistance) Walk(Direction[] directions)
{
    Point point = new Point(0, 0);

    int max = 0;

    foreach (Direction direction in directions)
    {
        point = point.GetNeightboringPoint(direction);

        max = Math.Max(max, point.GetDistance());
    }

    return (max, point.GetDistance());
}


Direction StringToDirection(string s)
{
    return s switch
    {
        "n" => Direction.North,
        "ne" => Direction.NorthEast,
        "nw" => Direction.NorthWest,
        "s" => Direction.South,
        "sw" => Direction.SouthWest,
        "se" => Direction.SouthEast,
        _ => throw new ApplicationException("invalid input")
    };
}


record struct Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.South => this with { Y = this.Y + 2 },
            Direction.North => this with { Y = this.Y - 2 },

            Direction.SouthWest => this with { X = this.X - 1, Y = this.Y + 1 },
            Direction.SouthEast => this with { X = this.X + 1, Y = this.Y + 1 },
            Direction.NorthEast => this with { X = this.X + 1, Y = this.Y - 1 },
            Direction.NorthWest => this with { X = this.X - 1, Y = this.Y - 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public int GetDistance()
    {
        var dcol = Math.Abs(X);
        var drow = Math.Abs(Y);
        return dcol + Math.Max(0, (drow - dcol) / 2);
    }

}

enum Direction
{
    None,
    South,
    North,
    NorthEast,
    NorthWest,
    SouthEast,
    SouthWest
}

