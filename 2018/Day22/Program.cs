// #define Sample

using System.Text.RegularExpressions;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day22\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day22\Full.txt";
#endif

    long[] values = 
        Regex.Matches(File.ReadAllText(fileName), "\\d+")
        .Select(m => long.Parse(m.Value))
        .ToArray();

    Maze maze = new Maze(values[0], new Point(values[1], values[2]));

    Console.WriteLine("Part 1: " + Part1(maze));
    Console.WriteLine("Part 2: " + Part2(maze));
    Console.ReadLine();
}

long Part1(Maze maze)
{
    long sum = 0;
    for(int x= 0; x <= maze.Target.X; x++)
    {
        for (int y = 0; y <= maze.Target.Y; y++)
        {
            sum += maze.GetRegion(new Point(x, y)).RiskLevel;
        }
    }
    return sum;
}

// 1036 too low ???
// 1037 too low

long Part2(Maze maze)
{
    return maze.FindShortestPath();
}

public class Maze
{
    public long Depth;
    public Point Mouth;
    public Point Target;

    private Dictionary<Point, Region> regions = [];

    public Maze(long depth, Point target)
    {
        Depth = depth;
        Mouth = new Point(0, 0);
        Target = target;
    }

    public Region GetRegion(Point point)
    {
        if(!regions.TryGetValue(point, out var region))
        {
            region = new Region(this, point);
            regions.Add(point, region);
        }

        return region;
    }

    public int FindShortestPath()
    {
        HashSet<(Point Point, Tool Tool)> visited = [];
        PriorityQueue<(Point Point, Tool Tool, int Minutes), int> todos = new();
        todos.Enqueue((Mouth, Tool.Torch, 0), 0);

        while(todos.TryDequeue(out var current, out _))
        {
            if (visited.Contains((current.Point, current.Tool))) continue;
            visited.Add((current.Point, current.Tool));

            if (current.Point == Target)
            {
                return current.Minutes;
            }

            foreach(var direction in AllDirections)
            {
                var nextPoint = current.Point.GetNeightboringPoint(direction);
                
                if(nextPoint.X < 0 || nextPoint.Y < 0) continue;
                
                var currentlyAllowedTools = GetToolsForRegionType(GetRegion((current.Point)).RegionType);
                var nextAllowedTools = GetToolsForRegionType(GetRegion((nextPoint)).RegionType); 

                foreach (var nextTool in currentlyAllowedTools.Intersect(nextAllowedTools))
                {
                    var nextMinutes = (current.Tool == nextTool) ? current.Minutes + 1 : current.Minutes + 1 + 7;
                    todos.Enqueue((nextPoint, nextTool, nextMinutes), nextMinutes);
                }
            }
        }

        return 0;
    }

    Tool[] GetToolsForRegionType(RegionType type)
    {
        return type switch
        {
            RegionType.Rocky => [Tool.ClimbingGear, Tool.Torch],
            RegionType.Wet => [Tool.ClimbingGear, Tool.Neither],
            RegionType.Narrow => [Tool.Torch, Tool.Neither],
            RegionType.Mouth or RegionType.Target or _ => [Tool.Torch],
        }; 
    }

    private static Direction[] AllDirections = [Direction.East, Direction.North, Direction.South, Direction.West];
}

public record struct Region
{
    public Region(Maze maze, Point point)
    {
        GeologicIndex = MakeGeologicIndex(maze, point);
        ErosionLevel = (GeologicIndex + maze.Depth) % 20183;
        
        RegionType = (ErosionLevel % 3) switch
        {
            0 => RegionType.Rocky,
            1 => RegionType.Wet,
            2 or _ => RegionType.Narrow,
        };
        if (point == maze.Mouth) RegionType = RegionType.Mouth;
        if (point == maze.Target) RegionType = RegionType.Target;

        RiskLevel = RegionType switch
        {
            RegionType.Wet => 1,
            RegionType.Narrow => 2,
            RegionType.Rocky or RegionType.Mouth or RegionType.Target or _ => 0,
        };
    }

    private long MakeGeologicIndex(Maze maze, Point point)
    {
        if (point == maze.Mouth) return 0;
        if (point == maze.Target) return 0;
        if (point.Y == 0) return point.X * 16807;
        if (point.X == 0) return point.Y * 48271;
        return 
            maze.GetRegion(point with { X = point.X - 1}).ErosionLevel * 
            maze.GetRegion(point with { Y = point.Y - 1 }).ErosionLevel;
    }

    public Point Point;
    public long GeologicIndex;
    public long ErosionLevel;
    public RegionType RegionType;
    public long RiskLevel;
}

public record struct Point
{
    public long X;
    public long Y;

    public Point(long x, long y)
    {
        X = x;
        Y = y;
    }

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

public enum RegionType
{
    Rocky,
    Wet,
    Narrow,
    Mouth,
    Target
}

public enum Tool
{
    Torch,
    ClimbingGear,
    Neither
}

public enum Direction
{
    South,
    West,
    North,
    East
}
