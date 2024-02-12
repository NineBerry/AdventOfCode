// #define Sample
using System.Text;
using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day17\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day17\Full.txt";
#endif

    string outputName = @"D:\Dropbox\Work\AdventOfCode\2018\Day17\output.txt";

    var lines = File.ReadAllLines(fileName);
    Map map = new Map(lines);
    map.FlowWater();
    map.SaveTo(outputName);

    Console.WriteLine("Part 1: " + map.CountAllWaterSpots());
    Console.WriteLine("Part 2: " + map.CountRestingWaterSpots());
    Console.ReadLine();
}

public class Map
{
    HashSet<Point> Walls = [];
    HashSet<Point> FlowingWater = [];
    HashSet<Point> RestingWater = [];
    Point Spring = new Point(500, 0);

    long MinX;
    long MaxX;
    long MinY;
    long MaxY;

    public Map(string[] lines)
    {
        foreach (var line in lines)
        {
            AddWall(line);
        }

        ReduceX();

        MinX = Walls.Select(p => p.X).Min();
        MaxX = Walls.Select(p => p.X).Max();
        MinY = Walls.Select(p => p.Y).Min();
        MaxY = Walls.Select(p => p.Y).Max();
    }

    private void AddWall(string line)
    {
        (int minX, int maxX) = GetWall(line, 'x');
        (int minY, int maxY) = GetWall(line, 'y');

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Walls.Add(new Point(x, y));
            }
        }
    }

    private (int min, int max) GetWall(string line, char coordinate)
    {
        string coordinatesString = Regex.Match(line, coordinate + "=([0-9.]+)").Groups[1].Value;
        int[] values = coordinatesString.Split("..").Select(int.Parse).ToArray();

        if (values.Length == 1) return (values[0], values[0]);
        return (values[0], values[1]);
    }

    private void ReduceX()
    {
        long minX = Walls.Select(p => p.X).Min();
        Walls = Walls.Select(p => new Point(p.X - minX, p.Y)).ToHashSet();
        Spring = new Point(Spring.X - minX, 0);
    }

    public void FlowWater()
    {
        Queue<Point> queue = [];
        queue.Enqueue(Spring);

        while(queue.TryDequeue(out var point))
        {
            if (point.Y >= MaxY) continue;

            Point below = point.GetNeightboringPoint(Direction.South);

            if (Walls.Contains(below) || RestingWater.Contains(below))
            {
                // Look in both directions, then decide how to continue
                (var leftPoints, var leftHitWall) = CheckInDirection(point, Direction.West);
                (var rightPoints, var rightHitWall) = CheckInDirection(point, Direction.East);

                if(leftHitWall && rightHitWall)
                {
                    FlowingWater.Remove(point);
                    RestingWater.UnionWith(leftPoints);
                    RestingWater.UnionWith(rightPoints);
                    queue.Enqueue(point.GetNeightboringPoint(Direction.North));
                }
                else
                {
                    FlowingWater.UnionWith(leftPoints);
                    FlowingWater.UnionWith(rightPoints);

                    if (!leftHitWall) queue.Enqueue(leftPoints.Last());
                    if (!rightHitWall) queue.Enqueue(rightPoints.Last());
                }
            }
            else if (FlowingWater.Contains(below))
            {
                continue;
            }
            else
            {
                FlowingWater.Add(below);
                queue.Enqueue(below);
            }
        }
    }

    private (List<Point> Points, bool HitWall) CheckInDirection(Point start, Direction direction)
    {
        List<Point> visited = [start];
        bool hitWall = false;
        Point next = start.GetNeightboringPoint(direction);

        while (true)
        {
            if (Walls.Contains(next))
            {
                hitWall = true;
                break;
            }

            Point below = next.GetNeightboringPoint(Direction.South);
            if(!Walls.Contains(below) && !RestingWater.Contains(below))
            {
                visited.Add(next);
                break;
            }

            visited.Add(next);
            next = next.GetNeightboringPoint(direction);
        }

        return (visited, hitWall);
    }

    public long CountAllWaterSpots()
    {
        return 
            FlowingWater
            .Union(RestingWater)
            .Where(p => p.Y >= MinY && p.Y <= MaxY)
            .Count();
    }

    public long CountRestingWaterSpots()
    {
        return
            RestingWater
            .Where(p => p.Y >= MinY && p.Y <= MaxY)
            .Count();
    }

    public void SaveTo(string fileName)
    {
        List<string> lines = [];

        long currentMinX = Walls.Union(FlowingWater).Union(RestingWater).Select(p => p.X).Min();
        long currentMaxX = Walls.Union(FlowingWater).Union(RestingWater).Select(p => p.X).Max();

        for (long row = 0; row <= MaxY; row++)
        {
            StringBuilder sb = new StringBuilder();

            for (long col = currentMinX; col <= currentMaxX; col++)
            {
                Point p = new Point(col, row);

                string toAppend = ".";

                if (Walls.Contains(p))
                {
                    toAppend = "#";
                }
                else if (RestingWater.Contains(p))
                {
                    toAppend = "~";
                }
                else if (FlowingWater.Contains(p))
                {
                    toAppend = "|";
                }
                else if (p == Spring)
                {
                    toAppend = "+";
                }

                sb.Append(toAppend);
            }
            lines.Add(sb.ToString());
        }
        File.WriteAllLines(fileName, lines);
    }
}

record struct Point(long X, long Y)
{
    public Point GetNeightboringPoint(Direction direction, long distance = 1)
    {
        return direction switch
        {
            Direction.South => this with { Y = this.Y + distance },
            Direction.West => this with { X = this.X - distance },
            Direction.North => this with { Y = this.Y - distance },
            Direction.East => this with { X = this.X + distance },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }
}

enum Direction
{
    South,
    West,
    North,
    East
}
