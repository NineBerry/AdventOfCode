// #define Sample

using Path = Point[];

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day20\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2018\Day20\Full.txt";
#endif

    string input = File.ReadAllText(fileName);
    (Path[] paths, Path[] detours) = GetPaths(input);

    Console.WriteLine("Part 1: " + Part1(paths));
    Console.WriteLine("Part 2: " + Part2(paths, detours));

    Console.ReadLine();
}


int Part1(Path[] paths)
{
    // Group paths by end point, then get shortest path per end point
    var shortestPathsForRooms =
        paths
        .GroupBy(p => p.Last())
        .Select(g => g.OrderBy(p => p.Length).First())
        .ToList();

    // Get length of longest path
    return shortestPathsForRooms.Max(p => p.Length) ;
}

int Part2(Path[] paths, Path[] detours)
{
    paths = [.. paths, .. detours];
    
    HashSet<Point> allPoints = paths.SelectMany(path => path).ToHashSet();
    var reachableWith999Steps = paths.SelectMany(s => s.Take(999));

    return allPoints.Except(reachableWith999Steps).Count();
}

(Path[] Paths, Path[] Detours) GetPaths(string input)
{
    List<string> skipped = [];
    
    Stack<List<HashSet<string>>> stack = [];
    stack.Push([[""]]);

    string currString = "";

    foreach ((var ch, int i) in input.Select((ch, i) => (ch, i)))
    {

        switch (ch)
        {
            case '^':
            case '$':
                // Ignore
                break;

            case '(':
                currString = "";
                stack.Push([[..stack.Peek()[^1]]]);
                break;

            case '|':
                if (input[i+1] == ')' && IsDetour(currString))
                {
                    foreach(var s in stack.Peek()[^1])
                    {
                        skipped.Add(s);
                    }

                    stack.Peek()[^1] = [];
                }
                stack.Peek().Add([.. stack.Skip(1).First()[^1]]);
                currString = "";
                break;

            case ')':
                currString = "";

                var toCollapse = stack.Pop();
                stack.Peek()[^1] = toCollapse.SelectMany(l => l).ToHashSet();
                break;

            default:
                currString += ch;
                stack.Peek()[^1] = stack.Peek()[^1].Select(s => s + ch).ToHashSet();
                break;
        }
    }

    var paths = stack.Pop().Single().Select(GetPath);
    var detours = skipped.Select(GetPath);

    return ([..paths], [..detours]);
}

bool IsDetour(string path)
{
    if(path == "") return true; 

    return GetPath(path).Last() == new Point(0,0);
}

Point[] GetPath(string stringPath)
{
    Point point = new Point(0, 0);

    List<Point> pointPath = [];
    foreach (var ch in stringPath)
    {
        point = point.GetNeightboringPoint((Direction)ch);
        pointPath.Add(point);
    }

    return pointPath.ToArray();
}

record Point(int X, int Y)
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
    South = 'S',
    West = 'W',
    North = 'N',
    East = 'E'
}