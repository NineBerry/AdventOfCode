// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day07\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2025\Day07\Full.txt";
#endif

    string outputName = @"D:\Dropbox\Work\AdventOfCode\2025\Day07\output.txt";

    var lines = File.ReadAllLines(fileName);

    Console.WriteLine("Part 1: " + Part1(lines, outputName));
    Console.WriteLine("Part 2: " + Part2(lines));
    Console.ReadLine();
}

long Part1(string[] lines, string outputName)
{
    // After implementing part 2, we could just use part 2 instead for part 1
    // and count the distinct splitters we encounter during the algorithm.
    // But we already have this code and it runs fast and gives us the nice
    // visualization...

    Map map = new Map(lines);
    map.FlowBeam();
    map.SaveTo(outputName);
    return map.CountActivatedSplitters();
}

long Part2(string[] lines)
{
    Map map = new Map(lines);
    return map.CountTimelines();
}

public class Map
{
    private HashSet<Point> Splitters = [];
    private HashSet<Point> Beam = [];
    private Point Start;

    private int Height;
    private int Width;

    public Map(string[] lines)
    {
        Height = lines.Length;
        Width = lines.First().Length;

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                char tile = lines[y][x];
                if (tile == '^') Splitters.Add(new Point(x, y));
                if (tile == 'S') Start = new Point(x, y);
            }
        }
    }

    public void FlowBeam()
    {
        Queue<Point> queue = [];
        queue.Enqueue(Start);

        while (queue.TryDequeue(out var point))
        {
            // don't go below map
            if (point.Y >= Height) continue;

            // If there already is a beam, then we don't need to continue
            if (Beam.Contains(point)) continue;

            Beam.Add(point);

            Point below = point.GetNeightboringPoint(Direction.Down);

            if (Splitters.Contains(below))
            {
                // Look in both directions, then decide how to continue
                queue.Enqueue(below.GetNeightboringPoint(Direction.Left));
                queue.Enqueue(below.GetNeightboringPoint(Direction.Right));
            }
            else
            {
                queue.Enqueue(below);
            }
        }
    }

    public long CountTimelines()
    {
        Dictionary<Point, long> timelinesCache = [];

        return TraverseTimelines(Start);

        long TraverseTimelines(Point currentPoint)
        {
            if(timelinesCache.TryGetValue(currentPoint, out long cached)) return cached;

            long result = 0;

            if (currentPoint.Y >= Height)
            {
                result = 1;
            }
            else if(Splitters.Contains(currentPoint))
            {
                result = 
                    TraverseTimelines(currentPoint.GetNeightboringPoint(Direction.Left))
                    + TraverseTimelines(currentPoint.GetNeightboringPoint(Direction.Right));
            }
            else
            {
                result = TraverseTimelines(currentPoint.GetNeightboringPoint(Direction.Down));
            }

            timelinesCache.Add(currentPoint, result);
            return result;
        }
    }

    public long CountActivatedSplitters()
    {
        // Splitters that have a beam at their entry
        return Splitters.Count(splitter => Beam.Contains(splitter.GetNeightboringPoint(Direction.Up)));
    }

    public void SaveTo(string fileName)
    {
        List<string> lines = [];

        for (long row = 0; row < Height; row++)
        {
            System.Text.StringBuilder sb = new();

            for (long col = 0; col < Width; col++)
            {
                Point p = new Point(col, row);

                string toAppend = ".";

                if (Splitters.Contains(p))
                {
                    toAppend = "^";
                }
                else if (Beam.Contains(p))
                {
                    toAppend = "|";
                }
                else if (p == Start)
                {
                    toAppend = "S";
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
            Direction.Down => this with { Y = this.Y + distance },
            Direction.Left => this with { X = this.X - distance },
            Direction.Up => this with { Y = this.Y - distance },
            Direction.Right => this with { X = this.X + distance },
            _ => throw new ArgumentException("Unknown direction", nameof(direction)),
        };
    }
}

enum Direction
{
    Down,
    Left,
    Up,
    Right
}

