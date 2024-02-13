// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day11\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day11\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);

    Grid grid = new Grid(input);

    int step = 0;
    int flashesSum = 0;
    bool allFlashed = false;

    while (step < 100 || !allFlashed)
    {
        int flashes = grid.Step();
        flashesSum += flashes;
        step++;

        if (step == 100) Console.WriteLine("Part 1: " + flashesSum);
        if (flashes == grid.Width * grid.Height)
        {
            Console.WriteLine("Part 2: " + step);
            allFlashed = true;
        }
    }

    Console.ReadLine();
}

public class Grid
{
    public Grid(string[] input)
    {
        Width = input.First().Length;
        Height = input.Length;

        InitializeEnergyLevels(input);
    }

    private void InitializeEnergyLevels(string[] input)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Point point = new Point(x, y);
                char ch = input[y][x];
                EnergyLevels[point] = (int)char.GetNumericValue(ch);
            }
        }
    }

    public int Height;
    public int Width;

    private Dictionary<Point, int> EnergyLevels = [];

    private bool IsInGrid(Point point)
    {
        return
            point.X >= 0
            && point.X < Width
            && point.Y >= 0
            && point.Y < Height;
    }


    public int Step()
    {
        var allPoints = EnergyLevels.Keys.ToArray();
        
        foreach(var point in allPoints)
        {
            EnergyLevels[point]++;
        }

        HashSet<Point> flashes = [];
        Queue<Point> queue = [];

        foreach(var point in EnergyLevels.Where(p => p.Value > 9).Select(p => p.Key))
        {
            queue.Enqueue(point);
        }

        while(queue.TryDequeue(out var point))
        {
            if (flashes.Contains(point)) continue;
            
            flashes.Add(point);
            EnergyLevels[point] = 0;
            
            foreach(var neighbor in point.GetAllNeighboringPoints().Except(flashes))
            {
                if (!IsInGrid(neighbor)) continue;

                EnergyLevels[neighbor]++;
                if(EnergyLevels[neighbor] > 9)
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        return flashes.Count;
    }
}


public record Point(int X, int Y)
{
    public Point GetNeightboringPoint(Direction direction)
    {
        return direction switch
        {
            Direction.South => this with { Y = this.Y + 1 },
            Direction.West => this with { X = this.X - 1 },
            Direction.North => this with { Y = this.Y - 1 },
            Direction.East => this with { X = this.X + 1 },

            Direction.SouthWest => this with { X = this.X - 1, Y = this.Y + 1 },
            Direction.SouthEast => this with { X = this.X + 1, Y = this.Y + 1 },
            Direction.NorthEast => this with { X = this.X + 1, Y = this.Y - 1 },
            Direction.NorthWest => this with { X = this.X - 1, Y = this.Y - 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }

    public Point[] GetAllNeighboringPoints()
    {
        return AllDirections.Select(d => GetNeightboringPoint(d)).ToArray();
    }
    
    private static Direction[] AllDirections = 
    [
        Direction.East,
        Direction.North,
        Direction.South,
        Direction.West,
        Direction.NorthEast,
        Direction.SouthEast,
        Direction.NorthWest,
        Direction.SouthWest
    ];

}

public enum Direction
{
    West,
    East,
    North,
    South,
    NorthEast,
    NorthWest,
    SouthEast,
    SouthWest
}
