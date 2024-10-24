﻿#define Sample
using System.Text;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day17\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2022\Day17\Full.txt";
#endif

    string outputNamePart1 = @"D:\Dropbox\Work\AdventOfCode\2022\Day17\VisualizationPart1.txt";
    // string outputNamePart2 = @"D:\Dropbox\Work\AdventOfCode\2022\Day17\VisualizationPart2.txt";

    string flow = File.ReadAllText(fileName);

    Console.WriteLine("Part 1: " + Solve(flow, 2022, outputNamePart1));
    Console.WriteLine("Part 2: " + Solve(flow, 1_000_000_000_000, ""));
    Console.ReadLine();
}

long Solve(string flow, long maxRocks, string outputName)
{
    Map map = new Map(flow);

    // TODO: Add cycle detection for part 2
    // Idea: Use a dictionary with key consisting of WindPosition, nextRockShape and several top lines of the tower
    // and value being the count of fixed rocks

    while (map.FixedRocks < maxRocks)
    {
        map.PerformStep();
    }

    if(outputName != "") map.SaveTo(outputName);

    return Math.Abs(map.MinY);
}

public class Map
{
    HashSet<Point> RestingRock = [];
    HashSet<Point> MovingRock = [];

    private string Flow;
    private int WindPosition = 0;
    private int nextRockShape = 0;
    
    public long MinY = 0;
    public long FixedRocks = 0;

    public Map(string flow)
    {
        Flow = flow;
    }

    private bool IsHard(Point point)
    {
        if (point.Y >= 0) return true;
        if (point.X <= 0) return true;
        if (point.X >= 8) return true;
        return RestingRock.Contains(point);
    }

    public void PerformStep()
    {
        if (!MovingRock.Any())
        {
            SpawnRock();
        }

        ApplyWind();

        if (!MoveDown())
        {
            FixateRock();
        }
    }

    private void SpawnRock()
    {
        if (nextRockShape > 4) nextRockShape = 0;

        switch (nextRockShape)
        {
            case 0:
                MovingRock.Add(new Point(3, MinY - 4));
                MovingRock.Add(new Point(4, MinY - 4));
                MovingRock.Add(new Point(5, MinY - 4));
                MovingRock.Add(new Point(6, MinY - 4));
                break;
            case 1:
                MovingRock.Add(new Point(4, MinY - 6));
                MovingRock.Add(new Point(3, MinY - 5));
                MovingRock.Add(new Point(4, MinY - 5));
                MovingRock.Add(new Point(5, MinY - 5));
                MovingRock.Add(new Point(4, MinY - 4));
                break;
            case 2:
                MovingRock.Add(new Point(5, MinY - 6));
                MovingRock.Add(new Point(5, MinY - 5));
                MovingRock.Add(new Point(3, MinY - 4));
                MovingRock.Add(new Point(4, MinY - 4));
                MovingRock.Add(new Point(5, MinY - 4));
                break;
            case 3:
                MovingRock.Add(new Point(3, MinY - 7));
                MovingRock.Add(new Point(3, MinY - 6));
                MovingRock.Add(new Point(3, MinY - 5));
                MovingRock.Add(new Point(3, MinY - 4));
                break;
            case 4:
                MovingRock.Add(new Point(3, MinY - 5));
                MovingRock.Add(new Point(4, MinY - 5));
                MovingRock.Add(new Point(3, MinY - 4));
                MovingRock.Add(new Point(4, MinY - 4));
                break;
        }

        nextRockShape++;
    }

    private bool MoveRock(Direction direction)
    {
        HashSet<Point>  newPoints = MovingRock.Select(p => p.GetNeightboringPoint(direction)).ToHashSet();

        bool canMove = newPoints.All(p => !IsHard(p));
        if (canMove)
        {
            MovingRock = newPoints;
        }
        return canMove;
    }

    private void ApplyWind()
    {
        if (WindPosition >= Flow.Length) WindPosition = 0;
        Direction direction = (Direction)Flow[WindPosition];
        WindPosition++;

        MoveRock(direction);
    }

    private bool MoveDown()
    {
        return MoveRock(Direction.South);
    }

    private void FixateRock()
    {
        MinY = Math.Min(MinY, MovingRock.Min(p => p.Y));
        RestingRock.UnionWith(MovingRock);
        MovingRock.Clear();
        FixedRocks++;
    }

    public void SaveTo(string fileName)
    {
        List<string> lines = [];

        int currentMinX = 0;
        int currentMaxX = 8;

        long minY = RestingRock.Union(MovingRock).Min(p => p.Y);


        for (long row = minY; row < 0 ; row++)
        {
            StringBuilder sb = new StringBuilder();

            for (int col = currentMinX; col <= currentMaxX; col++)
            {
                Point p = new Point(col, row);

                string toAppend = ".";

                if (RestingRock.Contains(p))
                {
                    toAppend = "#";
                }
                else if (MovingRock.Contains(p))
                {
                    toAppend = "@";
                }
                else if (col == 0 || col == 8)
                {
                    toAppend = "|";
                }


                sb.Append(toAppend);
            }
            lines.Add(sb.ToString());
        }

        lines.Add("+-------+");

        File.WriteAllLines(fileName, lines);
    }
}

record struct Point(long X, long Y)
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
    South = 'v',
    West = '<',
    North = '^',
    East = '>'
}
