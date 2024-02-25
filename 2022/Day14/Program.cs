// #define Sample
using System.Text;
using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day14\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day14\Full.txt";
#endif

    string outputNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2022\Day14\VisualizationPart1.txt";
    string outputNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2022\Day14\VisualizationPart2.txt";


    Console.WriteLine("Part 1: " + Solve(fileName, outputNamePart1, useFloor: false));
    Console.WriteLine("Part 2: " + Solve(fileName, outputNamePart2, useFloor: true));
    Console.ReadLine();
}

long Solve(string inputFileName, string outputFileName, bool useFloor)
{
    Map map = new Map(File.ReadAllLines(inputFileName), useFloor);
    map.FlowSand();
    map.SaveTo(outputFileName);
    return map.CountRestingSand();
}

public class Map
{
    HashSet<Point> Walls = [];
    HashSet<Point> FlowingSand = [];
    HashSet<Point> RestingSand = [];
    Point Spring = new Point(500, 0);

    long MaxY;
    bool UseFloor;

    public Map(string[] lines, bool useFloor)
    {
        foreach (var line in lines)
        {
            AddWalls(line);
        }

        ReduceX();

        MaxY = Walls.Select(p => p.Y).Max() + 2;
        UseFloor = useFloor;
    }

    private void AddWalls(string line)
    {
        Point[] wallCorners = 
            Regex.Matches(line, "(\\d+),(\\d+)")
            .Select(m => new Point(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)))
            .ToArray();

        foreach (var pair in wallCorners.Zip(wallCorners.Skip(1)))
        {
            AddWall(pair.First, pair.Second);
        }
    }

    private void AddWall(Point first, Point second)
    {
        int minX = Math.Min(first.X, second.X);
        int minY = Math.Min(first.Y, second.Y);
        int maxX = Math.Max(first.X, second.X);
        int maxY = Math.Max(first.Y, second.Y);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Walls.Add(new Point(x, y));
            }
        }
    }

    private void ReduceX()
    {
        int minX = Walls.Select(p => p.X).Min();
        Walls = Walls.Select(p => new Point(p.X - minX, p.Y)).ToHashSet();
        Spring = new Point(Spring.X - minX, 0);
    }

    public void FlowSand()
    {
        while (SpawnGrain());
    }

    private bool SpawnGrain()
    {
        FlowingSand.Clear();
        
        Point grain = Spring;

        while (!RestingSand.Contains(Spring))
        {
            FlowingSand.Add(grain);

            Point below = grain.GetNeightboringPoint(Direction.South);
            Point belowLeft = grain.GetNeightboringPoint(Direction.SouthWest);
            Point belowRight = grain.GetNeightboringPoint(Direction.SouthEast);

            if (IsHard(below))
            {
                if (IsHard(belowLeft))
                {
                    if (IsHard(belowRight))
                    {
                        RestingSand.Add(grain);
                        return true;
                    }
                    else
                    {
                        grain = belowRight;
                    }
                }
                else
                {
                    grain = belowLeft;
                }
            }
            else
            {
                grain = below;
            }

            if (grain.Y > MaxY) return false;
        }

        return false;
    }

    private bool IsHard(Point point)
    {
        if(UseFloor && point.Y == MaxY) return true;
        return Walls.Contains(point) || RestingSand.Contains(point);
    }

    public long CountRestingSand()
    {
        return RestingSand.Count();
    }

    public void SaveTo(string fileName)
    {
        List<string> lines = [];

        int currentMinX = Walls.Union(FlowingSand).Union(RestingSand).Select(p => p.X).Min();
        int currentMaxX = Walls.Union(FlowingSand).Union(RestingSand).Select(p => p.X).Max();

        for (int row = 0; row <= MaxY; row++)
        {
            StringBuilder sb = new StringBuilder();

            for (int col = currentMinX; col <= currentMaxX; col++)
            {
                Point p = new Point(col, row);

                string toAppend = ".";

                if (Walls.Contains(p))
                {
                    toAppend = "#";
                }
                else if (RestingSand.Contains(p))
                {
                    toAppend = "o";
                }
                else if (p == Spring)
                {
                    toAppend = "+";
                }
                else if (FlowingSand.Contains(p))
                {
                    toAppend = "~";
                }
                else if (UseFloor && row == MaxY)
                {
                    toAppend = "-";
                }

                sb.Append(toAppend);
            }
            lines.Add(sb.ToString());
        }
        File.WriteAllLines(fileName, lines);
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
            Direction.SouthWest => this with { X = this.X - 1, Y = this.Y + 1 },
            Direction.SouthEast => this with { X = this.X + 1, Y = this.Y + 1 },
            Direction.NorthEast => this with { X = this.X + 1, Y = this.Y - 1 },
            Direction.NorthWest => this with { X = this.X - 1, Y = this.Y - 1 },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }
}

enum Direction
{
    South,
    West,
    North,
    East,
    NorthEast,
    NorthWest,
    SouthEast,
    SouthWest
}
