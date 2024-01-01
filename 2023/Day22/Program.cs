// #define Sample
using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day22\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day22\Full.txt";
#endif

    Brick[] bricks =
        File.ReadAllLines(fileName)
        .Select(s => new Brick(s))
        .OrderBy(b => b.FirstCoordinate.Z)
        .ToArray();

    Playground playground = new();
    foreach (var brick in bricks)
    {
        playground.PlaceBrick(brick);
    }

    Console.WriteLine("Part 1: " + Part1(bricks));
    Console.WriteLine("Part 2: " + Part2(bricks));
    Console.ReadLine();
}

long Part1(Brick[] bricks)
{
    return bricks.Count(brick => brick.CanDisintegrateIsolated());
}

long Part2(Brick[] bricks)
{
    return bricks.Sum(brick => brick.CountFallingBricksWhenDisintegrated());
}

public record struct XYZCoordinate (int X, int Y, int Z);
public record struct XYCoordinate (int X, int Y);
public record struct BrickColumn(int Height, int Position, Brick Brick);

public class PlaygroundColumn: Stack<BrickColumn>
{
    public int Height()
    {
        if(this.TryPeek(out var brickColumn))
        {
            return brickColumn.Position + brickColumn.Height - 1;
        }
        return 0;
    }

    internal void PlaceBrick(Brick brick, int position, int height)
    {
        BrickColumn brickColumn = new BrickColumn(height, position, brick);

        if(this.Height() == position - 1)
        {
            if(TryPeek(out var currentTop))
            {
                currentTop.Brick.TopBricks.Add(brick);
                brick.BottomBricks.Add(currentTop.Brick);
            }
        }

        Push(brickColumn);
    }
}

public class Playground: Dictionary<XYCoordinate, PlaygroundColumn>
{
    public PlaygroundColumn GetColumn(XYCoordinate coordinate)
    {
        if(!this.TryGetValue(coordinate, out var column))
        {
            column = new PlaygroundColumn();
            this.Add(coordinate, column);
        }
        return column;
    }

    public IEnumerable<PlaygroundColumn> GetColumns(IEnumerable<XYCoordinate> coordinates)
    {
        return coordinates.Select(GetColumn);
    }

    public int GetMaxHeight(IEnumerable<XYCoordinate> coordinates)
    {
        return GetColumns(coordinates).Max(c => c.Height());
    }

    public void PlaceBrick(Brick brick)
    {
        var brickColumns = brick.GetColumns();
        int newPosition = GetMaxHeight(brickColumns) + 1;

        foreach(var playgroundColumn in GetColumns(brickColumns))
        {
            playgroundColumn.PlaceBrick(brick, newPosition, brick.Height);
        }
    }
}

public class Brick
{
    public Brick(string input)
    {
        var numbers = Regex.Matches(input, "[0-9]+").Select(m => int.Parse(m.Value)).ToArray();

        FirstCoordinate = new XYZCoordinate(numbers[0], numbers[1], numbers[2]);
        SecondCoordinate = new XYZCoordinate(numbers[3], numbers[4], numbers[5]);
    }

    public readonly XYZCoordinate FirstCoordinate;
    public readonly XYZCoordinate SecondCoordinate;

    public readonly HashSet<Brick> BottomBricks = [];
    public readonly HashSet<Brick> TopBricks = [];

    public IEnumerable<XYCoordinate> GetColumns()
    {
        int minX = Math.Min(FirstCoordinate.X, SecondCoordinate.X);
        int maxX = Math.Max(FirstCoordinate.X, SecondCoordinate.X);
        int minY = Math.Min(FirstCoordinate.Y, SecondCoordinate.Y);
        int maxY = Math.Max(FirstCoordinate.Y, SecondCoordinate.Y);

        List<XYCoordinate> columns = new List<XYCoordinate>();

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                columns.Add(new XYCoordinate { X = x, Y = y });
            }
        }

        return columns.ToArray();
    }

    public int Height => Math.Abs(SecondCoordinate.Z - FirstCoordinate.Z) + 1;
    
    public bool CanDisintegrateIsolated() => TopBricks.All(b => b.BottomBricks.Count > 1);

    public bool MustFall(HashSet<Brick> alreadyGone) => !BottomBricks.Except(alreadyGone).Any();

    public int CountFallingBricksWhenDisintegrated()
    {
        HashSet<Brick> alreadyFallen = [];
        Queue<Brick> queue = new([this]);

        while (queue.TryDequeue(out var brick))
        {
            if (alreadyFallen.Contains(brick)) continue;

            if ((brick == this) || brick.MustFall(alreadyFallen))
            {
                alreadyFallen.Add(brick);

                foreach (var nextBrick in brick.TopBricks)
                {
                    queue.Enqueue(nextBrick);
                }
            }
        }

        return alreadyFallen.Except([this]).Count();
    }

}